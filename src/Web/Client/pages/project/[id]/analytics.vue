<template>
    <div class="h-full" v-if="taskAnalytics && workflow">
        <p class="text-lg">Analytics</p>
        <div class="w-full bg-white shadow-md p-3 mt-4">
            Status
        </div>
        <div class="grid grid-cols-6 gap-2 w-full mt-2">
            <div class="col-span-2 bg-white w-full shadow-md h-96 p-4">
                <PropertyCountChart ref="statusCountChart" />
            </div>
            <div class="col-span-2 bg-white w-full shadow-md h-96 p-4">
                <PropertyCountByDayChart ref="statusCountByDayChart" />
            </div>
            <div class="col-span-2 bg-white w-full shadow-md h-96 p-4">
                <PropertyCountByDayChart ref="cumulativeStatusCountByDayChart" />
            </div>
        </div>
        <div class="w-full bg-white shadow-md p-3 mt-4">
            Priority
        </div>
        <div class="grid grid-cols-6 gap-2 w-full mt-2">
            <div class="col-span-2 bg-white w-full shadow-md h-96 p-4">
                <PropertyCountChart ref="priorityCountChart" />
            </div>
            <div class="col-span-2 bg-white w-full shadow-md h-96 p-4">
                <PropertyCountByDayChart ref="priorityCountByDayChart" />
            </div>
            <div class="col-span-2 bg-white w-full shadow-md h-96 p-4">
                <PropertyCountByDayChart ref="cumulativePriorityCountByDayChart" />
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
import { allTaskPriorities, TaskPriority } from '~/types/enums';
import PropertyCountChart from '~/components/Analytics/PropertyCountChart.vue';
import PropertyCountByDayChart from '~/components/Analytics/PropertyCountByDayChart.vue';

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
const taskAnalytics = ref(await analyticsService.getTaskAnalytics(projectId.value));

const statusCountChart = ref();
const priorityCountChart = ref();
const statusCountByDayChart = ref();
const priorityCountByDayChart = ref();
const cumulativeStatusCountByDayChart = ref();
const cumulativePriorityCountByDayChart = ref();

onMounted(() => {
    initCharts();
})

function initCharts() {
    if(!workflow.value || !taskAnalytics.value) {  
        return;
    }

    //const colors = [ '#22c55e', '#6366f1', '#f43f5e' ];
    //const colors = [ '#22c55e', '#06b6d4', '#6366f1' ];

    const statusesNames = workflow.value.statuses.map(x => x.name);
    const dates = taskAnalytics.value.dates.map(x => new Date(x).toLocaleDateString());
    const prioritiesNames = allTaskPriorities.map(x => x.name);

    const statusCountData = [];
    for(const status of workflow.value.statuses) {
        const count = taskAnalytics.value.countByStatusId[status.id] ?? 0;
        statusCountData.push({
            value: count,
            name: status.name
        })
    }

    statusCountChart.value.initChart(
        [ '#57c0f0', '#c388fa', '#f8798f' ],
        statusesNames,
        statusCountData
    );


    const priorityCountData = [];
    for(const priority of allTaskPriorities) {
        const count = taskAnalytics.value.countByPriority[priority.name] ?? 0;
        priorityCountData.push({
            value: count,
            name: priority.name
        })
    }

    priorityCountChart.value.initChart(
        allTaskPriorities.map(x => getPriorityColor(x.key)),
        prioritiesNames,
        priorityCountData
    );

    //const colors = [ '#22c55e', '#6366f1', '#f43f5e' ];
    //const colors2 = [ '#0ea5e9', '#6366f1', '#f43f5e' ];
    //const colors = [ '#f43f5e', '#a855f7', '#0ea5e9' ];

    initStatusCountByDayChart(statusCountByDayChart.value, statusesNames, dates, false);
    initStatusCountByDayChart(cumulativeStatusCountByDayChart.value, statusesNames, dates, true);

    initPriorityCountByDayChart(priorityCountByDayChart.value, prioritiesNames, dates, false);
    initPriorityCountByDayChart(cumulativePriorityCountByDayChart.value, prioritiesNames, dates, true);
}

function initStatusCountByDayChart(chart: any, statusesNames: string[], dates: string[], cumulative: boolean) {
    const data = [];
    for(const status of workflow.value!.statuses) {
        const counts = taskAnalytics.value!.dailyCountByStatusId[status.id] ?? [];
        data.push({
            name: status.name,
            values: counts
        })
    };

    chart.initChart(
        [ '#0ea5e9',  '#a855f7', '#f43f5e' ],
        statusesNames,
        dates,
        data,
        cumulative
    );
}

function initPriorityCountByDayChart(chart: any, prioritiesNames: string[], dates: string[], cumulative: boolean) {
    const data = [];
    for(const priority of allTaskPriorities) {
        const counts = taskAnalytics.value!.dailyCountByPriority[priority.name] ?? [];
        data.push({
            name: priority.name,
            values: counts,
        })
    }

    chart.initChart(
        ['#22c55e', '#0ea5e9',  '#f97316', '#ef4444'],
        prioritiesNames,
        dates,
        data,
        cumulative
    );
}

function getPriorityColor(priority: TaskPriority) {
    switch (+priority) {
        case TaskPriority.Urgent: 
            return "#f47d7d";
        case TaskPriority.High:
            return "#fb9d5c";
        case TaskPriority.Normal:
            return "#57c0f0";
        case TaskPriority.Low:
            return "#65d78f";
        default: 
            return "#ffffff";
    }
}
</script>