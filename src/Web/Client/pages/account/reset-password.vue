<template>
    <AccountLayout>
        <AccountPageTitle title="Reset Password"></AccountPageTitle>
        <div>
            <label for="password1" class="block text-900 font-medium mb-2">Password</label>
            <Password id="password1" v-model="newPassword" :feedback="false" class="w-full mb-5" inputClass="w-full" placeholder="Your password" toggleMask />
            <Button label="Reset Password" icon="pi pi-user" class="w-full mb-6" @click="updatePassword" :disabled="buttonDisabled"></Button>
        </div>
    </AccountLayout>
</template>

<script setup lang="ts">
definePageMeta({
    layout: false
});

const auth = useAuth();

const newPassword = ref();

const buttonDisabled = computed(() => {
    return !newPassword.value;
})

async function updatePassword() {
    await auth.updatePassword(newPassword.value);
    navigateTo('/');
}
</script>