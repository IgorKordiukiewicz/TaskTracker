<template>
    <Avatar :label="avatarLabel" shape="circle" :style="{background: avatarBackground }" style="color: #ffffff" />
</template>

<script setup lang="ts">
import { useUsersPresentationData } from '~/composables/usersPresentationData';

const props = defineProps({
    userId: { type: String, required: true }
})

const usersPresentationData = useUsersPresentationData();

const data = ref(await usersPresentationData.getUser(props.userId));

const avatarLabel = computed(() => {
    if(!data.value) {
        return '';
    }

    return `${data.value.firstName[0].toUpperCase()}${data.value.lastName[0].toUpperCase()}`;
})

const avatarBackground = computed(() => {
    if(!data.value) {
        return '#000000';
    }

    return data.value.avatarColor;
})
</script>