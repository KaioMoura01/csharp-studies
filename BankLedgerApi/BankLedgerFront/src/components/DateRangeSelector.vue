<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import Button from 'primevue/button'
import Popover from 'primevue/popover'
import SelectButton from 'primevue/selectbutton'
import DatePicker from 'primevue/datepicker'

export interface DateRange {
  from: Date
  to: Date
}

const emit = defineEmits<{
  'update:range': [range: DateRange]
}>()

interface Preset {
  label: string
  days: number
}

const presets: Preset[] = [
  { label: '7 dias', days: 7 },
  { label: '15 dias', days: 15 },
  { label: '21 dias', days: 21 },
  { label: '30 dias', days: 30 },
]

const selectedPreset = ref<Preset | undefined>(presets[3])
const customDates = ref<Date[]>()
const popover = ref()
const currentRange = ref<DateRange | null>(null)

const dateFormatter = new Intl.DateTimeFormat('pt-BR', { day: '2-digit', month: '2-digit' })

const periodLabel = computed(() => {
  if (!currentRange.value) return 'Alterar período'
  return `${dateFormatter.format(currentRange.value.from)} - ${dateFormatter.format(currentRange.value.to)}`
})

function toggle(event: Event) {
  popover.value.toggle(event)
}

function rangeFromDays(days: number): DateRange {
  const to = new Date()
  const from = new Date()
  from.setDate(from.getDate() - (days - 1))
  return { from, to }
}

function applyPreset(preset: Preset) {
  if (!preset) return

  selectedPreset.value = preset
  customDates.value = undefined
  const range = rangeFromDays(preset.days)
  currentRange.value = range
  emit('update:range', range)
  popover.value.hide()
}

function applyCustomRange() {
  const [from, to] = customDates.value ?? []
  if (!from || !to) return

  selectedPreset.value = undefined
  currentRange.value = { from, to }
  emit('update:range', { from, to })

  setTimeout(() => popover.value.hide(), 150)
}

watch(customDates, (dates) => {
  const [from, to] = dates ?? []
  if (from && to) applyCustomRange()
})

onMounted(() => {
  const range = rangeFromDays(selectedPreset.value!.days)
  currentRange.value = range
  emit('update:range', range)
})
</script>

<template>
  <div>
    <Button :label="periodLabel" icon="pi pi-calendar" severity="secondary" @click="toggle" />
    <Popover ref="popover">
      <div class="flex flex-col gap-4 w-72">
        <div class="flex flex-col gap-2">
          <span class="text-sm font-medium">Períodos rápidos</span>
          <SelectButton
            :modelValue="selectedPreset"
            :options="presets"
            optionLabel="label"
            @update:modelValue="applyPreset"
          />
        </div>
        <div class="flex flex-col gap-2">
          <span class="text-sm font-medium">Ou escolha no calendário</span>
          <DatePicker
            date-format="dd/mm/y"
            v-model="customDates"
            selectionMode="range"
            :manualInput="false"
            :maxDate="new Date()"
            showIcon
            fluid
          />
        </div>
      </div>
    </Popover>
  </div>
</template>
