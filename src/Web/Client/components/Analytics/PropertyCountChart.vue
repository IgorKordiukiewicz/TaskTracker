<template>
    <v-chart class="h-full w-full flex justify-center" :option="chartConfig" />
</template>

<script setup lang="ts">
defineExpose({ initChart });

const chartConfig = ref();

function initChart(colorData: string[], legendData: string[], seriesData: { value: string, name: string }[]) { 
    chartConfig.value = {
        color: colorData,
        title: {
            text: 'Count',
            left: 'center',
            textStyle: {
                fontWeight: 'normal',
                fontFamily: 'Inter',
                color: '#334155',
                fontSize: 16
            }
        },
        tooltip: {
            trigger: 'item',
            formatter: '{a} <br/>{b} : {c} ({d}%)',
        },
        legend: {
            orient: 'horizontal',
            top: '30',
            data: legendData,
        },
        series: [
            {
                name: 'Priority count',
                type: 'pie',
                radius: '70%',
                center: ['50%', '60%'],
                data: seriesData,
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
</script>