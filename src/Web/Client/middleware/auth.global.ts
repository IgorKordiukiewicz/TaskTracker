export default defineNuxtRouteMiddleware((to, _from) => {
    const user = useSupabaseUser();

    const allowedUrls = [ 'account-login', 'account-forgot-password', 'account-register' ];
    if(to.name && allowedUrls.includes(to.name.toString())) {
        if(user.value) {
            return navigateTo('/');
        }

        return;
    }

    if(!user.value) {
        return navigateTo('/account/login');
    }
})