export const usePermissions = defineStore('permissions', {
    state: () => ({
        permissions: null as number | null,
        owner: false as boolean | null
    }),
    getters: {
        hasPermission: (state) => {
            return (permission: number) => state.permissions 
                ? (state.permissions & permission) === permission 
                : false;
        },
        isOwner: (state) => {
            return () => state.owner;
        }
    },
    actions: {
        async checkProjectPermissions(id: string) {
            if(!this.permissions) {
                const projectsService = useProjectsService();
                const data = await projectsService.getUserPermissions(id);
                this.permissions = data?.permissions ?? null;
                this.owner = data?.isOwner ?? null;
            }
        }
    }
})