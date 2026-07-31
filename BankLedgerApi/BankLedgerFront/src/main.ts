import { createApp } from 'vue'
import { createPinia } from 'pinia'
import PrimeVue from 'primevue/config';
import ConfirmationService from 'primevue/confirmationservice';
import Aura from '@primeuix/themes/aura';
import 'primeicons/primeicons.css'
import './style.css'

import App from './App.vue'
import router from './router'

const app = createApp(App)

app.use(PrimeVue, {
  theme: {
    preset: Aura,
    options: {
      prefix: 'p',
      darkModeSelector: 'system',
      cssLayer: {
        name: 'primevue',
        order: 'theme, base, primevue, utilities'
      },
      cssVariables: true
    }
  },
  locale: {
    startsWith: 'Começa com',
    contains: 'Contém',
    notContains: 'Não contém',
    endsWith: 'Termina com',
    equals: 'Igual a',
    notEquals: 'Diferente de',
    noFilter: 'Sem filtro',
    lt: 'Menor que',
    lte: 'Menor ou igual a',
    gt: 'Maior que',
    gte: 'Maior ou igual a',
    dateIs: 'A data é',
    dateIsNot: 'A data não é',
    dateBefore: 'A data é anterior a',
    dateAfter: 'A data é posterior a',
    clear: 'Limpar',
    apply: 'Aplicar',
    matchAll: 'Corresponder a todos',
    matchAny: 'Corresponder a qualquer',
    addRule: 'Adicionar regra',
    removeRule: 'Remover regra',
    accept: 'Sim',
    reject: 'Não',
    choose: 'Escolher',
    upload: 'Enviar',
    cancel: 'Cancelar',
    today: 'Hoje',
    weekHeader: 'Sem',
    firstDayOfWeek: 0,
    dateFormat: 'dd/mm/yy',
    weak: 'Fraca',
    medium: 'Média',
    strong: 'Forte',
    passwordPrompt: 'Informe uma senha',
    emptyFilterMessage: 'Nenhum resultado encontrado',
    searchMessage: '{0} resultados disponíveis',
    selectionMessage: '{0} itens selecionados',
    emptySelectionMessage: 'Nenhum item selecionado',
    emptySearchMessage: 'Nenhum resultado encontrado',
    emptyMessage: 'Nenhuma opção disponível',
    dayNames: ['domingo', 'segunda-feira', 'terça-feira', 'quarta-feira', 'quinta-feira', 'sexta-feira', 'sábado'],
    dayNamesShort: ['dom', 'seg', 'ter', 'qua', 'qui', 'sex', 'sáb'],
    dayNamesMin: ['Do', 'Se', 'Te', 'Qu', 'Qu', 'Se', 'Sá'],
    monthNames: [
      'janeiro', 'fevereiro', 'março', 'abril', 'maio', 'junho',
      'julho', 'agosto', 'setembro', 'outubro', 'novembro', 'dezembro',
    ],
    monthNamesShort: ['jan', 'fev', 'mar', 'abr', 'mai', 'jun', 'jul', 'ago', 'set', 'out', 'nov', 'dez'],
    chooseYear: 'Escolher ano',
    chooseMonth: 'Escolher mês',
    chooseDate: 'Escolher data',
    prevDecade: 'Década anterior',
    nextDecade: 'Próxima década',
    prevYear: 'Ano anterior',
    nextYear: 'Próximo ano',
    prevMonth: 'Mês anterior',
    nextMonth: 'Próximo mês',
    prevHour: 'Hora anterior',
    nextHour: 'Próxima hora',
    prevMinute: 'Minuto anterior',
    nextMinute: 'Próximo minuto',
    prevSecond: 'Segundo anterior',
    nextSecond: 'Próximo segundo',
    am: 'am',
    pm: 'pm',
  },
  license: import.meta.env.VITE_PRIMEUI_KEY
});

app.use(ConfirmationService)
app.use(createPinia())
app.use(router)

app.mount('#app')
