<template>
  <div class="w-full h-full" v-if="workflow">
    <div class="flex justify-between items-center">
      <p class="text-lg">Workflow</p>
      <Button icon="pi pi-chevron-down" severity="primary" label="Actions" @click="toggleCreateMenu" aria-haspopup="true" aria-controls="overlay_menu" icon-pos="right" />
      <Menu ref="createMenu" :model="createMenuItems" :popup="true" />
      <AddWorkflowStatusDialog ref="addWorkflowStatusDialog" :workflow-id="workflow.id" :project-id="projectId" @on-add="(name) => updateDiagram({ newStatusName: name })" />
      <AddWorkflowTransitionDialog ref="addWorkflowTransitionDialog" :workflow="workflow" :project-id="projectId" @on-add="(value) => updateDiagram({ newTransition: value})" />
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
          <TransitionEdge v-bind="props" ref="transitionEdges" />
        </template>
      </VueFlow>
    </div>
  </div>
</template>

<script setup lang="ts">
import { type Node, type Edge, MarkerType, Panel, Position, ConnectionMode, type NodePositionChange, useVueFlow } from '@vue-flow/core';
import { VueFlow } from '@vue-flow/core'
import { Background } from '@vue-flow/background';
import type { WorkflowTransitionVM } from '~/types/viewModels/workflows';

const route = useRoute();
const workflowsService = useWorkflowsService();
const vueFlow = useVueFlow();

const projectId = ref(route.params.id as string);

const workflow = ref(await workflowsService.getWorkflow(projectId.value));

const createMenu = ref();
const addWorkflowStatusDialog = ref();
const addWorkflowTransitionDialog = ref();
const nodes = ref<Node[]>();
// TODO: NodeToolbar in custom node on click?

const edges = ref<Edge[]>();
const transitionEdges = useTemplateRef('transitionEdges');

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
  //nodes.value = [];
  const newNodes: Node[] = [];
  for(const status of workflow.value.statuses) {
    newNodes.push({ id: status.id, type: 'status', data: { label: status.name }, position: { x: 0, y: 0 } });
    // TODO: set position, set Initial flag
  }

  vueFlow.setNodes(newNodes);

  // Edges
  //edges.value = [];
  const newEdges: Edge[] = [];
  const edgesCreatedByTransitionId = new Map(workflow.value.transitions.map(x => [getTransitionKey(x), false]));
  for(const transition of workflow.value.transitions) {
    if(edgesCreatedByTransitionId.get(getTransitionKey(transition))) {
      continue;
    }

    const isBidirectional = edgesCreatedByTransitionId.has(getReverseTransitionKey(transition));

    newEdges.push({ id: getTransitionKey(transition), source: transition.fromStatusId, target: transition.toStatusId, 
      type: 'transition', data: { bidirectional: isBidirectional },
      markerEnd: MarkerType.ArrowClosed, 
      markerStart: isBidirectional ? MarkerType.ArrowClosed : undefined });
    
    edgesCreatedByTransitionId.set(getTransitionKey(transition), true);
    if(isBidirectional) {
      edgesCreatedByTransitionId.set(getReverseTransitionKey(transition), true);
    }
  }

  vueFlow.setEdges(newEdges);

  // setEdges ?
}

function getTransitionKey(transition: WorkflowTransitionVM) {
  return `${transition.fromStatusId}${transition.toStatusId}`;
}

function getReverseTransitionKey(transition: WorkflowTransitionVM) {
  return `${transition.toStatusId}${transition.fromStatusId}`;
}

async function updateDiagram(options: { 
  newStatusName?: string, 
  newTransition?: { fromId: string, toId: string } 
}) {
  workflow.value = await workflowsService.getWorkflow(projectId.value);
  if(!workflow.value) {
    return;
  }

  if(options.newStatusName) {
    const newStatus = workflow.value.statuses.find(x => x.name === options.newStatusName)!;
    vueFlow.addNodes({ id: newStatus.id, type: 'status', data: { label: newStatus.name }, position: { x: 0, y: 0 } })
  }

  if(options.newTransition) {
    const newTransitionKey = `${options.newTransition.fromId}${options.newTransition.toId}`;
    const newTransition = workflow.value.transitions
      .find(x => getTransitionKey(x) === newTransitionKey)!;
    const reverseTransitionExists = workflow.value.transitions
      .some(x => getReverseTransitionKey(x) === newTransitionKey);
      // Update existing
      if(reverseTransitionExists) {
        const existingEdge = vueFlow.findEdge(getReverseTransitionKey(newTransition))!;
        existingEdge.data.bidirectional = true;
        existingEdge.markerStart = MarkerType.ArrowClosed;
        vueFlow.updateEdgeData(existingEdge.id, existingEdge.data);
      }
      // Add new 
      else {
        vueFlow.addEdges({ id: getTransitionKey(newTransition), source: newTransition.fromStatusId, target: newTransition.toStatusId, 
          type: 'transition', data: { bidirectional: false },
          markerEnd: MarkerType.ArrowClosed });
      }
  }
}

</script>