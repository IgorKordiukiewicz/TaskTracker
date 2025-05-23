import type { UpdateUserDto } from "~/types/dtos/user";
import type { UserDetailsVM, UsersPresentationDataVM, UsersVM } from "~/types/viewModels/users";

export const useUsersService = () => {
    const api = useApi();

    return {
        async getPresentationData() {
            return await api.sendGetRequest<UsersPresentationDataVM>('users/presentation');
        },
        async getUser() {
            return await api.sendGetRequest<UserDetailsVM>('users/me');
        },
        async updateUser(model: UpdateUserDto) {
            await api.sendPostRequest('users/me/update-name', model);
        }
    }
}