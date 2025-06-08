import type { UserPresentationDataVM } from "~/types/viewModels/users";

export const useUsersPresentationData = () => {
    const usersService = useUsersService();
    const userId = useAuth().getUserId();

    return {
        async getUser(id: string) {
            return (await getUsers()).find(x => x.userId === id);
        },
        async reset() {
            await reset();
        }
    }

    async function getUsers() {
        if(!userId) {
            return [];
        }

        const data = sessionStorage.getItem(getStorageKey(userId));
        if(!data) {
            const result = await usersService.getPresentationData();
            if(!result) {
                return [];
            }

            sessionStorage.setItem(getStorageKey(userId), JSON.stringify(result.users));
            return result.users;
        }
        
        const users: UserPresentationDataVM[] = JSON.parse(data);
        return users;
    }

    async function reset() {
        if(!userId) {
            return;
        }

        const result = await usersService.getPresentationData();
        if(!result) {
            return;
        }

        sessionStorage.setItem(getStorageKey(userId), JSON.stringify(result.users));
    }

    function getStorageKey(userId: string) {
        return `upd-${userId}`;
    }
}