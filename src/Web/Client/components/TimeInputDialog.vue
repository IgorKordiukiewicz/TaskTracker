<template>
    <ActionDialog :header="props.header" submit-label="Confirm" @submit="submit" ref="dialog">
        <LabeledInput label="Time">
            <InputText class="w-full" v-model="input" :invalid="!isInputValid" />
        </LabeledInput>
    </ActionDialog>
</template>

<script setup lang="ts">
defineExpose({ show });
const emit = defineEmits([ 'onSubmit' ]);
const props = defineProps({
    header: { type: String, required: true }
})

const timeParser = useTimeParser();

const dialog = ref();
const input = ref();

const isInputValid = computed(() => {
    return timeParser.tryToMinutes(input.value).result;
})

function show() {
    dialog.value.show();
}

function submit() {
    emit('onSubmit', timeParser.tryToMinutes(input.value).value);
}
</script>