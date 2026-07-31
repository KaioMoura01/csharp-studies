<script setup lang="ts">
import { onMounted, ref } from 'vue'
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
  emit('update:range', rangeFromDays(preset.days))
  popover.value.hide()
}

function applyCustomRange() {
  const [from, to] = customDates.value ?? []
  if (!from || !to) return

  selectedPreset.value = undefined
  emit('update:range', { from, to })
  popover.value.hide()
}

onMounted(() => {
  emit('update:range', rangeFromDays(selectedPreset.value!.days))
})
</script>

<template>
  <div>
    <Button label="Alterar período" icon="pi pi-calendar" severity="secondary" @click="toggle" />
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
            v-model="customDates"
            selectionMode="range"
            :manualInput="false"
            :maxDate="new Date()"
            showIcon
            fluid
          />
          <Button
            label="Aplicar período"
            size="small"
            :disabled="!customDates || customDates.length < 2 || !customDates[1]"
            @click="applyCustomRange"
          />
        </div>
      </div>
    </Popover>
  </div>
</template>
