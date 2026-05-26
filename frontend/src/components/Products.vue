<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { apiService } from '../services/api';
import type { Producto } from '../types';

const list = ref<Producto[]>([]);
const loading = ref(true);
const errorMsg = ref('');
const filterQuery = ref('');

// Estado del Modal
const isModalOpen = ref(false);
const isEditing = ref(false);
const submitting = ref(false);

// Campos del Formulario
const formId = ref<number | undefined>(undefined);
const formNombre = ref('');
const formDescripcion = ref('');
const formPrecio = ref<number | null>(null);
const formStock = ref<number | null>(null);

// Mensajes de error de validacion
const errors = ref({
  nombre: '',
  precio: '',
  stock: ''
});

const loadProducts = async () => {
  loading.value = true;
  errorMsg.value = '';
  try {
    const res = await apiService.productos.getAll();
    list.value = res;
  } catch (err: any) {
    console.error(err);
    errorMsg.value = 'Error al cargar los productos de la base de datos.';
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  loadProducts();
});

const filteredProducts = computed(() => {
  if (!filterQuery.value.trim()) return list.value;
  const q = filterQuery.value.toLowerCase().trim();
  return list.value.filter(p => 
    p.nombre.toLowerCase().includes(q) || 
    p.descripcion.toLowerCase().includes(q) ||
    p.id?.toString().includes(q)
  );
});

const openAddModal = () => {
  isEditing.value = false;
  formId.value = undefined;
  formNombre.value = '';
  formDescripcion.value = '';
  formPrecio.value = null;
  formStock.value = 10;
  errors.value = { nombre: '', precio: '', stock: '' };
  isModalOpen.value = true;
};

const openEditModal = (producto: Producto) => {
  isEditing.value = true;
  formId.value = producto.id;
  formNombre.value = producto.nombre;
  formDescripcion.value = producto.descripcion;
  formPrecio.value = producto.precio;
  formStock.value = producto.stock;
  errors.value = { nombre: '', precio: '', stock: '' };
  isModalOpen.value = true;
};

const validateForm = (): boolean => {
  let isValid = true;
  errors.value = { nombre: '', precio: '', stock: '' };

  if (!formNombre.value.trim()) {
    errors.value.nombre = 'Debe ingresar el nombre del producto.';
    isValid = false;
  }

  if (formPrecio.value === null || formPrecio.value <= 0) {
    errors.value.precio = 'Debe ingresar un precio mayor que 0.';
    isValid = false;
  }

  if (formStock.value === null || formStock.value < 0) {
    errors.value.stock = 'El stock no puede ser menor que 0.';
    isValid = false;
  }

  return isValid;
};

const handleSave = async () => {
  if (!validateForm()) return;

  submitting.value = true;
  const payload: Producto = {
    id: formId.value,
    nombre: formNombre.value.trim(),
    descripcion: formDescripcion.value.trim() || 'Sin descripcion',
    precio: Number(formPrecio.value),
    stock: Number(formStock.value)
  };

  try {
    if (isEditing.value && payload.id) {
      await apiService.productos.update(payload);
    } else {
      await apiService.productos.create(payload);
    }
    isModalOpen.value = false;
    await loadProducts();
  } catch (err: any) {
    console.error(err);
    alert('Ocurrio un error al intentar guardar el producto.');
  } finally {
    submitting.value = false;
  }
};

const handleDelete = async (id?: number) => {
  if (!id) return;
  if (!confirm('¿Desea borrar este producto de la base de datos?')) return;

  try {
    await apiService.productos.delete(id);
    await loadProducts();
  } catch (err: any) {
    console.error(err);
    alert('No se pudo borrar el producto.');
  }
};

const formatCurrency = (val: number) => {
  return 'RD$ ' + val.toLocaleString('es-DO', { minimumFractionDigits: 2 });
};
</script>

