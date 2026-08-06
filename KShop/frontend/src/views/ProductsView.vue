<script setup lang="ts">
import { computed, ref } from 'vue'
import Card from 'primevue/card'
import Tag from 'primevue/tag'
import Select from 'primevue/select'
import Skeleton from 'primevue/skeleton'
import Message from 'primevue/message'
import { ProductService } from '@/services/product_service'

const { products, categories, loading, error } = ProductService()

const selectedCategoryId = ref<string | null>(null)

const categoryOptions = computed(() => [
  { label: 'Todas as categorias', value: null },
  ...categories.value.map((c) => ({ label: c.name ?? '—', value: c.id })),
])

const filteredProducts = computed(() =>
  selectedCategoryId.value
    ? products.value.filter((p) => p.categoryId === selectedCategoryId.value)
    : products.value,
)

function formatPrice(price: number) {
  return price.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })
}
</script>

<template>
  <div class="flex flex-col gap-6">
    <div class="flex items-center justify-between gap-4">
      <h1 class="text-2xl font-bold">Produtos</h1>
      <Select
        v-model="selectedCategoryId"
        :options="categoryOptions"
        optionLabel="label"
        optionValue="value"
        class="w-64"
      />
    </div>

    <Message v-if="error" severity="error" variant="simple" size="small">{{ error }}</Message>

    <div v-if="loading" class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
      <Skeleton v-for="n in 6" :key="n" width="100%" height="16rem" />
    </div>

    <div v-else-if="filteredProducts.length === 0" class="text-surface-500">
      Nenhum produto encontrado.
    </div>

    <div v-else class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
      <Card v-for="product in filteredProducts" :key="product.id" class="overflow-hidden">
        <template #header>
          <img
            v-if="product.imageUrl"
            :src="product.imageUrl"
            :alt="product.name"
            class="h-48 w-full object-cover"
          />
          <div v-else class="h-48 w-full flex items-center justify-center bg-surface-100 dark:bg-surface-800">
            <i class="pi pi-image text-4xl text-surface-400" />
          </div>
        </template>
        <template #title>{{ product.name }}</template>
        <template #subtitle>
          <Tag v-if="product.categoryName" :value="product.categoryName" severity="secondary" />
        </template>
        <template #content>
          <p class="text-sm text-surface-500 line-clamp-2">{{ product.description }}</p>
          <div class="flex items-center justify-between mt-4">
            <span class="text-xl font-semibold text-primary">{{ formatPrice(product.price) }}</span>
            <span class="text-xs text-surface-500">{{ product.stock }} em estoque</span>
          </div>
        </template>
      </Card>
    </div>
  </div>
</template>
