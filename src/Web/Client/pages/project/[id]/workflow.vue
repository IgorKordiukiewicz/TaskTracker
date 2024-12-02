<template>
  <div class="w-full h-full" v-if="canViewPage && workflow">
    <div class="flex justify-between items-center">
      <p class="text-lg">Workflow</p>
      <Button icon="pi pi-chevron-down" severity="primary" label="Actions" @click="toggleCreateMenu" aria-haspopup="true" aria-controls="overlay_menu" icon-pos="right" />
      <Menu ref="createMenu" :model="createMenuItems" :popup="true" />
      <AddWorkflowStatusDialog ref="addWorkflowStatusDialog" :workflow-id="workflow.id" :project-id="projectId" @on-add="(name) => updateDiagram({ newStatusName: name })" />
      <AddWorkflowTransitionDialog ref="addWorkflowTransitionDialog" :workflow="workflow" :project-id="projectId" @on-add="(value) => updateDiagram({ newTransition: value})" />
      <ConfirmDialog></ConfirmDialog>
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
import { type Node, type Edge, MarkerType, Panel, Position, ConnectionMode, type NodePositionChange, useVueFlow, type NodeSelectionChange, type EdgeSelectionChange, type XYPosition } from '@vue-flow/core';
import { VueFlow } from '@vue-flow/core'
import { Background } from '@vue-flow/background';
import type { WorkflowTransitionVM } from '~/types/viewModels/workflows';
import { ChangeInitialWorkflowStatusDto, DeleteWorkflowStatusDto, DeleteWorkflowTransitionDto } from '~/types/dtos/workflows';
import { ProjectPermissions, TaskStatusDeletionEligibility } from '~/types/enums';

const route = useRoute();
const workflowsService = useWorkflowsService();
const vueFlow = useVueFlow();
const { onNodesChange, onEdgesChange } = useVueFlow();
const confirm = useConfirm();
const permissions = usePermissions();

const projectId = ref(route.params.id as string);
const workflow = ref(await workflowsService.getWorkflow(projectId.value));

await permissions.checkProjectPermissions(projectId.value);

const createMenu = ref();
const addWorkflowStatusDialog = ref();
const addWorkflowTransitionDialog = ref();
const nodes = ref<Node[]>();

const edges = ref<Edge[]>();

const selectedNode = ref();
const selectedEdge = ref();

const createMenuItems = ref([
  {
    label: 'Add Status',
    icon: "pi pi-plus",
    command: () => {
      addWorkflowStatusDialog.value.show();
    },
  },
  {
    label: 'Add Transition',
    icon: "pi pi-plus",
    command: () => {
      addWorkflowTransitionDialog.value.show(); // TODO: disable button if no possible from transitions are available
    }
  },
  {
    separator: true,
    visible: isNodeOrEdgeSelected
  },
  {
    label: `Selected Status`,
    visible: isNodeSelected,
    items: [
      {
        label: 'Make Initial',
        icon: 'pi pi-asterisk',
        disabled: isSelectedNodeInitial,
        command: () => {
          const statusName = selectedNode.value.data.label;
          confirm.require({
              message: `Are you sure you want to make the ${statusName} status initial?`,
              header: 'Confirm action',
              rejectProps: {
                  label: 'Cancel',
                  severity: 'secondary'
              },
              acceptProps: {
                  label: 'Confirm',
                  severity: 'primary'
              },
              accept: async () => await changeInitialStatus()
          })
        }
      },
      {
        label: 'Delete',
        icon: 'pi pi-trash',
        command: onDeleteStatusButtonClicked
      }
    ]
  },
  {
    label: `Selected Transition`,
    visible: isEdgeSelected,
    items: [
      {
        label: getSelectedTransitionName,
        icon: 'pi pi-trash',
        command: async () => {
          await confirmDeleteTransition();
        }
      },
      {
        label: getSelectedReverseTransitionName,
        icon: 'pi pi-trash',
        visible: isEdgeSelectedBidirectional,
        command: async () => {
          await confirmDeleteTransition(true);
        }
      }
    ]
  }
])

