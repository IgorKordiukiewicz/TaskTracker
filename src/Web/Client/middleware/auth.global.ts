export default defineNuxtRouteMiddleware((to, _from) => {
    const user = useAuth().getUser();

    const allowedUrls = [ 'account-login', 'account-forgot-password', 'account-register' ];
    if(to.name && allowedUrls.includes(to.name.toString())) {
        if(user) {
            return navigateTo('/');
        }

        return;
    }

    if(!user) {
        return navigateTo('/account/login');
    }
})