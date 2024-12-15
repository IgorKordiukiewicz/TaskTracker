<template>
    <ActionDialog header="Log time" submit-label="Confirm" @submit="submit" ref="dialog" :submit-disabled="submitDisabled">
        <LabeledInput label="Date">
            <DatePicker v-model="date" />
        </LabeledInput>
        <LabeledInput label="Time">
            <InputText id="timeinput" class="w-full" v-model="input" />
            <p class="text-sm">Input has to be in format: {0}d {1}h {2}m</p>
        </LabeledInput>
    </ActionDialog>
</template>

<script setup lang="ts">
import { AddTaskLoggedTimeDto } from '~/types/dtos/tasks';

defineExpose({ show });
const emit = defineEmits([ 'onSubmit' ]);

const timeParser = useTimeParser();

const dialog = ref();
const input = ref();
const date = ref();

const submitDisabled = computed(() => {
    return !timeParser.tryToMinutes(input.value).result || !date.value;
})

function show() {
    dialog.value.show();
}

function submit() {
    const model = new AddTaskLoggedTimeDto();
    model.minutes = timeParser.tryToMinutes(input.value).value;
    model.day = date.value;
    emit('onSubmit', model);

    input.value = '';
    date.value = null;
}
</script>