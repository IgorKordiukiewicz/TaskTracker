export const useApi = () => {
    const client = useSupabaseClient();
    const toast = useToast();
    const config = useRuntimeConfig();

    return {
        async sendPostRequest<T>(url: string, body: T, customHeaders?: Record<string, string>) {
            try {
                const { data, error } = await useFetch(`${config.public.apiUrl}${url}`, {
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
        async sendFormDataPostRequest(url: string, formData: FormData, customHeaders?: Record<string, string>) {
            try {
                const { data, error } = await useFetch(`${config.public.apiUrl}${url}`, {
                    method: 'POST',
                    body: formData,
                    headers: { 
                        ...getHeaders(await getAccessToken(), true),
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
                const { data, error } = await useFetch<T>(`${config.public.apiUrl}${url}`, {
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

    function getHeaders(accessToken?: string, skipContentType = false) {
        const headers: Record<string, string> = {
        Authorization: `Bearer ${accessToken}`,
        };

        if (!skipContentType) {
            headers['Content-Type'] = 'application/json';
        }

        return headers;
    }

    async function getAccessToken() {
        return (await client.auth.getSession()).data.session?.access_token;
    }

    function handleError(error: any) {
        
    }
}