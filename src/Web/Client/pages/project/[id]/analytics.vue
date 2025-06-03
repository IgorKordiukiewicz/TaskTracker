<template>
    <div class="h-full">
        <p class="text-lg">Analytics</p>
        <div class="grid grid-cols-6 gap-4 w-full mt-4" v-if="totalStatuses && workflow && totalStatusesByDay">
            <div class="col-span-2 bg-white w-full shadow-md h-96 p-4">
                <v-chart class="h-full w-full flex justify-center" :option="option" />
                <!-- TODO: What colors? -->
            </div>
            <div class="col-span-2 bg-white w-full shadow-md h-96 p-4">
                Count by Priority
                <!-- Priority colors -->
            </div>
            <div class="col-span-2 bg-white w-full shadow-md h-96 p-4">
                Count by Assignee
                <!-- TODO: Assignee colors? -->
            </div>
            <div class="col-span-2 bg-white w-full shadow-md h-96 p-4">
                <v-chart class="h-full w-full flex justify-center" :option="cumulativeFlowConfig" />
                <!-- TODO: What colors? -->
            </div>
            <div class="col-span-2 bg-white w-full shadow-md h-96 p-4">
                Priority Cumulative Flow
                <!-- Priority colors -->
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

const option = ref();
const cumulativeFlowConfig = ref();

initCharts();

// TODO: separate each chart into component
function initCharts() {
    if(!workflow.value || !totalStatuses.value || !totalStatusesByDay.value) {
        return;
    }

    const data = Object.entries(totalStatuses.value.countByStatusId).map(
        ([id, count]) => ({
            value: count,
            name: getStatusName(id)
        })
    )

    option.value = {
        title: {
          text: 'Count by Status',
          left: 'center',
        },
        tooltip: {
          trigger: 'item',
          formatter: '{a} <br/>{b} : {c} ({d}%)',
        },
        legend: {
          orient: 'horizontal',
          top: '30',
          data: workflow.value.statuses.map(x => x.name),
        },
        series: [
          {
            name: 'Status counts',
            type: 'pie',
            radius: '70%',
            center: ['50%', '60%'],
            data: data,
            emphasis: {
              itemStyle: {
                shadowBlur: 10,
                shadowOffsetX: 0,
                shadowColor: 'rgba(0, 0, 0, 0.5)',
              },
            },
          },
        ],
    }

    const dates = totalStatusesByDay.value.dates;
    const cumulativeFlowData = [];
    for(const [id, counts] of Object.entries(totalStatusesByDay.value.countsByStatusId)) {
        cumulativeFlowData.push({
            name: getStatusName(id),
            type: 'line',
            stack: 'total',
            areaStyle: {},
            emphasis: {
                    focus: 'series'
                },
            data: counts
        })
    }

    cumulativeFlowConfig.value = {
        title: {
            text: 'Status Cumulative Flow',
            left: 'center',
        },
        tooltip: {
            trigger: 'axis'
        },
        legend: {
            orient: 'horizontal',
            top: '30',
            data: workflow.value.statuses.map(x => x.name)
        },
        xAxis: [
            {
                type: 'category',
                boundaryGap: false,
                data: dates
            }
        ],
        yAxis: [
            {
                type: 'value'
            }
        ],
        series: cumulativeFlowData
    }
}

function getStatusName(id: string) {
    const status = workflow.value!.statuses.find(x => x.id === id);
    return status?.name ?? id;
}
</script>