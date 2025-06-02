<template>
    <div class="h-full">
        <p class="text-lg">Analytics</p>
        <div v-if="totalStatuses && workflow">
            <div v-for="(count, id) in totalStatuses.countByStatusId" :key="id">
                {{ getStatusName(id) }}: {{ count }}
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { useAnalyticsService } from '~/composables/analyticsService';

const route = useRoute();
const analyticsService = useAnalyticsService();
const workflowsService = useWorkflowsService();

const projectId = ref(route.params.id as string);

const workflow = ref(await workflowsService.getWorkflow(projectId.value));

const totalStatuses = ref(await analyticsService.getTotalTaskStatuses(projectId.value));

function getStatusName(id: string) {
    const status = workflow.value!.statuses.find(x => x.id === id);
    return status?.name ?? id;
}
</script>