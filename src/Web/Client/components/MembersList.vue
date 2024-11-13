<template>
    <DataTable :value="members" class="mt-4 shadow">
        <Column header="Name">
            <template #body="slotProps">
                <div class="flex gap-4 items-center">
                    <Avatar label="AA" shape="circle" /> <!--TODO-->
                    <p>{{ slotProps.data.name }}</p>
                </div>
            </template>
        </Column>
        <Column field="email" header="Email"></Column>
        <Column field="roleName" header="Role">
            <template #body="slotProps">
                <RoleSelect :roles="roles" :member="slotProps.data" @on-update="updateMemberRole" />
            </template>
        </Column>
        <Column header="" style="width: 10px;">
            <template #body="slotProps">
                <Button type="button" icon="pi pi-ellipsis-v" text severity="secondary" @click="(e) => toggleMenu(e, slotProps.data.id)" />
                <Menu ref="menu" :model="menuItems" :popup="true" />
            </template>             
        </Column>
    </DataTable>
</template>

<script setup lang="ts">
import type { PropType } from 'vue';
import { RemoveMemberDto, type UpdateMemberRoleDto } from '~/types/dtos/shared';
import type { MemberVM, RoleVM } from '~/types/viewModels/shared';

const props = defineProps({
    members: { type: Object as PropType<MemberVM[]>, required: true },
    roles: { type: Object as PropType<RoleVM[]>, required: true }
})

const emit = defineEmits([
    'onUpdateMemberRole', 'onRemoveMember'
])

const menu = ref();
const menuItems = ref([
    {
        label: 'Options',
        items: [
            {
                label: 'Remove',
                icon: 'pi pi-trash',
                command: () => {
                    const model = new RemoveMemberDto();
                    model.memberId = selectedMemberId.value;
                    emit('onRemoveMember', model);
                }
            }
        ]
    }
])
const selectedMemberId = ref();

function updateMemberRole(model: UpdateMemberRoleDto) {
    emit('onUpdateMemberRole', model);
}

function toggleMenu(event: Event, memberId: string) {
    selectedMemberId.value = memberId;
    menu.value.toggle(event);
}

</script>