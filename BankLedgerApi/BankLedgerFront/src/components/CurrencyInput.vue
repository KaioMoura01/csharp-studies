<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import InputText from 'primevue/inputtext'

const props = withDefaults(
  defineProps<{
    modelValue: number | null
    id?: string
    fluid?: boolean
    autofocus?: boolean
    disabled?: boolean
  }>(),
  { fluid: false, autofocus: false, disabled: false },
)

const emit = defineEmits<{
  'update:modelValue': [value: number | null]
}>()

const MAX_DIGITS = 15

const integerFormatter = new Intl.NumberFormat('pt-BR', { maximumFractionDigits: 0 })

function digitsFromValue(value: number | null): string {
  if (!value) return ''
  return Math.round(value * 100).toString()
}

const digits = ref(digitsFromValue(props.modelValue))

watch(
  () => props.modelValue,
  (value) => {
    const next = digitsFromValue(value)
    if (next !== digits.value) digits.value = next
  },
)

const display = computed(() => {
  if (!digits.value) return ''
  const padded = digits.value.padStart(3, '0')
  const integerPart = padded.slice(0, -2)
  const decimalPart = padded.slice(-2)
  return `R$ ${integerFormatter.format(Number(integerPart))},${decimalPart}`
})

function emitValue() {
  emit('update:modelValue', digits.value ? Number(digits.value) / 100 : null)
}

function handleKeydown(event: KeyboardEvent) {
  if (event.ctrlKey || event.metaKey || event.altKey) return

  if (/^\d$/.test(event.key)) {
    event.preventDefault()
    if (digits.value.length >= MAX_DIGITS) return
    digits.value = (digits.value + event.key).replace(/^0+(?=\d)/, '')
    emitValue()
    return
  }

  if (event.key === 'Backspace' || event.key === 'Delete') {
    event.preventDefault()
    digits.value = digits.value.slice(0, -1)
    emitValue()
    return
  }

  if (['Tab', 'Shift', 'ArrowLeft', 'ArrowRight', 'Home', 'End'].includes(event.key)) return

  event.preventDefault()
}

function handlePaste(event: ClipboardEvent) {
  event.preventDefault()
  const pasted = event.clipboardData?.getData('text') ?? ''
  const onlyDigits = pasted.replace(/\D/g, '')
  if (!onlyDigits) return
  digits.value = (digits.value + onlyDigits).replace(/^0+(?=\d)/, '').slice(0, MAX_DIGITS)
  emitValue()
}
</script>

<template>
  <InputText
    :id="id"
    :model-value="display"
    :fluid="fluid"
    :autofocus="autofocus"
    :disabled="disabled"
    inputmode="numeric"
    placeholder="R$ 0,00"
    @keydown="handleKeydown"
    @paste="handlePaste"
  />
</template>
