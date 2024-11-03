<template>
    <Dialog v-model:visible="visible" modal :header="props.header" style="min-width: 24rem;">
        <div class="flex flex-col gap-2">
            <slot />
            <div class="flex justify-end gap-2 mt-4">
                <Button type="button" label="Cancel" severity="secondary" @click="visible = false" autofocus></Button>
                <Button type="button" :label="props.submitLabel" @click="submit"></Button>
            </div>
        </div>
    </Dialog>
</template>

<script setup lang="ts">
defineExpose({ show });
const emit = defineEmits([ 'submit' ]);
const props = defineProps({
    header: { type: String, required: true },
    submitLabel: { type: String, required: true }
});

const visible = ref(false);

function show() {
    visible.value = true;
}

async function submit() {
    emit('submit');
    visible.value = false;
}
</script>