<template>
    <div class="h-full">
        <p class="text-lg">Analytics</p>
        <div class="grid grid-cols-6 gap-4 w-full mt-4" v-if="totalStatuses && workflow">
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
        </div>
    </div>
</template>

<script setup lang="ts">
import { use } from 'echarts/core';
import { CanvasRenderer } from 'echarts/renderers';
import { PieChart } from 'echarts/charts';
import {
  TitleComponent,
  TooltipComponent,
  LegendComponent,
} from 'echarts/components';

use([
  CanvasRenderer,
  PieChart,
  TitleComponent,
  TooltipComponent,
  LegendComponent,
]);

const route = useRoute();
const analyticsService = useAnalyticsService();
const workflowsService = useWorkflowsService();

const projectId = ref(route.params.id as string);

const workflow = ref(await workflowsService.getWorkflow(projectId.value));

const totalStatuses = ref(await analyticsService.getTotalTaskStatuses(projectId.value));

const option = ref();

initCharts();

function initCharts() {
    if(!workflow.value || !totalStatuses.value) {
        console.log('XD');
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
            name: 'Traffic Sources',
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
}

function getStatusName(id: string) {
    const status = workflow.value!.statuses.find(x => x.id === id);
    return status?.name ?? id;
}
</script>