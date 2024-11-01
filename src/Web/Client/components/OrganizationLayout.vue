<template>
    <div>
        <slot/>
    </div>
</template>

<script setup lang="ts">
const route = useRoute();

const organizationId = ref(route.params.id as string);
</script>