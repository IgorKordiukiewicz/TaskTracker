<template>
    <AccountLayout>
        <AccountPageTitle title="Complete Registration"></AccountPageTitle>
        <div>
            <label for="firstName" class="block text-900 font-medium mb-2">First Name</label>
            <InputText id="firstName" v-model="firstName" type="text" class="w-full mb-3" placeholder="Your first name" />

            <label for="lastName" class="block text-900 font-medium mb-2">Last Name</label>
            <InputText id="lastName" v-model="lastName" type="text" class="w-full mb-5" placeholder="Your last name" />

            <Button label="Continue" icon="pi pi-user" class="w-full mb-6" @click="completeRegistration" :disabled="buttonDisabled"></Button>
        </div>
    </AccountLayout>
</template>

<script setup lang="ts">
definePageMeta({
    layout: false
});

const auth = useAuth();

const firstName = ref();
const lastName = ref();

const buttonDisabled = computed(() => {
    return !firstName.value || !lastName.value;
})

async function completeRegistration() {
    const result = await auth.completeRegistration(firstName.value, lastName.value);
    if(!result) {

    }
}
</script>