const canViewPage = computed(() => {
  return permissions.hasPermission(ProjectPermissions.EditProject);
})

initializeDiagram();

onNodesChange((changes) => {
  for(const change of changes.filter(x => x.type === 'select' && x.selected)) {
    selectedEdge.value = null;
    selectedNode.value = vueFlow.findNode((change as NodeSelectionChange).id);
  }

  // Save positions
  if(changes.some(x => x.type === 'position' || x.type === 'add' || x.type === 'remove')) {
    const nodesPositions = vueFlow.getNodes.value.map(x => ({id: x.id, position: x.position, }));
    localStorage.setItem(getLocalStorageKey(), JSON.stringify(nodesPositions));
  }
})

onEdgesChange((changes) => {
  for(const change of changes.filter(x => x.type === 'select' && x.selected)) {
    selectedNode.value = null;
    selectedEdge.value = vueFlow.findEdge((change as EdgeSelectionChange).id);
  }
})

function isNodeSelected() {
  return selectedNode.value;
}

function isEdgeSelected() {
  return selectedEdge.value;
}

function isNodeOrEdgeSelected() {
  return isNodeSelected() || isEdgeSelected();
}

function isSelectedNodeInitial() {
  return isNodeSelected() && selectedNode.value.data.initial;
}

function isEdgeSelectedBidirectional() {
  return selectedEdge.value && selectedEdge.value.data.bidirectional;
}

function getSelectedTransitionName() {
  const nodes = getSelectedEdgeNodeNames();
  return `Delete (${nodes.source} -> ${nodes.target})`;
}

function getSelectedReverseTransitionName() {
  const nodes = getSelectedEdgeNodeNames();
  return `Delete (${nodes.target} -> ${nodes.source})`;
}

function getSelectedEdgeNodeNames() {
  const sourceNode = vueFlow.findNode(selectedEdge.value.source)?.data.label;
  const targetNode = vueFlow.findNode(selectedEdge.value.target)?.data.label;
  return { source: sourceNode, target: targetNode };
}

function getLocalStorageKey() {
  return `workflow-${workflow.value?.id}`;
}

function toggleCreateMenu(event: Event) {
  createMenu.value.toggle(event);
}

function initializeDiagram() {
  if(!workflow.value) {
    return;
  }

  const nodePositions: { id: string, position: XYPosition }[] = JSON.parse(localStorage.getItem(getLocalStorageKey()) ?? '[]');
  const positionByNodeId = new Map(nodePositions.map(x => [x.id as string, x.position]));

  // Nodes
  const newNodes: Node[] = [];
  let yPosition = 0;
  for(const status of workflow.value.statuses) {
    const position = positionByNodeId.has(status.id) ? positionByNodeId.get(status.id)! : { x: 0, y: yPosition };
    yPosition += 80;

    newNodes.push({ id: status.id, type: 'status', data: { label: status.name, initial: status.initial }, position: position });
  }

  vueFlow.setNodes(newNodes);

  // Edges
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
}

function getTransitionKey(transition: WorkflowTransitionVM) {
  return `${transition.fromStatusId}${transition.toStatusId}`;
}

function getReverseTransitionKey(transition: WorkflowTransitionVM) {
  return `${transition.toStatusId}${transition.fromStatusId}`;
}

