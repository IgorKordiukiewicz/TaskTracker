import { UserRegistrationDto } from "~/types/dtos/user";

export const useAuth = () => {
    const supabaseClient = useSupabaseClient();
    const supabaseUser = useSupabaseUser();

    return {
        async login(email: string, password: string) {
            try
            {
                const result = await supabaseClient.auth.signInWithPassword({
                    email: email,
                    password: password
                });

                const accessToken = result.data.session?.access_token;
                const { data } = await useFetch<boolean>('https://localhost:7075/users/me/registered', {
                    method: 'GET',
                    headers: {
                        'Authorization': `Bearer ${accessToken}`
                    }
                });
                console.log(data.value);
                if(!data.value) {
                    navigateTo('/account/complete-registration');
                }
                else {
                    navigateTo('/');
                }
            }
            catch(error)
            {
                console.log(error);
            }
        },
        async register(firstName: string, lastName: string) {
            const email = (await supabaseClient.auth.getUser()).data.user?.email;
            const accessToken = (await supabaseClient.auth.getSession()).data.session?.access_token;
            const model = new UserRegistrationDto(email!, firstName, lastName);

            await useFetch('https://localhost:7075/users/me/register', {
                method: 'POST',
                body: JSON.stringify(model),
                headers: {
                    'Authorization': `Bearer ${accessToken}`
                }
            });

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