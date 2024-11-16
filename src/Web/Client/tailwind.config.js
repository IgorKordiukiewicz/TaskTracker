export default {
    content: [
      './composables/*.ts',
      './pages/**/*.vue',
      './layouts/*.vue'
    ],
    theme: {
      extend: {},
    },
    plugins: [
      require('tailwindcss-primeui')
    ],
}