<template>
    <div>
        <MembersList v-if="members && roles" :members="members.members" :roles="roles.roles" :can-edit-members="canEditMembers"
        @on-update-member-role="updateMemberRole" @on-remove-member="removeMember">
        </MembersList>
    </div>
</template>

<script setup lang="ts">
import { RemoveMemberDto, UpdateMemberRoleDto } from '~/types/dtos/shared';
import { OrganizationPermissions } from '~/types/enums';

const route = useRoute();
const organizationsService = useOrganizationsService();
const permissions = usePermissions();

const organizationId = ref(route.params.id as string);

await permissions.checkOrganizationPermissions(organizationId.value);

const canEditMembers = computed(() => {
    return permissions.hasPermission(OrganizationPermissions.EditMembers);
})

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