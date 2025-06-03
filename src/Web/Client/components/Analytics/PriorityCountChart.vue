<template>
    <v-chart class="h-full w-full flex justify-center" :option="chartConfig" />
</template>

<script setup lang="ts">
import type { PropType } from 'vue';
import { allTaskPriorities } from '~/types/enums';
import type { TotalTaskPrioritiesVM } from '~/types/viewModels/analytics';
import type { WorkflowVM } from '~/types/viewModels/workflows';

defineExpose({ initChart });
const props = defineProps({
    workflow: { type: Object as PropType<WorkflowVM>, required: true },
    totalPriorities: { type: Object as PropType<TotalTaskPrioritiesVM>, required: true }
})

const chartConfig = ref();

function initChart() {
    if(!props.workflow || !props.totalPriorities) {
        return;
    }

    const data = Object.entries(props.totalPriorities.countByPriority).map(
        ([id, count]) => ({
            value: count,
            name: getStatusName(id)
        })
    )

    chartConfig.value = {
        title: {
          text: 'Priority count',
          left: 'center',
        },
        tooltip: {
          trigger: 'item',
          formatter: '{a} <br/>{b} : {c} ({d}%)',
        },
        legend: {
          orient: 'horizontal',
          top: '30',
          data: allTaskPriorities.map(x => x.name),
        },
        series: [
          {
            name: 'Priority count',
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
    const status = props.workflow.statuses.find(x => x.id === id);
    return status?.name ?? id;
}
</script>