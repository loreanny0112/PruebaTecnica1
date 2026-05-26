<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { apiService } from '../services/api';
import type { Venta } from '../types';

const list = ref<Venta[]>([]);
const loading = ref(true);
const errorMsg = ref('');
const searchQuery = ref('');

// Comprobante seleccionado
const selectedInvoice = ref<Venta | null>(null);

const loadSalesHistory = async () => {
  loading.value = true;
  errorMsg.value = '';
  try {
    const res = await apiService.ventas.getAll();
    list.value = res;
  } catch (err: any) {
    console.error(err);
    errorMsg.value = 'Error al cargar el historial de ventas.';
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  loadSalesHistory();
});

const filteredSales = computed(() => {
  const sorted = [...list.value].sort((a, b) => new Date(b.fecha).getTime() - new Date(a.fecha).getTime());
  
  if (!searchQuery.value.trim()) return sorted;
  const q = searchQuery.value.toLowerCase().trim();
  
  return sorted.filter(v => 
    v.cliente.nombre.toLowerCase().includes(q) ||
    v.cliente.email.toLowerCase().includes(q) ||
    String(v.id).includes(q) ||
    v.productos.some(p => p.nombre.toLowerCase().includes(q))
  );
});

// Suma de montos filtrados
const totalAmountFiltered = computed(() => {
  return filteredSales.value.reduce((sum, v) => sum + v.total, 0);
});

const formatCurrency = (val: number) => {
  return 'RD$ ' + val.toLocaleString('es-DO', { minimumFractionDigits: 2 });
};

const formatDate = (dateStr: string) => {
  const d = new Date(dateStr);
  return d.toLocaleDateString('es-DO', { day: '2-digit', month: '2-digit', year: 'numeric' });
};

const formatFullDate = (dateStr: string) => {
  const d = new Date(dateStr);
  return d.toLocaleDateString('es-DO', { day: '2-digit', month: 'long', year: 'numeric', hour: '2-digit', minute: '2-digit' });
};
</script>

