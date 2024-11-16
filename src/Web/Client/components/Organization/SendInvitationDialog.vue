<template>
    <ActionDialog header="Send an invitation" submit-label="Send" @submit="sendInvitation" ref="dialog" :submit-disabled="submitDisabled">
        <LabeledInput label="User">
            <AutoComplete v-model="selectedUser" option-label="email" :suggestions="filteredUsers" @complete="searchUsers" :inputStyle="{ 'width': '100%' }" />
        </LabeledInput>
    </ActionDialog>
</template>

<script setup lang="ts">
import type { AutoCompleteCompleteEvent } from 'primevue/autocomplete';
import { CreateOrganizationInvitationDto } from '~/types/dtos/organizations';

defineExpose({ show });
const emit = defineEmits([ 'onCreate' ]);
const props = defineProps({
    organizationId: { type: String, required: true }
})

const organizationsService = useOrganizationsService();
const usersService = useUsersService();

const dialog = ref();

const selectedUser = ref();
const filteredUsers = ref();

const submitDisabled = computed(() => {
    return !selectedUser.value;
})

function show() {
    dialog.value.show();
}

async function sendInvitation() {
    const model = new CreateOrganizationInvitationDto();
    model.userId = selectedUser.value.id;
    await organizationsService.createInvitation(props.organizationId, model);

    selectedUser.value = undefined;

    emit('onCreate');
}

async function searchUsers(event: AutoCompleteCompleteEvent) {
    filteredUsers.value = (await usersService.getAvailableForInvitation(props.organizationId, event.query))?.users;
}
</script>