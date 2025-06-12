<template>
    <div class="h-full" v-if="taskAnalytics && workflow">
        <p class="text-lg">Analytics</p>
        <template v-if="hasData">
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
            <div class="w-full bg-white shadow-md p-3 mt-4">
                Assignee
            </div>
            <div class="grid grid-cols-6 gap-2 w-full mt-2">
                <div class="col-span-2 bg-white w-full shadow-md h-96 p-4">
                    <PropertyCountChart ref="assigneeCountChart" />
                </div>
                <div class="col-span-2 bg-white w-full shadow-md h-96 p-4">
                    <PropertyCountByDayChart ref="assigneeCountByDayChart" />
                </div>
                <div class="col-span-2 bg-white w-full shadow-md h-96 p-4">
                    <PropertyCountByDayChart ref="cumulativeAssigneeCountByDayChart" />
                </div>
            </div>
        </template>
        <template v-else>
            <p class="text-sm mt-4">No data yet.</p>
        </template>
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
const projectsService = useProjectsService();
const usersPresentationData = useUsersPresentationData();

const projectId = ref(route.params.id as string);

const workflow = ref(await workflowsService.getWorkflow(projectId.value));
const taskAnalytics = ref(await analyticsService.getTaskAnalytics(projectId.value));
const members = ref(await projectsService.getMembers(projectId.value));
const presentationData = ref(await usersPresentationData.getUsers());

const statusCountChart = ref();
const priorityCountChart = ref();
const assigneeCountChart = ref();

const statusCountByDayChart = ref();
const priorityCountByDayChart = ref();
const assigneeCountByDayChart = ref();

const cumulativeStatusCountByDayChart = ref();
const cumulativePriorityCountByDayChart = ref();
const cumulativeAssigneeCountByDayChart = ref();

onMounted(() => {
    initCharts();
})

const hasData = computed(() => {
    return workflow.value && taskAnalytics.value && taskAnalytics.value.dates.length > 0;
})

const membersList = computed(() => {
    if(!members.value) {
        return [];
    }

    const unassignedMember = {
        userId: '00000000-0000-0000-0000-000000000000',
        name: 'Unassigned',
    };

    return [ unassignedMember, ... members.value.members ];
})

function initCharts() {
    if(!workflow.value || !taskAnalytics.value || !members.value || !hasData.value) {  
        return;
    }

    const statusesNames = workflow.value.statuses.map(x => x.name);
    const dates = taskAnalytics.value.dates.map(x => new Date(x).toLocaleDateString());
    const prioritiesNames = allTaskPriorities.map(x => x.name);
    const membersNames = membersList.value.map(x => x.name);

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


    const assigneeCountData = [];
    console.log(membersList.value);
    for(const member of membersList.value) {
        const count = taskAnalytics.value.countByAssigneeId[member.userId] ?? 0;
        assigneeCountData.push({
            value: count,
            name: member.name
        });
    }

    assigneeCountChart.value.initChart(
        membersList.value.map(x => getAssigneeColor(x.userId)),
        membersNames,
        assigneeCountData
    );

    //const colors = [ '#22c55e', '#6366f1', '#f43f5e' ];
    //const colors2 = [ '#0ea5e9', '#6366f1', '#f43f5e' ];
    //const colors = [ '#f43f5e', '#a855f7', '#0ea5e9' ];

    initStatusCountByDayChart(statusCountByDayChart.value, statusesNames, dates, false);
    initStatusCountByDayChart(cumulativeStatusCountByDayChart.value, statusesNames, dates, true);

    initPriorityCountByDayChart(priorityCountByDayChart.value, prioritiesNames, dates, false);
    initPriorityCountByDayChart(cumulativePriorityCountByDayChart.value, prioritiesNames, dates, true);

    initAssigneeCountByDayChart(assigneeCountByDayChart.value, membersNames, dates, false);
    initAssigneeCountByDayChart(cumulativeAssigneeCountByDayChart.value, membersNames, dates, true);
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

function initAssigneeCountByDayChart(chart: any, assigneesNames: string[], dates: string[], cumulative: boolean) {
    const data = [];
    for(const member of membersList.value) {
        const counts = taskAnalytics.value!.dailyCountByAssigneeId[member.userId] ?? [];
        data.push({
            name: member.name,
            values: counts
        });
    }

    chart.initChart(
        membersList.value.map(x => getAssigneeColor(x.userId)),
        assigneesNames,
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

function getAssigneeColor(assigneeId: string) {
    return presentationData.value.find(x => x.userId == assigneeId)?.avatarColor ?? '#94a3b8';
}
</script>