<template>
    <OrganizationLayout>
        <p class="text-lg">Members</p>
        <MembersList v-if="members && roles" :members="members.members" :roles="roles.roles" 
        @on-update-member-role="updateMemberRole" @on-remove-member="removeMember" />
    </OrganizationLayout>
</template>

<script setup lang="ts">
import { RemoveMemberDto, UpdateMemberRoleDto } from '~/types/dtos/shared';

const route = useRoute();
const organizationsService = useOrganizationsService();

const organizationId = ref(route.params.id as string);

const members = ref(await organizationsService.getMembers(organizationId.value));
const roles = ref(await organizationsService.getRoles(organizationId.value));

async function updateMembers() {
    members.value = await organizationsService.getMembers(organizationId.value);
}

async function updateMemberRole(model: UpdateMemberRoleDto) {
    await organizationsService.updateMemberRole(organizationId.value, model);
    await updateMembers();
}

async function removeMember(model: RemoveMemberDto) {
    await organizationsService.removeMember(organizationId.value, model);
    await updateMembers();
}
</script>