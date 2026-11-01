import { createApp } from 'vue'
import { createPinia } from 'pinia'
import PrimeVue from 'primevue/config'
import Aura from '@primeuix/themes/aura'
import ToastService from 'primevue/toastservice'
import ConfirmationService from 'primevue/confirmationservice'
import 'primeicons/primeicons.css'

import './style.css'
import App from './App.vue'
import router from './router'
import { useAuthStore } from './stores/auth_store'

const app = createApp(App)

app.use(PrimeVue, {
  theme: {
    preset: Aura,
    options: {
      prefix: 'p',
      darkModeSelector: 'system',
      cssLayer: {
        name: 'primevue',
        order: 'theme, base, primevue, utilities',
      },
      cssVariables: true,
    },
  },
  license: import.meta.env.VITE_PRIMEUI_KEY,
})
app.use(ToastService)
app.use(ConfirmationService)
app.use(createPinia())

// Valida o token guardado (issuer + validade) antes de montar, para o guard de rota nunca
// decidir com base num token de outro realm ou vencido.
await useAuthStore().initSession()

app.use(router)

app.mount('#app')
