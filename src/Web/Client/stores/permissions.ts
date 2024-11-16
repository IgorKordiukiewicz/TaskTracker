export const usePermissions = defineStore('permissions', {
    state: () => ({
        permissions: null as number | null
    }),
    getters: {
        hasPermission: (state) => {
            return (permission: number) => state.permissions 
                ? (state.permissions & permission) === permission 
                : false;
        }
    },
    actions: {
        async checkOrganizationPermissions(id: string) {
            if(!this.permissions) {
                const organizationsService = useOrganizationsService();
                this.permissions = (await organizationsService.getUserPermissions(id))?.permissions ?? null;
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