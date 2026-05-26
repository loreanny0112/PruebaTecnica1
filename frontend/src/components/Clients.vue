<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { apiService } from '../services/api';
import type { Cliente } from '../types';

const list = ref<Cliente[]>([]);
const loading = ref(true);
const errorMsg = ref('');
const filterQuery = ref('');

// Control del Modal
const isModalOpen = ref(false);
const isEditing = ref(false);
const submitting = ref(false);

// Campos del formulario
const formId = ref<number | undefined>(undefined);
const formNombre = ref('');
const formEmail = ref('');
const formTelefono = ref('');

// Errores de validacion
const errors = ref({
  nombre: '',
  email: '',
  telefono: ''
});

const loadClients = async () => {
  loading.value = true;
  errorMsg.value = '';
  try {
    const res = await apiService.clientes.getAll();
    list.value = res;
  } catch (err: any) {
    console.error(err);
    errorMsg.value = 'Error al cargar los clientes desde el servidor.';
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  loadClients();
});

const filteredClients = computed(() => {
  if (!filterQuery.value.trim()) return list.value;
  const q = filterQuery.value.toLowerCase().trim();
  return list.value.filter(c => 
    c.nombre.toLowerCase().includes(q) || 
    c.email.toLowerCase().includes(q) ||
    c.telefono.includes(q) ||
    c.id?.toString().includes(q)
  );
});

const openAddModal = () => {
  isEditing.value = false;
  formId.value = undefined;
  formNombre.value = '';
  formEmail.value = '';
  formTelefono.value = '';
  errors.value = { nombre: '', email: '', telefono: '' };
  isModalOpen.value = true;
};

const openEditModal = (cliente: Cliente) => {
  isEditing.value = true;
  formId.value = cliente.id;
  formNombre.value = cliente.nombre;
  formEmail.value = cliente.email;
  formTelefono.value = cliente.telefono;
  errors.value = { nombre: '', email: '', telefono: '' };
  isModalOpen.value = true;
};

const validateForm = (): boolean => {
  let isValid = true;
  errors.value = { nombre: '', email: '', telefono: '' };

  if (!formNombre.value.trim()) {
    errors.value.nombre = 'El nombre del cliente es obligatorio.';
    isValid = false;
  }

  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  if (!formEmail.value.trim()) {
    errors.value.email = 'El correo electronico es obligatorio.';
    isValid = false;
  } else if (!emailRegex.test(formEmail.value.trim())) {
    errors.value.email = 'Escriba un correo valido (ejemplo@correo.com).';
    isValid = false;
  }

  if (!formTelefono.value.trim()) {
    errors.value.telefono = 'El telefono es obligatorio.';
    isValid = false;
  }

  return isValid;
};

const handleSave = async () => {
  if (!validateForm()) return;

  submitting.value = true;
  const payload: Cliente = {
    id: formId.value,
    nombre: formNombre.value.trim(),
    email: formEmail.value.trim().toLowerCase(),
    telefono: formTelefono.value.trim()
  };

  try {
    if (isEditing.value && payload.id) {
      await apiService.clientes.update(payload);
    } else {
      await apiService.clientes.create(payload);
    }
    isModalOpen.value = false;
    await loadClients();
  } catch (err: any) {
    console.error(err);
    alert('Error al guardar el cliente.');
  } finally {
    submitting.value = false;
  }
};

const handleDelete = async (id?: number) => {
  if (!id) return;
  if (!confirm('¿Seguro que desea eliminar a este cliente?')) return;

  try {
    await apiService.clientes.delete(id);
    await loadClients();
  } catch (err: any) {
    console.error(err);
    alert('Error al eliminar cliente.');
  }
};
</script>

