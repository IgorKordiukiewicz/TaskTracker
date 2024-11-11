<template>
    <div class="flex flex-col gap-1">
        <label>{{ props.label }}</label>
        <slot />
    </div>
</template>

<script setup lang="ts">
const props = defineProps({
    label: { type: String, required: true }
})
</script>