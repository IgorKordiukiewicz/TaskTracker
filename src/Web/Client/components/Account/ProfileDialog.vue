<template>
    <ActionDialog header="My Profile" submit-label="Save" ref="dialog" v-if="userDetails" :submit-disabled="submitDisabled" @submit="updateUser">
        <div class="flex gap-4">
            <UserAvatar :user-id="auth.getUserId()!" :large="true" />
            <div class="flex flex-col gap-2">
                <p class="text-xl font-semibold">{{ userDetails.fullName }}</p>
                <p>{{ userDetails.email }}</p>
            </div>
        </div>
        <Divider class="profile-divider" />
        <LabeledInput label="First Name">
            <InputText v-model="model.firstName" autocomplete="off" class="w-full" />
        </LabeledInput>
        <LabeledInput label="Last Name">
            <InputText v-model="model.lastName" autocomplete="off" class="w-full" />
        </LabeledInput>
    </ActionDialog>
</template>

<script setup lang="ts">
import { UpdateUserDto } from '~/types/dtos/user';

defineExpose({ show });
const emit = defineEmits([ 'onCreate' ]);

const auth = useAuth();
const usersService = useUsersService();

const model = ref();
const userDetails = ref();

const dialog = ref();

const submitDisabled = computed(() => {
    return !model.value.firstName || !model.value.lastName 
        || (model.value.firstName === userDetails.value.firstName && model.value.lastName === userDetails.value.lastName);
})

async function show() {
    await updateUserDetails();
    dialog.value.show();
}

async function updateUserDetails() {
    userDetails.value = await usersService.getUser();
    if(!userDetails.value) {
        return;
    }

    model.value = new UpdateUserDto();
    model.value.firstName = userDetails.value.firstName;
    model.value.lastName = userDetails.value.lastName;
}

async function updateUser() {
    await usersService.updateUser(model.value);
    await updateUserDetails();
}
</script>

<style scoped>
.profile-divider {
    margin-bottom: 0.5rem;
    margin-top: 0.5rem;
}
</style>