<template>
    <div class="flex items-center justify-center h-screen bg-primary-200">
        <div class="surface-card p-6 shadow-md rounded w-1/4 bg-gray-50">
            <div class="text-center mb-5">
                <div class="text-900 text-3xl font-medium mb-3">Reset Password</div>
            </div>
            <div>
                <label for="password1" class="block text-900 font-medium mb-2">Password</label>
                <Password id="password1" v-model="newPassword" :feedback="false" class="w-full mb-5" inputClass="w-full" placeholder="Your password" toggleMask />

                <Button label="Reset Password" icon="pi pi-user" class="w-full mb-6" @click="updatePassword"></Button>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
definePageMeta({
    layout: false
});

const auth = useAuth();

const newPassword = ref();

async function updatePassword() {
    await auth.updatePassword(newPassword.value);
    navigateTo('/');
}
</script>