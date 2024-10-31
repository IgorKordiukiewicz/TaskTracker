import Aura from '@primevue/themes/aura';
import { defineNuxtConfig } from 'nuxt/config';

// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  ssr: false,
  compatibilityDate: '2024-04-03',
  devtools: { enabled: true },
  css: [ 
    'primeicons/primeicons.css', 
    './assets/css/main.css' 
  ],
  modules: ['@primevue/nuxt-module', '@nuxtjs/tailwindcss', '@nuxtjs/supabase'],
  supabase: {
    redirectOptions: {
      login: '/account/login',
      callback: '/',
      exclude: [ '/account/forgot-password', '/account/reset-password', 'account/complete-registration' ]
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