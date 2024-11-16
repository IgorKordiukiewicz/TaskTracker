<template>
    <AccountLayout>
        <AccountPageTitle title="Sign In">
            <span class="text-600 font-medium line-height-3">Don't have an account?</span>
            <a class="font-medium ml-2 underline cursor-pointer" href="/account/register">Create today!</a>
        </AccountPageTitle>
        <div>
            <label for="email" class="block text-900 font-medium mb-2">Email</label>
            <InputText id="email" v-model="email" type="email" class="w-full mb-3" placeholder="Your email adress" />

            <label for="password" class="block text-900 font-medium mb-2">Password</label>
            <Password id="password" v-model="password" :feedback="false" class="w-full mb-5" inputClass="w-full" placeholder="Your password" toggleMask />

            <Button label="Sign In" icon="pi pi-user" class="w-full mb-6" @click="login" :disabled="buttonDisabled"></Button>
            <div class="flex justify-center align-center">
                <a class="text-sm cursor-pointer underline" @click="navigateTo('/account/forgot-password')">Forgot your password?</a>
            </div>
        </div>
    </AccountLayout>
</template>

<script setup lang="ts">
import { ref } from "vue";

definePageMeta({
    layout: false
});

const auth = useAuth();

const email = ref();
const password = ref();

const buttonDisabled = computed(() => {
    return !email.value || !password.value;
})

async function login() {
    await auth.login(email.value, password.value);
}

</script>