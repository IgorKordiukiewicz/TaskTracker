<template>
    <ActionDialog header="Send an invitation" submit-label="Send" @submit="sendInvitation" ref="dialog" :submit-disabled="submitDisabled">
        <LabeledInput label="User">
            <AutoComplete v-model="selectedUser" option-label="email" :suggestions="filteredUsers" @complete="searchUsers" :inputStyle="{ 'width': '100%' }" />
        </LabeledInput>
        <LabeledInput label="Expiration days">
            <InputNumber v-model="expirationDays" :min="0" />
        </LabeledInput>
    </ActionDialog>
</template>

<script setup lang="ts">
import type { AutoCompleteCompleteEvent } from 'primevue/autocomplete';
import { CreateProjectInvitationDto } from '~/types/dtos/projects';

defineExpose({ show });
const emit = defineEmits([ 'onCreate' ]);
const props = defineProps({
    projectId: { type: String, required: true }
})

const projectsService = useProjectsService();
const usersService = useUsersService();

const dialog = ref();

const selectedUser = ref();
const filteredUsers = ref();
const expirationDays = ref();

const submitDisabled = computed(() => {
    return !selectedUser.value;
})

function show() {
    dialog.value.show();
}

async function sendInvitation() {
    const model = new CreateProjectInvitationDto();
    model.userId = selectedUser.value.id;
    model.expirationDays = expirationDays.value;
    await projectsService.createInvitation(props.projectId, model);

    selectedUser.value = undefined;
    expirationDays.value = undefined;

    emit('onCreate');
}

async function searchUsers(event: AutoCompleteCompleteEvent) {
    filteredUsers.value = (await usersService.getAvailableForInvitation(props.projectId, event.query))?.users;
}
</script>