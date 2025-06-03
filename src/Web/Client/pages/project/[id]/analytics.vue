<template>
    <div class="h-full">
        <p class="text-lg">Analytics</p>
        <div class="grid grid-cols-6 gap-4 w-full mt-4" v-if="totalStatuses && workflow && totalStatusesByDay && totalPriorities && totalPrioritiesByDay">
            <div class="col-span-2 bg-white w-full shadow-md h-96 p-4">
                <StatusCountChart :workflow="workflow" :total-statuses="totalStatuses" ref="totalStatusesChart" />
            </div>
            <div class="col-span-2 bg-white w-full shadow-md h-96 p-4">
                <PriorityCountChart :workflow="workflow" :total-priorities="totalPriorities" ref="totalPrioritiesChart" />
            </div>
            <div class="col-span-2 bg-white w-full shadow-md h-96 p-4">
                Count by Assignee
                <!-- TODO: Assignee colors? -->
            </div>
            <div class="col-span-2 bg-white w-full shadow-md h-96 p-4">
                <StatusCumulativeFlowChart :workflow="workflow" :total-statuses-by-day="totalStatusesByDay" ref="statusCumulativeFlowChart" />
            </div>
            <div class="col-span-2 bg-white w-full shadow-md h-96 p-4">
                <PriorityCumulativeFlowChart :workflow="workflow" :total-priorities-by-day="totalPrioritiesByDay" ref="priorityCumulativeFlowChart" />
            </div>
            <div class="col-span-2 bg-white w-full shadow-md h-96 p-4">
                Assignee Cumulative Flow
                <!-- TODO: Assignee colors? -->
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { use } from 'echarts/core';
import { CanvasRenderer } from 'echarts/renderers';
import { PieChart, LineChart } from 'echarts/charts';
import {
  TitleComponent,
  TooltipComponent,
  LegendComponent,
  GridComponent,
} from 'echarts/components';

use([
  CanvasRenderer,
  PieChart,
  TitleComponent,
  TooltipComponent,
  LegendComponent,
  LineChart,
  GridComponent
]);

const route = useRoute();
const analyticsService = useAnalyticsService();
const workflowsService = useWorkflowsService();

const projectId = ref(route.params.id as string);

const workflow = ref(await workflowsService.getWorkflow(projectId.value));

const totalStatuses = ref(await analyticsService.getTotalTaskStatuses(projectId.value));
const totalStatusesByDay = ref(await analyticsService.getTotalTaskStatusesByDay(projectId.value));
const totalPriorities = ref(await analyticsService.getTotalTaskPriorities(projectId.value));
const totalPrioritiesByDay = ref(await analyticsService.getTotalTaskPrioritiesByDay(projectId.value));

const totalStatusesChart = ref();
const statusCumulativeFlowChart = ref();
const totalPrioritiesChart = ref();
const priorityCumulativeFlowChart = ref();

onMounted(() => {
    initCharts();
})

function initCharts() {
    if(!totalStatusesChart.value || !statusCumulativeFlowChart.value || !totalPrioritiesChart.value || !priorityCumulativeFlowChart.value) {  
        return;
    }

    totalStatusesChart.value.initChart();
    statusCumulativeFlowChart.value.initChart();
    totalPrioritiesChart.value.initChart();
    priorityCumulativeFlowChart.value.initChart();
}
</script>