<template>
    <div class="bg-slate-600 text-white shadow w-40 p-4 rounded-md flex" :class="{ selected: isSelected }">
        <p class="text-center text-sm w-full">{{ data.label }}</p>
        <div style="position: relative;">
            <Handle type="target" :position="Position.Bottom" style="opacity: 0; margin-left: -60px; margin-bottom: 8px;" :connectable="true" />
            <Handle type="source" :position="Position.Bottom" style="opacity: 0; margin-left: -60px; margin-bottom: 8px;" :connectable="false"/>
        </div>
    </div>
</template>

<script setup lang="ts">
import { useVueFlow, Handle, Position } from '@vue-flow/core';

const props = defineProps(['id', 'data', 'selected' ]);
const instance = useVueFlow();

const isSelected = computed(() => {
    return instance.findNode(props.id)?.selected;
})
</script>

<style scoped>
.selected {
    background: var(--p-primary-color);
}
</style>