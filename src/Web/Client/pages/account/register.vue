<template>
    <AccountLayout>
        <div class="text-center mb-5">
            <div class="text-900 text-3xl font-medium mb-3">Sign Up</div>
            <span class="text-600 font-medium line-height-3">Already have an account?</span>
            <a class="font-medium ml-2 underline cursor-pointer" href="/account/login">Sign In!</a>
        </div>
        <div>
            <label for="email" class="block text-900 font-medium mb-2">Email</label>
            <InputText id="email" v-model="email" type="email" class="w-full mb-3" placeholder="Your email address" />

            <label for="password" class="block text-900 font-medium mb-2">Password</label>
            <Password id="password" v-model="password" :feedback="false" class="w-full mb-5" inputClass="w-full" placeholder="Your password" toggleMask />

            <Button label="Sign Up" icon="pi pi-user" class="w-full" @click="register" :disabled="buttonDisabled"></Button>
        </div>
    </AccountLayout>
</template>

<script setup lang="ts">
definePageMeta({
    layout: false
});

const auth = useAuth();

const email = ref();
const password = ref();

const buttonDisabled = computed(() => {
    return !email.value || !password.value;
})

async function register() {
    await auth.register(email.value, password.value);
}
</script>