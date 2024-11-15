<template>
  <div class="w-full h-full" v-if="workflow">
    <div class="flex justify-between items-center">
      <p class="text-lg">Workflow</p>
      <Button icon="pi pi-chevron-down" severity="primary" label="Actions" @click="toggleCreateMenu" aria-haspopup="true" aria-controls="overlay_menu" icon-pos="right" />
      <Menu ref="createMenu" :model="createMenuItems" :popup="true" />
      <AddWorkflowStatusDialog ref="addWorkflowStatusDialog" :workflow-id="workflow.id" :project-id="projectId" @on-add="updateWorkflow" />
      <AddWorkflowTransitionDialog ref="addWorkflowTransitionDialog" :workflow="workflow" :project-id="projectId" @on-add="updateWorkflow" />
    </div>
    <div class="w-full h-full mt-4 shadow bg-white" style="height: calc(100% - 1rem - 28px);"> <!-- 150 is temp-->
        <VueFlow
      v-model:nodes="nodes"
      v-model:edges="edges"
      fit-view-on-init
      :default-zoom="1.5"
      :min-zoom="1.0"
      :max-zoom="1.5" :connection-mode="ConnectionMode.Loose">
        <Background variant="lines" pattern-color="" />
        <template #node-status="props">
          <StatusNode :id="props.id" :data="props.data" />
        </template>
        <template #edge-transition="props">
          <TransitionEdge v-bind="props" />
        </template>
      </VueFlow>
    </div>
  </div>
</template>

<script setup lang="ts">
import { type Node, type Edge, MarkerType, Panel, Position, ConnectionMode, type NodePositionChange } from '@vue-flow/core';
import { VueFlow } from '@vue-flow/core'
import { Background } from '@vue-flow/background';
import type { WorkflowTransitionVM } from '~/types/viewModels/workflows';

const route = useRoute();
const workflowsService = useWorkflowsService();

const projectId = ref(route.params.id as string);

const workflow = ref(await workflowsService.getWorkflow(projectId.value));

const createMenu = ref();
const addWorkflowStatusDialog = ref();
const addWorkflowTransitionDialog = ref();
const nodes = ref<Node[]>();
// TODO: NodeToolbar in custom node on click?

const edges = ref<Edge[]>();

const createMenuItems = ref([
  {
    label: 'Add Status',
    icon: "pi pi-plus",
    command: () => {
      addWorkflowStatusDialog.value.show();
    }
  },
  {
    label: 'Add Transition',
    icon: "pi pi-plus",
    command: () => {
      addWorkflowTransitionDialog.value.show(); // TODO: disable button if no possible from transitions are available
    }
  },
])

initializeDiagram();

function toggleCreateMenu(event: Event) {
  createMenu.value.toggle(event);
}

function initializeDiagram() {
  if(!workflow.value) {
    return;
  }

  // Nodes
  nodes.value = [];
  for(const status of workflow.value.statuses) {
    nodes.value.push({ id: status.id, type: 'status', data: { label: status.name }, position: { x: 0, y: 0 } });
    // TODO: set position, set Initial flag
  }

  // Edges
  edges.value = [];
  const edgesCreatedByTransitionId = new Map(workflow.value.transitions.map(x => [getTransitionKey(x), false]));
  for(const transition of workflow.value.transitions) {
    console.log(edgesCreatedByTransitionId.get(getTransitionKey(transition)));
    if(edgesCreatedByTransitionId.get(getTransitionKey(transition))) {
      continue;
    }

    const isBidirectional = edgesCreatedByTransitionId.get(getReverseTransitionKey(transition));

    edges.value.push({ id: getTransitionKey(transition), source: transition.fromStatusId, target: transition.toStatusId, 
      type: 'transition', data: { bidirectional: isBidirectional },
      markerEnd: MarkerType.ArrowClosed, 
      markerStart: isBidirectional ? MarkerType.ArrowClosed : undefined });
    
    edgesCreatedByTransitionId.set(getTransitionKey(transition), true);
    if(isBidirectional) {
      edgesCreatedByTransitionId.set(getReverseTransitionKey(transition), true);
    }
  }
}

function getTransitionKey(transition: WorkflowTransitionVM) {
  return `${transition.fromStatusId}${transition.toStatusId}`;
}

function getReverseTransitionKey(transition: WorkflowTransitionVM) {
  return `${transition.toStatusId}${transition.fromStatusId}`;
}

async function updateWorkflow() {
  workflow.value = await workflowsService.getWorkflow(projectId.value);
  initializeDiagram();
}

</script>