async function updateDiagram(options: { 
  newStatusName?: string, 
  newTransition?: { fromId: string, toId: string },
  deletedStatusId?: string,
  deletedTransition?: { fromId: string, toId: string },
  newInitialStatusId?: string
}) {
  const oldInitialStatusId = workflow.value?.statuses.find(x => x.initial)?.id;
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

  if(options.deletedStatusId) {
    vueFlow.removeNodes(options.deletedStatusId);
  }

  if(options.deletedTransition) {
    const transitionKey = getTransitionKey({ fromStatusId: options.deletedTransition.fromId, toStatusId: options.deletedTransition.toId });
    const reverseTransitionKey = getTransitionKey({ fromStatusId: options.deletedTransition.toId, toStatusId: options.deletedTransition.fromId });
    let existingEdge = vueFlow.findEdge(transitionKey);
    let usingReverse = false;
    if(!existingEdge) {
      existingEdge = vueFlow.findEdge(reverseTransitionKey);
      usingReverse = true;
    }

    if(!existingEdge) {
      return;
    }

    if(existingEdge.data.bidirectional) {
      existingEdge.data.bidirectional = false;
      if(usingReverse) {
        existingEdge.markerStart = undefined;
      }
      else {
        existingEdge.markerEnd = undefined;
      }
      existingEdge.source = options.deletedTransition.toId;
      existingEdge.target = options.deletedTransition.fromId;
      vueFlow.updateEdgeData(existingEdge.id, existingEdge.data);
    }
    else {
      vueFlow.removeEdges(existingEdge);
    }
  }

  if(options.newInitialStatusId) {
    if(!oldInitialStatusId) {
      return;
    }

    const currentInitialNode = vueFlow.findNode(oldInitialStatusId)!;
    currentInitialNode.data.initial = false;

    const newInitialNode = vueFlow.findNode(options.newInitialStatusId)!;
    newInitialNode.data.initial = true;

    vueFlow.updateNodeData(currentInitialNode.id, currentInitialNode.data);
    vueFlow.updateNodeData(newInitialNode.id, newInitialNode.data);
  }
}

async function deleteStatus() {
  const statusId = selectedNode.value.id;
  const model = new DeleteWorkflowStatusDto();
  model.statusId = statusId;
  await workflowsService.deleteStatus(workflow.value!.id, projectId.value, model);
  await updateDiagram({ deletedStatusId: statusId });
}

async function confirmDeleteTransition(reverse: boolean = false) {
  const nodesNames = getSelectedEdgeNodeNames();
  confirm.require({
      message: `Are you sure you want to delete the ${reverse ? nodesNames.target : nodesNames.source} to ${reverse ? nodesNames.source : nodesNames.target} transition?`,
      header: 'Confirm action',
      rejectProps: {
          label: 'Cancel',
          severity: 'secondary'
      },
      acceptProps: {
          label: 'Confirm',
          severity: 'danger'
      },
      accept: async () => await deleteTransition(reverse)
  })
}

async function deleteTransition(reverse: boolean = false) {
  const model = new DeleteWorkflowTransitionDto();
  model.fromStatusId = reverse ? selectedEdge.value.target : selectedEdge.value.source;
  model.toStatusId = reverse ? selectedEdge.value.source : selectedEdge.value.target;
  await workflowsService.deleteTransition(workflow.value!.id, projectId.value, model);
  await updateDiagram({ deletedTransition: { fromId: model.fromStatusId, toId: model.toStatusId }})
}

async function changeInitialStatus() {
  const statusId = selectedNode.value.id;
  const model = new ChangeInitialWorkflowStatusDto();
  model.statusId = statusId;
  await workflowsService.changeInitialStatus(workflow.value!.id, projectId.value, model);
  await updateDiagram({ newInitialStatusId: statusId});
}

async function onDeleteStatusButtonClicked() {
  const status = workflow.value!.statuses.find(x => x.id === selectedNode.value.id)!;
  if(status.deletionEligibility != TaskStatusDeletionEligibility.Eligible) {
    const reason = status.deletionEligibility == TaskStatusDeletionEligibility.InUse ? 'in use' : 'the initial status'
    confirm.require({
      message: `Status ${status.name} can't be deleted because it is ${reason}.`,
      header: 'Information',
      rejectClass: 'invisible',
      acceptProps: {
        label: 'Ok',
        severity: 'secondary'
      }
    })
  }
  else {
    confirm.require({
      message: `Are you sure you want to delete the ${status.name} status?`,
      header: 'Confirm action',
      rejectProps: {
          label: 'Cancel',
          severity: 'secondary'
      },
      acceptProps: {
          label: 'Confirm',
          severity: 'danger'
      },
      accept: async () => await deleteStatus()
   })
  }

  
}
</script>