<template>
  <div class="space-y-4 font-sans text-gray-850">
    
    <!-- Titulo Principal -->
    <div class="flex flex-col sm:flex-row items-start sm:items-center justify-between border-b border-gray-300 pb-3 gap-2">
      <div>
        <h1 class="text-xl font-bold text-gray-800">Administracion de Clientes</h1>
        <p class="text-xs text-gray-600">Registro de nombres, telefonos y correos de los clientes del sistema.</p>
      </div>
      <button 
        @click="openAddModal"
        class="bg-blue-600 hover:bg-blue-700 text-white font-bold text-xs px-3 py-2 rounded border border-blue-700 cursor-pointer"
      >
        Agregar Nuevo Cliente
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
          placeholder="Escriba nombre o telefono..."
          class="w-full px-2 py-1 bg-white border border-gray-300 rounded focus:outline-none focus:ring-1 focus:ring-blue-500"
        />
      </div>
      <span class="text-gray-500 font-bold">
        Cantidad: {{ filteredClients.length }} de {{ list.length }} clientes
      </span>
    </div>

    <!-- Tabla de Clientes -->
    <div class="border border-gray-300 bg-white rounded overflow-hidden">
      
      <div v-if="loading" class="p-8 text-center text-xs text-gray-500">
        Cargando tabla de clientes...
      </div>

      <div v-else-if="filteredClients.length === 0" class="p-8 text-center text-xs text-gray-500">
        No se encontraron clientes registrados.
      </div>

      <div v-else class="overflow-x-auto">
        <table class="w-full text-left font-sans text-xs border-collapse">
          <thead>
            <tr class="bg-gray-200 border-b border-gray-300 font-bold text-gray-700">
              <th class="p-2 border-r border-gray-300 w-12">ID</th>
              <th class="p-2 border-r border-gray-300">Nombre Completo</th>
              <th class="p-2 border-r border-gray-300">Correo Electronico</th>
              <th class="p-2 border-r border-gray-300">Telefono</th>
              <th class="p-2 w-32 text-center">Operaciones</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-250">
            <tr v-for="c in filteredClients" :key="c.id" class="hover:bg-gray-50">
              
              <!-- ID -->
              <td class="p-2 border-r border-gray-300 font-mono text-center text-gray-800">
                {{ c.id }}
              </td>
              
              <!-- NOMBRE -->
              <td class="p-2 border-r border-gray-300 font-bold text-gray-800">
                {{ c.nombre }}
              </td>

              <!-- EMAIL -->
              <td class="p-2 border-r border-gray-300 font-mono text-gray-600">
                {{ c.email }}
              </td>
              
              <!-- TELEFONO -->
              <td class="p-2 border-r border-gray-300 font-mono text-gray-700">
                {{ c.telefono }}
              </td>
              
              <!-- ACCIONES -->
              <td class="p-2 text-center">
                <div class="flex justify-center gap-1">
                  <button 
                    @click="openEditModal(c)"
                    class="px-2 py-1 bg-gray-200 hover:bg-gray-300 border border-gray-400 rounded cursor-pointer font-bold"
                  >
                    Editar
                  </button>
                  <button 
                    @click="handleDelete(c.id)"
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
            {{ isEditing ? 'Editar Cliente Seleccionado' : 'Registrar Nuevo Cliente' }}
          </span>
          <button @click="isModalOpen = false" class="text-gray-500 hover:text-gray-850 font-bold font-mono">×</button>
        </div>

        <form @submit.prevent="handleSave" class="p-4 space-y-3 text-xs font-sans">
          
          <div class="space-y-1">
            <label for="client-nombre" class="block font-bold text-gray-700">Nombre del Cliente:</label>
            <input 
              id="client-nombre"
              v-model="formNombre"
              type="text"
              placeholder="Ej: Jose Miguel Santos"
              class="w-full px-2 py-1.5 border border-gray-300 rounded focus:outline-none focus:ring-1 focus:ring-blue-500 bg-white"
            />
            <span v-if="errors.nombre" class="text-red-650 font-bold block animate-fade-in text-[10px]">{{ errors.nombre }}</span>
          </div>

          <div class="space-y-1">
            <label for="client-email" class="block font-bold text-gray-700">Correo Electronico:</label>
            <input 
              id="client-email"
              v-model="formEmail"
              type="text"
              placeholder="pepe@gmail.com"
              class="w-full px-2 py-1.5 border border-gray-300 rounded focus:outline-none focus:ring-1 focus:ring-blue-500 bg-white font-mono"
            />
            <span v-if="errors.email" class="text-red-650 font-bold block animate-fade-in text-[10px]">{{ errors.email }}</span>
          </div>

          <div class="space-y-1">
            <label for="client-telefono" class="block font-bold text-gray-700">Numero de Telefono o Celular:</label>
            <input 
              id="client-telefono"
              v-model="formTelefono"
              type="text"
              placeholder="Ej: 809-555-0100"
              class="w-full px-2 py-1.5 border border-gray-300 rounded focus:outline-none focus:ring-1 focus:ring-blue-500 bg-white font-mono"
            />
            <span v-if="errors.telefono" class="text-red-650 font-bold block animate-fade-in text-[10px]">{{ errors.telefono }}</span>
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
