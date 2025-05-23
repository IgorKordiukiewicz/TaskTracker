<template>
    <template v-if="!isCurrentOwner">
        <Select :options="availableRoles" option-label="name" :model-value="getRoleValue(props.member)"
        @change="async (e) => await updateMemberRole(e, props.member)"  class="w-48" />
    </template>
    <template v-else>
        <InputText readonly :value="member.roleName" class="w-48" />
    </template>
</template>

<script setup lang="ts">
import type { SelectChangeEvent } from 'primevue/select';
import type { PropType } from 'vue';
import { UpdateMemberRoleDto } from '~/types/dtos/projects';
import type { ProjectMemberVM, RoleVM } from '~/types/viewModels/projects';

const props = defineProps({
    roles: { type: Object as PropType<RoleVM[]>, required: true },
    member: { type: Object as PropType<ProjectMemberVM>, required: true }
});

const emit = defineEmits([ 'onUpdate' ]);

const availableRoles = computed(() => {
    return props.roles.filter(x => !x.owner).map(x => ({ 
        id: x.id,
        name: x.name
    }));
})

const isCurrentOwner = computed(() => {
    return props.roles.find(x => x.id === props.member.roleId)!.owner;
})

function getRoleValue(member: ProjectMemberVM) {
    return {
        id: member.roleId,
        name: member.roleName
    };
}

async function updateMemberRole(event: SelectChangeEvent, member: ProjectMemberVM) {
    const model = new UpdateMemberRoleDto();
    model.memberId = member.id;
    model.roleId = event.value.id;
    emit('onUpdate', model);
}
</script>