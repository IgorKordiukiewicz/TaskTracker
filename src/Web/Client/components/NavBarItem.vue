<template>
    <li>
        <a class="flex items-center cursor-pointer p-3 hover:bg-surface-100 rounded gap-3" :class="{ 'bg-surface-200': selected }" :href="props.link">
            <i :class="props.icon" style="height: 16px; color: var(--p-menu-item-icon-color)"></i>
            <span>
                {{ label }}
            </span>
        </a>
    </li>
</template>

<script setup lang="ts">
const props = defineProps({
    label: { type: String, required: true },
    link: { type: String, required: true },
    icon: { type: String, required: true },
    includeIndex: { type: Boolean }
})

const route = useRoute()

const selected = computed(() => {
    return route.path === props.link || (props.includeIndex && route.path === '/');
})
</script>

<style scoped>
.selected {
    color: var(--p-primary-color);
}
</style>