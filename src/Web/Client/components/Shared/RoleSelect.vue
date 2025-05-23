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
import { UpdateMemberRoleDto } from '~/types/dtos/shared';
import type { MemberVM, RoleVM } from '~/types/viewModels/shared';

const props = defineProps({
    roles: { type: Object as PropType<RoleVM[]>, required: true },
    member: { type: Object as PropType<MemberVM>, required: true }
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

function getRoleValue(member: MemberVM) {
    return {
        id: member.roleId,
        name: member.roleName
    };
}

async function updateMemberRole(event: SelectChangeEvent, member: MemberVM) {
    const model = new UpdateMemberRoleDto();
    model.memberId = member.id;
    model.roleId = event.value.id;
    emit('onUpdate', model);
}
</script>