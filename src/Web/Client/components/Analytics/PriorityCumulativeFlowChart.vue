<template>
    <v-chart class="h-full w-full flex justify-center" :option="chartConfig" />
</template>

<script setup lang="ts">
import { allTaskPriorities } from '~/types/enums';
import type { TotalTaskPrioritiesByDayVM } from '~/types/viewModels/analytics';
import type { WorkflowVM } from '~/types/viewModels/workflows';


defineExpose({ initChart });
const props = defineProps({
    workflow: { type: Object as PropType<WorkflowVM>, required: true },
    totalPrioritiesByDay: { type: Object as PropType<TotalTaskPrioritiesByDayVM>, required: true }
})

const chartConfig = ref();

function initChart() {
    if(!props.workflow || !props.totalPrioritiesByDay) {
        return;
    }

    const dates = props.totalPrioritiesByDay.dates;
    const cumulativeFlowData = [];
    for(const [id, counts] of Object.entries(props.totalPrioritiesByDay.countsByPriority)) {
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

    chartConfig.value = {
        title: {
            text: 'Priority Cumulative Flow',
            left: 'center',
        },
        tooltip: {
            trigger: 'axis'
        },
        legend: {
            orient: 'horizontal',
            top: '30',
            data: allTaskPriorities.map(x => x.name)
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
    const status = props.workflow.statuses.find(x => x.id === id);
    return status?.name ?? id;
}
</script>