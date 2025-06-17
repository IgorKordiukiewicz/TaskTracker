<template>
    <div>
        <input type="file" :accept="fileTypes" @change="submitFile" style="display: none;" ref="fileInput" />
        <Button severity="secondary" text :icon="icon" style="height: 24px; width: 24px;" @click="openFileInput" />
    </div>
</template>

<script setup lang="ts">
const emit = defineEmits([ 'upload' ]);
defineProps({
    icon: { type: String, required: true },
    fileTypes: { type: String }
})

const fileInput = ref();

function submitFile(event: Event) {
    event.preventDefault();
    const input = event.target as HTMLInputElement;
    const files = input.files;
    if(!files || files.length === 0) {
        return;
    }

    emit('upload', files[0]);
}

function openFileInput() {
    fileInput.value.click();
}
</script>