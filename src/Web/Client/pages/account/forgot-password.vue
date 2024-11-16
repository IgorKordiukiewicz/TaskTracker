<template>
    <AccountLayout>
        <div class="text-center mb-5">
            <div class="text-900 text-3xl font-medium mb-3">Forgot Password</div>
        </div>
        <div>
            <label for="email1" class="block text-900 font-medium mb-2">Email</label>
            <InputText id="email1" v-model="email" type="email" class="w-full mb-3" placeholder="Your email adress" />

            <Button label="Continue" icon="pi pi-user" class="w-full mb-6" @click="sendResetPasswordEmail"></Button>
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

async function sendResetPasswordEmail() {
    await auth.sendResetPasswordEmail(email.value);
}
</script>