import { UserRegistrationDto } from "~/types/dtos/user";

export const useAuth = () => {
    const supabaseClient = useSupabaseClient();
    const supabaseUser = useSupabaseUser();
    const api = useApi();
    const config = useRuntimeConfig();
    const colorGenerator = useColorGenerator();
    const toast = useToast();

    return {
        async login(email: string, password: string) {
            try {
                const result = await supabaseClient.auth.signInWithPassword({
                    email: email,
                    password: password
                });
                if(result.error) {
                    throw new Error(result.error.message);
                }

                const data = await api.sendGetRequest<boolean>('users/me/registered');

                if(!data) {
                    navigateTo('/account/complete-registration');
                }
                else {
                    navigateTo('/');
                }

                return true;
            }
            catch(error) {
                handleError(error);
                return false;
            }
        },
        async register(email: string, password: string) {
            try {
                const result = await supabaseClient.auth.signUp({
                    email: email,
                    password: password
                });
                if(result.error) {
                    throw new Error(result.error.message);
                }

                navigateTo('/account/login');
                return true;
            }
            catch(error) {
                handleError(error);
                return false;
            }
        },
        async completeRegistration(firstName: string, lastName: string) {
            const email = (await supabaseClient.auth.getUser()).data.user?.email;
            const model = new UserRegistrationDto();
            model.email = email!;
            model.firstName = firstName;
            model.lastName = lastName;
            model.avatarColor = colorGenerator.generateAvatarColor();
            try {
                await api.sendPostRequest('users/me/register', model);

                navigateTo('/');
                return true;
            }
            catch(error) {
                handleError(error);
                return false;
            }
        },
        async sendResetPasswordEmail(email: string) {
            await supabaseClient.auth.resetPasswordForEmail(email, {
                redirectTo: `${config.public.url}account/reset-password`
            });
        },
        async updatePassword(newPassword: string) {
            await supabaseClient.auth.updateUser({ password: newPassword });
        },
        async logout() {
            await supabaseClient.auth.signOut();
            navigateTo('/account/login');
        },
        isAuthenticated() {
            return supabaseUser.value != null;
        },
        getUser() {
            return supabaseUser.value;
        },
        getUserId() {
            return supabaseUser.value?.id;
        }
    }

    // TODO: Refactor to not duplicate between auth and api composables
    function handleError(error: any) {
        toast.add({ severity: 'error', 'summary': 'Request Error', detail: error?.message, life: 2000 });
    }
}