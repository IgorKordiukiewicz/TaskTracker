<template>
    <div class="text-center mb-5">
        <p class="text-900 text-3xl font-medium mb-3">{{ title }}</p>
        <slot />
    </div>
</template>

<script setup lang="ts">
defineProps({
    title: { type: String, required: true }
})
</script>