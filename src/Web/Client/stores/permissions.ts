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
        async checkOrganizationPermissions(id: string) {
            if(!this.permissions) {
                const organizationsService = useOrganizationsService();
                const data = (await organizationsService.getUserPermissions(id));;
                this.permissions = data?.permissions ?? null;
                this.owner = data?.isOwner ?? null;
            }
        },
        async checkProjectPermissions(id: string) {
            if(!this.permissions) {
                const projectsService = useProjectsService();
                this.permissions = (await projectsService.getUserPermissions(id))?.permissions ?? null;
            }
        }
    }
})