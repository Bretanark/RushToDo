import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/gardener': {
        target: 'https://localhost:63240',
        changeOrigin: true,
        secure: false,
      },
      '/work-item': {
        target: 'https://localhost:63240',
        changeOrigin: true,
        secure: false,
      },
    },
  },
})
