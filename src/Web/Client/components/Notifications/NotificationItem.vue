<template>
    <div class="flex items-center justify-between">
        <div class="flex flex-col">
            <div class="flex items-center gap-2">
                <p class="font-semibold">
                    {{ notification.contextEntityName }}
                </p>
                <p class="text-sm">
                    {{ timeElapsed }}
                </p>
            </div>
            <p>
                {{ notification.message }}
            </p>
        </div>
        <div class="flex gap-2 items-center">
            <Button icon="pi pi-external-link" rounded text severity="secondary" @click="openGotoLink" v-if="gotoLink" />
            <Button icon="pi pi-check" rounded text severity="primary" />
        </div>
    </div>
</template>

<script setup lang="ts">
import type { PropType } from 'vue';
import type { NotificationVM } from '~/types/viewModels/notifications';

const emit = defineEmits([ 'onLinkClicked' ])

const props = defineProps({
    notification: { type: Object as PropType<NotificationVM>, required: true } 
})

const timeParser = useTimeParser();

const timeElapsed = computed(() => {
    return timeParser.toReadableTimeDifference(props.notification.occurredAt);
})

const gotoLink = computed(() => {
    if(props.notification.taskShortId) {
        return `/project/${props.notification.contextEntityId}/tasks/${props.notification.taskShortId}`;
    }

    return '';
})

function openGotoLink() {
    emit('onLinkClicked');
    navigateTo(gotoLink.value);
}
</script>