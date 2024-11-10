<template>
    <OrganizationLayout>
        <p class="text-lg">Members</p>
        <div v-if="members" class="rounded-md bg-white w-100 shadow mt-4 p-4">
            <DataTable :value="[ ...members.members, ...members.members, ...members.members, ...members.members]" v-if="members">
                <Column header="Name">
                    <template #body="slotProps">
                        <div class="flex gap-4 items-center">
                            <Avatar label="AA" shape="circle" /> <!--TODO-->
                            <p>{{ slotProps.data.name }}</p>
                        </div>
                    </template>
                </Column>
                <Column field="email" header="Email"></Column>
                <Column field="roleName" header="Role"></Column>
            </DataTable>
        </div>
    </OrganizationLayout>
</template>

<script setup lang="ts">
const route = useRoute();
const organizationsService = useOrganizationsService();

const organizationId = ref(route.params.id as string);

const members = ref(await organizationsService.getMembers(organizationId.value));
const roles = ref(await organizationsService.getRoles(organizationId.value));

const value = ref('Members');
const options = ref(['Members', 'Invitations', 'Roles']);
</script>