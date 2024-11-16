<template>
    <ActionDialog :header="props.header" submit-label="Confirm" @submit="submit" ref="dialog" :submit-disabled="submitDisabled">
        <LabeledInput label="Time">
            <InputText id="timeinput" class="w-full" v-model="input" />
            <p class="text-sm">Input has to be in format: {0}d {1}h {2}m</p>
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
const initialValue = ref();

const submitDisabled = computed(() => {
    return !timeParser.tryToMinutes(input.value).result || (initialValue.value && initialValue.value === input.value);
})

function show(initValue?: string) {
    initialValue.value = initValue;
    dialog.value.show();
}

function submit() {
    emit('onSubmit', timeParser.tryToMinutes(input.value).value);
}
</script>