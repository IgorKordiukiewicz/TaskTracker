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

const props = defineProps(['sourceX', 'sourceY', 'targetX', 'targetY', 'sourcePosition', 'targetPosition', 'source', 'target', 'data' ]);
const bidirectional = ref(props.data.bidirectional);
//

const fixedTargetX = ref(props.targetX);
const fixedTargetY = ref(props.targetY);
const fixedSourceX = ref(props.sourceX);
const fixedSourceY = ref(props.sourceY);

const { getNodes, onNodesChange, onNodesInitialized } = useVueFlow();

onNodesInitialized((_) => {
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
    const nearestTarget = pointOnRect(targetNode.position.x, targetNode.position.y, nodeSize.x, nodeSize.y, 
        sourceNode.position.x + nodeSize.x / 2, sourceNode.position.y  + nodeSize.y / 2);
    const nearestSource = pointOnRect(sourceNode.position.x, sourceNode.position.y, nodeSize.x, nodeSize.y, 
        targetNode.position.x + nodeSize.x / 2, targetNode.position.y  + nodeSize.y / 2);

    fixedTargetX.value = nearestTarget[0];
    fixedTargetY.value = nearestTarget[1];
    fixedSourceX.value = nearestSource[0];
    fixedSourceY.value = nearestSource[1];
}

function pointOnRect(left: number, top: number, width: number, height: number, x: number, y: number) { // for target - get source coords
    const right = left + width;
    const bottom = top + height;
    const midX = (left + right) / 2;
    const midY = (top + bottom) / 2;

    const m = (midY - y) / (midX - x);

    if (x <= midX) { // left
		var minXy = m * (left - x) + y;
		if (top <= minXy && minXy <= bottom)
			return [left,minXy];
	}

	if (x >= midX) { // right
		var maxXy = m * (right - x) + y;
		if (top <= maxXy && maxXy <= bottom)
			return [right, maxXy];
	}

	if (y <= midY) { // top
		var minYx = (top - y) / m + x;
		if (left <= minYx && minYx <= right)
			return [minYx, top];
	}

	if (y >= midY) { // bottom
		var maxYx = (bottom - y) / m + x;
		if (left <= maxYx && maxYx <= right)
			return [maxYx, bottom];
	}

	// edge case when finding midpoint intersection: m = 0/0 = NaN
	if (x === midX && y === midY) return [x, y];

    return [,];
}
</script>