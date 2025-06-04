<template>
    <v-chart class="h-full w-full flex justify-center" :option="chartConfig" />
</template>

<script setup lang="ts">
defineExpose({ initChart });

const chartConfig = ref();

function initChart(colors: string[], legend: string[], dates: string[], data: { name: string, values: number[] }[], cumulative: boolean = false) {
    chartConfig.value = {
        color: colors,
        title: {
            text: cumulative ? 'Cumulative Flow' : 'Count by Day',
            left: 'center',
            textStyle: {
                fontWeight: 'normal',
                fontFamily: 'Inter',
                color: '#334155',
                fontSize: 16
            }
        },
        tooltip: {
            trigger: 'axis'
        },
        legend: {
            orient: 'horizontal',
            top: '30',
            data: legend
        },
        grid: {
            bottom: '0%',
            containLabel: true
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
                type: 'value',
            }
        ],
        series: data.map(x => ({
            name: x.name,
            type: 'line',
            stack: cumulative ? 'total' : undefined,
            areaStyle: cumulative ? {} : undefined,
            emphasis: {
                focus: 'series'
            },
            data: x.values
        }))
    }
}
</script>