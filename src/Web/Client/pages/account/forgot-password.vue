<template>
    <AccountLayout>
        <AccountPageTitle title="Forgot Password"></AccountPageTitle>
        <div>
            <label for="email1" class="block text-900 font-medium mb-2">Email</label>
            <InputText id="email1" v-model="email" type="email" class="w-full mb-3" placeholder="Your email adress" />

            <Button label="Continue" icon="pi pi-user" class="w-full mb-6" @click="sendResetPasswordEmail" :disabled="buttonDisabled"></Button>
            <div class="flex justify-center align-center">
                <a class="text-sm cursor-pointer underline" @click="navigateTo('/account/login')">Have an account? Sign In!</a>
            </div>
        </div>
    </AccountLayout>
</template>

<script setup lang="ts">
definePageMeta({
    layout: false
});

const auth = useAuth();

const email = ref();

const buttonDisabled = computed(() => {
    return !email.value;
})

async function sendResetPasswordEmail() {
    await auth.sendResetPasswordEmail(email.value);
}
</script>