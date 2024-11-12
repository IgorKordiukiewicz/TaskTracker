<template>
    <ActionDialog header="Add a member" submit-label="Confirm" @submit="addMember" ref="dialog">
        <LabeledInput label="User">
            <AutoComplete v-model="selectedUser" option-label="email" :suggestions="filteredUsers" @complete="searchUsers"  :inputStyle="{ 'width': '100%' }" dropdown />
        </LabeledInput>
    </ActionDialog>
</template>

<script setup lang="ts">
import type { AutoCompleteCompleteEvent } from 'primevue/autocomplete';
import type { PropType } from 'vue';
import { AddProjectMemberDto } from '~/types/dtos/projects';
import type { UserVM } from '~/types/viewModels/users';

defineExpose({ show });
const emit = defineEmits([ 'onAdd' ]);
const props = defineProps({
    projectId: { type: String, required: true },
    availableUsers: { type: Object as PropType<UserVM[]>, required: true }
});

const projectsService = useProjectsService();

const dialog = ref();

const selectedUser = ref();
const filteredUsers = ref();

function show() {
    dialog.value.show();
}

async function addMember() {
    const model = new AddProjectMemberDto();
    model.userId = selectedUser.value.id;

    await projectsService.addMember(props.projectId, model);

    selectedUser.value = null;
    filteredUsers.value = null;

    emit('onAdd');
}

function searchUsers(event: AutoCompleteCompleteEvent) {
    filteredUsers.value = props.availableUsers.filter(x => x.email.includes(event.query));
}
</script>