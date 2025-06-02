import Aura from '@primevue/themes/aura';
import { defineNuxtConfig } from 'nuxt/config';

// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  ssr: false,
  compatibilityDate: '2024-04-03',
  devtools: { enabled: true },
  runtimeConfig: {
    public: {
      apiUrl: 'https://localhost:7075/',
      url: 'http://localhost:3000/'
    }
  },
  css: [ 
    'primeicons/primeicons.css', 
    './assets/css/main.css' 
  ],
  modules: ['@primevue/nuxt-module', '@nuxtjs/tailwindcss', '@nuxtjs/supabase', '@pinia/nuxt', 'nuxt-echarts'],
  components: [
    { 
      path: '~/components',
      pathPrefix: false
    }
  ],
  supabase: {
    redirectOptions: {
      login: '/account/login',
      callback: '/',
      exclude: [ '/account/forgot-password', '/account/reset-password', 'account/complete-registration', '/account/register' ]
    }
  },
  primevue: {
    options: {
      theme: {
        preset: Aura,
        options: {
          darkModeSelector: '.fake-theme' // TODO: Temporary light mode force
        }
      },
      ripple: true,
    },
    autoImport: true
  }
})