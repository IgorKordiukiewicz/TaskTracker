<template>
    <div class="flex items-start gap-3">
        <UserAvatar :user-id="comment.authorId" />
        <div class="flex flex-col">
            <div class="flex items-center gap-2">
                <p class="font-semibold">
                    {{ comment.authorName }}
                </p>
                <p class="text-sm">
                    {{ timeElapsed }}
                </p>
            </div>
            <p>
                {{ comment.content }}
            </p>
        </div>
    </div>
</template>

<script setup lang="ts">
import type { PropType } from 'vue';
import type { TaskCommentVM } from '~/types/viewModels/tasks';

const props = defineProps({
    comment: { type: Object as PropType<TaskCommentVM>, required: true }
})

const timeParser = useTimeParser();

const timeElapsed = computed(() => {
    return timeParser.toReadableTimeDifference(props.comment.createdAt);
})

</script>