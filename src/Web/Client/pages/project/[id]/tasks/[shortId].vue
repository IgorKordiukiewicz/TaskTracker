<template>
    <div>
        <div class="flex justify-between items-center" v-if="details">
            <div class="flex items-center gap-2">
                <p class="text-lg">
                    [#{{ taskShortId }}] {{ details.title }}
                </p>
            </div>
            <div class="flex gap-1 items-center">
                <Button icon="pi pi-pencil" text severity="secondary" label="Title" />
                <Button icon="pi pi-chevron-left" text severity="secondary" label="Back" @click="navigateTo(`/project/${projectId}/`)" />
            </div>
        </div>
        <div class="flex gap-4 w-100 mt-4" v-if="details">
            <div class="flex flex-col gap-4 w-3/4">
                <div class="bg-white w-full shadow p-4 flex flex-col gap-3">
                    <div class="flex justify-between items-center">
                        <div class="flex items-center gap-3">
                            <i class="pi pi-align-left" />
                            <p class="font-semibold">
                                Description
                            </p>
                        </div>
                        <div v-if="descriptionEditValue !== details.description" class="flex gap-3">
                            <Button severity="secondary" text icon="pi pi-times" style="height: 24px; width: 24px;" @click="cancelDescriptionEdit" />
                            <Button severity="primary" text icon="pi pi-check" :disabled="!descriptionEditValue" style="height: 24px; width: 24px;" @click="updateDescription" />
                        </div>
                    </div>
                    <div>
                        <Textarea v-model="descriptionEditValue" rows="5" :fluid="true" :auto-resize="true" style="resize: none;" />
                    </div>

                </div>
                <div class="bg-white w-full shadow p-4 flex flex-col gap-3">
                    <div class="flex items-center gap-3">
                        <i class="pi pi-comments" />
                        <p class="font-semibold">
                            Comments
                        </p>
                    </div>
                    <div class="flex gap-2 items-center">
                        <InputText class="w-full" placeholder="Add a comment" />
                        <Button icon="pi pi-send" label="Send" />
                    </div>
                </div>
                <div class="bg-white w-full shadow p-4 flex flex-col gap-3">
                    <div class="flex items-center gap-3">
                        <i class="pi pi-history" />
                        <p class="font-semibold">
                            Activity
                        </p>
                    </div>
                </div>
            </div>
            <div class="flex flex-col gap-4 w-1/4">
                <div class="bg-white w-full shadow p-4 flex flex-col gap-3 text-sm">
                    <div class="flex items-center gap-3 font-semibold text-base">
                        <i class="pi pi-list" /> <!-- pi-hashtag -->
                        <p class="font-semibold">
                            Properties
                        </p>
                    </div>
                    <LabeledInput label="Status">
                        <Select />
                    </LabeledInput>
                    <LabeledInput label="Priority">
                        <Select />
                    </LabeledInput>
                    <LabeledInput label="Assignee">
                        <Select />
                    </LabeledInput>
                </div>
                <div class="bg-white w-full shadow p-4">
                    <div class="flex justify-between items-center">
                        <div class="flex items-center gap-3">
                            <i class="pi pi-stopwatch" />
                            <p class="font-semibold">
                                Time Tracking
                            </p>
                        </div>
                        <div class="flex gap-3">
                            <Button severity="secondary" text icon="pi pi-pencil" style="height: 24px; width: 24px;" />
                            <Button severity="secondary" text icon="pi pi-plus" style="height: 24px; width: 24px;"  />
                        </div>
                    </div>
                    <div class="flex gap-2 items-center w-full mt-4">
                        <div class="flex flex-col gap-1 items-center w-full">
                            <p class="text-sm">Logged</p>
                            <p class="text-base font-semibold">1h 36min</p>
                        </div>
                        <Knob :model-value="40" :value-template="(val) => `${val}%`" :stroke-width="10" readonly /> <!-- on hover show remaining -->
                        <!-- if logged > estimated: red color and value 100 -->
                        <div class="flex flex-col gap-1 items-center w-full">
                            <p class="text-sm">Estimated</p>
                            <p class="text-base font-semibold">4h</p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { UpdateTaskDescriptionDto } from '~/types/dtos/tasks';

const route = useRoute();
const tasksService = useTasksService();

const projectId = ref(route.params.id as string);
const taskShortId = ref(+(route.params.shortId as string));
const details = ref(await tasksService.getTask(taskShortId.value, projectId.value));

const descriptionEditValue = ref(details.value?.description);

async function updateDetails() {
    details.value = await tasksService.getTask(taskShortId.value, projectId.value);
}

function cancelDescriptionEdit() {
    descriptionEditValue.value = details.value!.description;
}

async function updateDescription() {
    const model = new UpdateTaskDescriptionDto();
    model.description = descriptionEditValue.value!;
    await tasksService.updateDescription(details.value!.id, projectId.value, model);
    await updateDetails();
}

</script>