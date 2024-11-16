import { UserRegistrationDto } from "~/types/dtos/user";

export const useAuth = () => {
    const supabaseClient = useSupabaseClient();
    const supabaseUser = useSupabaseUser();
    const api = useApi();
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
            }
            catch(error) {
                console.log(error); // TODO
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
            }
            catch(error) {
                console.log(error); // TODO
            }
        },
        async completeRegistration(firstName: string, lastName: string) {
            const email = (await supabaseClient.auth.getUser()).data.user?.email;
            const model = new UserRegistrationDto(email!, firstName, lastName);
            await api.sendPostRequest('users/me/register', model);
            navigateTo('/');
        },
        async sendResetPasswordEmail(email: string) {
            await supabaseClient.auth.resetPasswordForEmail(email, {
                redirectTo: 'http://localhost:3000/account/reset-password'
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
        }
    }
}