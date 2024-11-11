export const useApi = () => {
    const client = useSupabaseClient();

    return {
        async sendPostRequest<T>(url: string, body: T, customHeaders?: Record<string, string>) {
            try {
                const { data, error } = await useFetch(`https://localhost:7075/${url}`, {
                    method: 'POST',
                    body: JSON.stringify(body),
                    headers: { 
                        ...getHeaders(await getAccessToken()),
                        ...customHeaders
                    }
                });
    
                if(error.value) {
                    throw new Error(error.value.data);
                }
            }
            catch(error: any) {
                handleError(error);
            }
        },
        async sendGetRequest<T>(url: string, customHeaders?: Record<string, string>) {
            try {
                const { data, error } = await useFetch<T>(`https://localhost:7075/${url}`, {
                    headers: { 
                        ...getHeaders(await getAccessToken()),
                        ...customHeaders
                    }
                });
    
                if(error.value) {
                    throw new Error(error.value.message);
                }
    
                return data.value;
            }
            catch(error: any) {
                handleError(error);
            }
        }
    }

    function getHeaders(accessToken?: string) {
        return {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${accessToken}`
        };
    }

    async function getAccessToken() {
        return (await client.auth.getSession()).data.session?.access_token;
    }

    function handleError(error: any) {
        // TODO
        //toast.add({ severity: 'error', 'summary': 'Request Error', detail: error?.message, life: 2000 });
    }
}