<template>
  <div class="space-y-4 font-sans text-gray-850">
    
    <!-- Titulo Principal -->
    <div class="flex flex-col sm:flex-row items-start sm:items-center justify-between border-b border-gray-300 pb-3 gap-2">
      <div>
        <h1 class="text-xl font-bold text-gray-800">Administracion de Productos</h1>
        <p class="text-xs text-gray-600">Crear, leer, actualizar y borrar elementos del inventario.</p>
      </div>
      <button 
        @click="openAddModal"
        class="bg-blue-600 hover:bg-blue-700 text-white font-bold text-xs px-3 py-2 rounded border border-blue-700 cursor-pointer"
      >
        Agregar Nuevo Producto
      </button>
    </div>

    <!-- Alertas -->
    <div v-if="errorMsg" class="bg-red-100 text-red-700 p-2 text-xs rounded border border-red-300">
      {{ errorMsg }}
    </div>

    <!-- Filtro simple -->
    <div class="bg-gray-100 p-3 rounded border border-gray-300 flex flex-col sm:flex-row gap-2 justify-between items-center text-xs">
      <div class="w-full sm:max-w-xs flex gap-1 items-center">
        <span>Buscar:</span>
        <input 
          v-model="filterQuery"
          type="text"
          placeholder="Escriba nombre o id para buscar..."
          class="w-full px-2 py-1 bg-white border border-gray-300 rounded focus:outline-none focus:ring-1 focus:ring-blue-500 font-sans"
        />
      </div>
      <span class="text-gray-500 font-bold">
        Total: {{ filteredProducts.length }} de {{ list.length }} productos
      </span>
    </div>

    <!-- Tabla de Productos -->
    <div class="border border-gray-300 bg-white rounded overflow-hidden">
      
      <div v-if="loading" class="p-8 text-center text-xs text-gray-500">
        Cargando tabla de productos... Por favor espere.
      </div>

      <div v-else-if="filteredProducts.length === 0" class="p-8 text-center text-xs text-gray-500">
        No se encontraron productos registrados en el sistema.
      </div>

      <div v-else class="overflow-x-auto">
        <table class="w-full text-left font-sans text-xs border-collapse">
          <thead>
            <tr class="bg-gray-200 border-b border-gray-300 font-bold text-gray-700">
              <th class="p-2 border-r border-gray-300 w-12">ID</th>
              <th class="p-2 border-r border-gray-300">Nombre</th>
              <th class="p-2 border-r border-gray-300">Descripcion</th>
              <th class="p-2 border-r border-gray-300">Precio de Venta</th>
              <th class="p-2 border-r border-gray-300 w-24 text-center">Disponible</th>
              <th class="p-2 w-32 text-center">Operaciones</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-250">
            <tr v-for="p in filteredProducts" :key="p.id" class="hover:bg-gray-50">
              
              <!-- ID -->
              <td class="p-2 border-r border-gray-300 font-mono text-center text-gray-800">
                {{ p.id }}
              </td>
              
              <!-- NOMBRE -->
              <td class="p-2 border-r border-gray-300 font-bold text-gray-800">
                {{ p.nombre }}
              </td>

              <!-- DESCRIPCION -->
              <td class="p-2 border-r border-gray-300 text-gray-600">
                {{ p.descripcion }}
              </td>
              
              <!-- PRECIO -->
              <td class="p-2 border-r border-gray-300 font-semibold text-gray-800">
                {{ formatCurrency(p.precio) }}
              </td>
              
              <!-- STOCK -->
              <td class="p-2 border-r border-gray-300 text-center">
                <span 
                  class="font-mono font-bold px-1.5 py-0.5 rounded"
                  :class="p.stock <= 0 ? 'bg-red-200 text-red-900 border border-red-300' : p.stock <= 5 ? 'bg-amber-200 text-amber-900 border border-amber-300' : 'bg-green-100 text-green-800 border border-green-200'"
                >
                  {{ p.stock }} uni
                </span>
              </td>
              
              <!-- ACCIONES -->
              <td class="p-2 text-center">
                <div class="flex justify-center gap-1">
                  <button 
                    @click="openEditModal(p)"
                    class="px-2 py-1 bg-gray-200 hover:bg-gray-300 border border-gray-400 rounded cursor-pointer font-bold"
                  >
                    Editar
                  </button>
                  <button 
                    @click="handleDelete(p.id)"
                    class="px-2 py-1 bg-red-100 hover:bg-red-200 border border-red-300 text-red-750 rounded cursor-pointer font-bold"
                  >
                    Borrar
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Modal Formulario -->
    <div v-if="isModalOpen" class="fixed inset-0 z-50 bg-black/50 flex items-center justify-center p-4">
      <div class="bg-white rounded shadow-lg max-w-sm w-full border border-gray-300 overflow-hidden">
        
        <div class="bg-gray-200 border-b border-gray-350 px-4 py-2 flex justify-between items-center">
          <span class="font-bold text-gray-800 text-sm">
            {{ isEditing ? 'Editar Producto Seleccionado' : 'Registrar Nuevo Producto' }}
          </span>
          <button @click="isModalOpen = false" class="text-gray-500 hover:text-gray-850 font-bold font-mono">×</button>
        </div>

        <form @submit.prevent="handleSave" class="p-4 space-y-3 text-xs font-sans">
          
          <div class="space-y-1">
            <label for="form-nombre" class="block font-bold text-gray-700">Nombre del Producto:</label>
            <input 
              id="form-nombre"
              v-model="formNombre"
              type="text"
              placeholder="Ej: Teclado de Computadora"
              class="w-full px-2 py-1.5 border border-gray-300 rounded focus:outline-none focus:ring-1 focus:ring-blue-500 bg-white"
            />
            <span v-if="errors.nombre" class="text-red-650 font-bold block">{{ errors.nombre }}</span>
          </div>

          <div class="space-y-1">
            <label for="form-descripcion" class="block font-bold text-gray-700">Descripcion / Caracteristicas:</label>
            <textarea 
              id="form-descripcion"
              v-model="formDescripcion"
              rows="2"
              placeholder="Ej: Inalambrico, color negro..."
              class="w-full px-2 py-1.5 border border-gray-300 rounded focus:outline-none focus:ring-1 focus:ring-blue-500 bg-white"
            ></textarea>
          </div>

          <div class="grid grid-cols-2 gap-2">
            <div class="space-y-1">
              <label for="form-precio" class="block font-bold text-gray-700">Precio (RD$):</label>
              <input 
                id="form-precio"
                v-model.number="formPrecio"
                type="number"
                step="any"
                placeholder="0.00"
                class="w-full px-2 py-1.5 border border-gray-300 rounded focus:outline-none focus:ring-1 focus:ring-blue-500 bg-white font-mono"
              />
              <span v-if="errors.precio" class="text-red-650 font-bold block text-[10px]">{{ errors.precio }}</span>
            </div>

            <div class="space-y-1">
              <label for="form-stock" class="block font-bold text-gray-700">Stock Inicial:</label>
              <input 
                id="form-stock"
                v-model.number="formStock"
                type="number"
                placeholder="10"
                class="w-full px-2 py-1.5 border border-gray-300 rounded focus:outline-none focus:ring-1 focus:ring-blue-500 bg-white font-mono"
              />
              <span v-if="errors.stock" class="text-red-650 font-bold block text-[10px]">{{ errors.stock }}</span>
            </div>
          </div>

          <div class="flex items-center justify-end gap-2 pt-3 border-t border-gray-200">
            <button 
              type="button" 
              @click="isModalOpen = false"
              class="px-3 py-1.5 border border-gray-305 font-bold hover:bg-gray-50 rounded text-gray-700 cursor-pointer"
            >
              Cancelar
            </button>
            <button 
              type="submit"
              :disabled="submitting"
              class="px-3 py-1.5 bg-blue-600 hover:bg-blue-700 text-white font-bold rounded cursor-pointer disabled:opacity-50"
            >
              {{ submitting ? 'Guardando...' : 'Confirmar' }}
            </button>
          </div>
        </form>
      </div>
    </div>

  </div>
</template>