<template>
  <div class="space-y-4 font-sans text-gray-850">
    
    <!-- Encabezado simple -->
    <div class="flex flex-col sm:flex-row items-start sm:items-center justify-between border-b border-gray-300 pb-3 gap-2">
      <div>
        <h1 class="text-xl font-bold text-gray-800">Historial de Ventas</h1>
        <p class="text-xs text-gray-600">Registro historico de todas las transacciones de venta facturadas.</p>
      </div>
      <button 
        @click="loadSalesHistory" 
        class="px-3 py-1 bg-gray-200 hover:bg-gray-300 border border-gray-400 rounded text-xs font-bold cursor-pointer"
      >
        Actualizar Historial
      </button>
    </div>

    <!-- Alertas -->
    <div v-if="errorMsg" class="bg-red-100 text-red-700 p-2 text-xs rounded border border-red-300">
      {{ errorMsg }}
    </div>

    <!-- Barra de Filtros -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-3 bg-gray-100 p-3 rounded border border-gray-300 items-center text-xs">
      <!-- Buscador -->
      <div class="md:col-span-2 flex items-center gap-1">
        <span>Buscar:</span>
        <input 
          v-model="searchQuery"
          type="text"
          placeholder="Escriba cliente, factura id o producto..."
          class="w-full px-2 py-1.5 bg-white border border-gray-300 rounded focus:outline-none focus:ring-1 focus:ring-blue-500"
        />
      </div>

      <!-- Reporte de suma total filtrado -->
      <div class="bg-blue-50 border border-blue-200 rounded p-1.5 text-right font-sans">
        <span class="text-[10px] uppercase font-bold text-blue-700 block">Suma Facturas Filtradas</span>
        <span class="text-sm font-bold text-blue-900 font-mono">{{ formatCurrency(totalAmountFiltered) }}</span>
      </div>
    </div>

    <!-- Tabla Historial -->
    <div class="border border-gray-300 bg-white rounded overflow-hidden">
      
      <div v-if="loading" class="p-8 text-center text-xs text-gray-500">
        Cargando historial de facturas...
      </div>

      <div v-else-if="filteredSales.length === 0" class="p-8 text-center text-xs text-gray-500">
        No se tienen ventas registradas en este periodo de busqueda.
      </div>

      <div v-else class="overflow-x-auto">
        <table class="w-full text-left font-sans text-xs border-collapse">
          <thead>
            <tr class="bg-gray-200 border-b border-gray-300 font-bold text-gray-700">
              <th class="p-2 border-r border-gray-300 w-16">ID Factura</th>
              <th class="p-2 border-r border-gray-300 w-24">Fecha</th>
              <th class="p-2 border-r border-gray-300">Cliente</th>
              <th class="p-2 border-r border-gray-300">Resumen Articulos</th>
              <th class="p-2 border-r border-gray-300 w-28 text-right">Monto Neto</th>
              <th class="p-2 w-20 text-center">Accion</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-250">
            <tr v-for="v in filteredSales" :key="v.id" class="hover:bg-gray-50">
              
              <!-- ID FACTURA -->
              <td class="p-2 border-r border-gray-300 font-mono text-center text-gray-800">
                {{ v.id }}
              </td>
              
              <!-- FECHA -->
              <td class="p-2 border-r border-gray-300 font-mono text-gray-600 text-center">
                {{ formatDate(v.fecha) }}
              </td>

              <!-- CLIENTE -->
              <td class="p-2 border-r border-gray-300 font-bold text-gray-800">
                {{ v.cliente?.nombre || 'Cliente General' }}
                <div class="text-[10px] font-normal text-gray-500 font-mono">{{ v.cliente?.email || '' }}</div>
              </td>
              
              <!-- PRODUCTOS DETALLE -->
              <td class="p-2 border-r border-gray-300 text-gray-700 max-w-xs truncate">
                {{ v.productos.map(p => `${p.nombre} (x${p.cantidad})`).join(', ') }}
              </td>

              <!-- MONTO TOTAL -->
              <td class="p-2 border-r border-gray-300 font-mono font-bold text-gray-800 text-right">
                {{ formatCurrency(v.total) }}
              </td>
              
              <!-- ACCIONES -->
              <td class="p-2 text-center">
                <button 
                  @click="selectedInvoice = v"
                  class="px-2 py-0.5 bg-gray-200 hover:bg-gray-350 border border-gray-400 rounded cursor-pointer font-bold"
                >
                  Ver Recibo
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- POPUP COMPROBANTE DE VENTA -->
    <div v-if="selectedInvoice" class="fixed inset-0 z-50 bg-black/50 flex items-center justify-center p-4">
      <div class="bg-white rounded max-w-sm w-full border border-gray-300 p-4 space-y-3 shadow-lg">
        
        <div class="text-center border-b pb-2 border-gray-200">
          <h3 class="font-bold text-gray-800 text-sm uppercase">Comprobante de Venta</h3>
          <p class="text-[10px] text-gray-500">ID de Factura: #{{ selectedInvoice.id }}</p>
          <p class="text-[10px] text-gray-500">Fecha: {{ formatFullDate(selectedInvoice.fecha) }}</p>
        </div>

        <div class="text-xs space-y-1">
          <p class="text-gray-500 font-bold">Cliente:</p>
          <p class="font-bold text-gray-800">{{ selectedInvoice.cliente?.nombre || 'Cliente General' }}</p>
          <p class="text-gray-600">Email: {{ selectedInvoice.cliente?.email || '' }}</p>
          <p class="text-gray-600">Tel: {{ selectedInvoice.cliente?.telefono || '' }}</p>
        </div>

        <div class="border-t border-b py-2 border-dashed border-gray-300 text-xs space-y-1">
          <p class="text-gray-500 font-bold mb-1">Detalle de Articulos:</p>
          <div 
            v-for="item in selectedInvoice.productos" 
            :key="item.id" 
            class="flex justify-between"
          >
            <span>{{ item.nombre }} (x{{ item.cantidad }})</span>
            <span class="font-mono">{{ formatCurrency(item.precio * item.cantidad) }}</span>
          </div>
        </div>

        <div class="text-xs flex justify-between font-bold text-gray-800 pt-1">
          <span>Total Pagado:</span>
          <span class="font-mono text-blue-750">{{ formatCurrency(selectedInvoice.total) }}</span>
        </div>

        <div class="pt-2 border-t border-gray-200">
          <button 
            @click="selectedInvoice = null"
            class="w-full py-1.5 bg-gray-200 hover:bg-gray-300 text-gray-850 font-bold rounded text-xs border border-gray-400 cursor-pointer"
          >
            Cerrar Recibo
          </button>
        </div>
      </div>
    </div>

  </div>
</template>
