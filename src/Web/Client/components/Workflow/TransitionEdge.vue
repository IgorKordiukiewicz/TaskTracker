<template>
    <StraightEdge :style="{ strokeWidth: '2px' }"
      :source-x="fixedSourceX"
      :source-y="fixedSourceY"
      :target-x="fixedTargetX"
      :target-y="fixedTargetY"
      :source-position="sourcePosition"
      :target-position="targetPosition"
    />
</template>

<script setup lang="ts">
import { StraightEdge, useVueFlow, type NodePositionChange } from '@vue-flow/core';

defineExpose({ updateEdgePositions });

const props = defineProps(['id', 'sourceX', 'sourceY', 'targetX', 'targetY', 'sourcePosition', 'targetPosition', 'source', 'target', 'data' ]);
const bidirectional = ref(props.data.bidirectional);

const fixedTargetX = ref(props.targetX);
const fixedTargetY = ref(props.targetY);
const fixedSourceX = ref(props.sourceX);
const fixedSourceY = ref(props.sourceY);

const { getNodes, onNodesChange, onNodesInitialized } = useVueFlow();
const geometry = useGeometry();

onNodesInitialized((_) => {
    updateEdgePositions();
})

onMounted(() => {
    updateEdgePositions();
})

onNodesChange((changes) => {
  for(const change of changes.filter(x => x.type === 'position')) {
    const positionChange = change as NodePositionChange;
    if(!positionChange.position || !(positionChange.id === props.target || positionChange.id === props.source)) {
      continue;
    }

    updateEdgePositions();
  }
})

function updateEdgePositions() {
    const sourceNode = getNodes.value.find(x => x.id === props.source);
    if(!sourceNode) {
        return;
    }
    const targetNode = getNodes.value.find(x => x.id == props.target);
    if(!targetNode) {
        return;
    }

    // TODO: dont use hardcoded values?
    const nodeSize = { x: 160, y: 52 };
    const nearestTarget = geometry.getIntersectionPointOnRectangle(targetNode.position.x, targetNode.position.y, nodeSize.x, nodeSize.y, 
        sourceNode.position.x + nodeSize.x / 2, sourceNode.position.y  + nodeSize.y / 2);
    const nearestSource = geometry.getIntersectionPointOnRectangle(sourceNode.position.x, sourceNode.position.y, nodeSize.x, nodeSize.y, 
        targetNode.position.x + nodeSize.x / 2, targetNode.position.y  + nodeSize.y / 2);

    fixedTargetX.value = nearestTarget[0];
    fixedTargetY.value = nearestTarget[1];
    fixedSourceX.value = nearestSource[0];
    fixedSourceY.value = nearestSource[1];
}
</script>