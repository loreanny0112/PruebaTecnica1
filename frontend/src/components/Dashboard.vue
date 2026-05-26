<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { apiService } from '../services/api';
import type { Producto, Cliente, Venta } from '../types';

const emit = defineEmits(['navigate']);

const loading = ref(true);
const productos = ref<Producto[]>([]);
const clientes = ref<Cliente[]>([]);
const ventas = ref<Venta[]>([]);
const errorMsg = ref('');

const loadDashboardData = async () => {
  loading.value = true;
  errorMsg.value = '';
  try {
    const [pRes, cRes, vRes] = await Promise.all([
      apiService.productos.getAll(),
      apiService.clientes.getAll(),
      apiService.ventas.getAll()
    ]);
    productos.value = pRes;
    clientes.value = cRes;
    ventas.value = vRes;
  } catch (err) {
    console.error(err);
    errorMsg.value = 'No se pudo cargar la informacion del tablero. Puede volver a intentar.';
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  loadDashboardData();
});

const totalSalesAmount = computed(() => {
  return ventas.value.reduce((total, v) => total + v.total, 0);
});

const lowStockProducts = computed(() => {
  return productos.value.filter(p => p.stock <= 5);
});

const recentSales = computed(() => {
  return [...ventas.value]
    .sort((a, b) => new Date(b.fecha).getTime() - new Date(a.fecha).getTime())
    .slice(0, 5);
});

const formatCurrency = (val: number) => {
  return 'RD$ ' + val.toLocaleString('es-DO', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
};

const formatDate = (dateStr: string) => {
  const d = new Date(dateStr);
  return d.toLocaleDateString('es-DO', { day: '2-digit', month: 'short', hour: '2-digit', minute: '2-digit' });
};
</script>

<template>
  <div class="space-y-6 font-sans text-gray-850">
    <!-- Encabezado simple -->
    <div class="flex flex-col sm:flex-row items-start sm:items-center justify-between border-b pb-4 border-gray-300">
      <div>
        <h1 class="text-xl font-bold text-gray-800">Pagina de Inicio</h1>
        <p class="text-xs text-gray-600">Bienvenido al sistema. Aqui tienes un resumen rapido de la base de datos.</p>
      </div>
      <button 
        @click="loadDashboardData" 
        class="px-3 py-1 text-xs font-bold bg-gray-200 border border-gray-400 rounded hover:bg-gray-300 cursor-pointer"
      >
        Actualizar Datos
      </button>
    </div>

    <!-- Error simple -->
    <div v-if="errorMsg" class="bg-red-100 text-red-800 p-3 rounded border border-red-300 text-xs text-left">
      <span>{{ errorMsg }}</span>
      <button @click="loadDashboardData" class="underline font-bold ml-2">Reintentar</button>
    </div>

    <!-- Bloques de Estadisticas Simples -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
      
      <div class="bg-blue-50 p-4 rounded border border-blue-300">
        <span class="text-[11px] font-bold text-blue-700 uppercase block">Ingresos Totales (Suma)</span>
        <span class="text-lg font-bold text-blue-900 block mt-1">
          {{ loading ? 'Cargando...' : formatCurrency(totalSalesAmount) }}
        </span>
      </div>

      <div class="bg-gray-50 p-4 rounded border border-gray-300">
        <span class="text-[11px] font-bold text-gray-700 uppercase block">Total de Facturas Realizadas</span>
        <span class="text-lg font-bold text-gray-900 block mt-1">
          {{ loading ? 'Cargando...' : ventas.length + ' ventas' }}
        </span>
      </div>

      <div class="bg-gray-50 p-4 rounded border border-gray-300">
        <span class="text-[11px] font-bold text-gray-700 uppercase block">Productos en Catalogo</span>
        <span class="text-lg font-bold text-gray-900 block mt-1">
          {{ loading ? 'Cargando...' : productos.length + ' items' }}
        </span>
      </div>

      <div class="bg-gray-50 p-4 rounded border border-gray-300">
        <span class="text-[11px] font-bold text-gray-700 uppercase block">Clientes Registrados</span>
        <span class="text-lg font-bold text-gray-900 block mt-1">
          {{ loading ? 'Cargando...' : clientes.length + ' clientes' }}
        </span>
      </div>

    </div>

    <!-- Secciones Principales -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      
      <!-- Tabla de Ultimas Ventas -->
      <div class="lg:col-span-2 space-y-4">
        <div class="border border-gray-300 rounded bg-white p-4">
          <div class="flex items-center justify-between border-b pb-2 mb-3 border-gray-200">
            <h3 class="text-sm font-bold text-gray-800">Ultimas Ventas Realizadas</h3>
            <button 
              @click="emit('navigate', 'ventas-historial')" 
              class="text-xs text-blue-700 hover:underline font-bold"
            >
              Ir a todo el historial →
            </button>
          </div>

          <div v-if="loading" class="text-xs text-gray-500 py-4">
            Buscando ventas en la base de datos...
          </div>

          <div v-else-if="recentSales.length === 0" class="text-xs text-gray-500 py-4">
            No hay ninguna venta registrada en el sistema todavia.
          </div>

          <table v-else class="w-full text-left text-xs border-collapse">
            <thead>
              <tr class="bg-gray-100 border-b border-gray-300">
                <th class="p-2 font-bold text-gray-700">Cliente</th>
                <th class="p-2 font-bold text-gray-700">Fecha / Hora</th>
                <th class="p-2 font-bold text-gray-700 text-right">Monto Total</th>
              </tr>
            </thead>
            <tbody>
              <tr 
                v-for="sale in recentSales" 
                :key="sale.id" 
                class="border-b border-gray-200 hover:bg-gray-50"
              >
                <td class="p-2 text-gray-800 font-semibold">{{ sale.cliente?.nombre || 'Cliente General' }}</td>
                <td class="p-2 text-gray-600">{{ formatDate(sale.fecha) }}</td>
                <td class="p-2 text-gray-800 font-bold text-right">{{ formatCurrency(sale.total) }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Alertas de Stock Critico -->
      <div class="space-y-4">
        <div class="border border-gray-300 rounded bg-white p-4">
          <div class="border-b pb-2 mb-3 border-gray-250">
            <h3 class="text-sm font-bold text-red-700">Alertas de Inventario Bajo</h3>
          </div>
          
          <div v-if="loading" class="text-xs text-gray-500">
            Verificando stock de productos...
          </div>
          
          <div v-else-if="lowStockProducts.length === 0" class="bg-green-100 border border-green-300 text-green-800 p-3 rounded text-xs">
            Todo correcto. Todos los productos tienen mas de 5 unidades.
          </div>
          
          <div v-else class="space-y-2">
            <p class="text-[11px] text-gray-600">Los siguientes productos tienen un stock critico (5 o menos unidades):</p>
            <ul class="divide-y divide-gray-200 border border-gray-200 rounded max-h-60 overflow-y-auto">
              <li 
                v-for="p in lowStockProducts" 
                :key="p.id" 
                class="p-2 flex justify-between items-center text-xs bg-red-50 hover:bg-red-100"
              >
                <span class="font-semibold text-gray-800 truncate pr-2">{{ p.nombre }}</span>
                <span class="px-2 py-0.5 bg-red-200 text-red-900 border border-red-300 rounded font-bold">
                  {{ p.stock }} uni.
                </span>
              </li>
            </ul>
          </div>
        </div>

        <!-- Enlaces directos simples -->
        <div class="border border-gray-300 rounded bg-white p-4 space-y-2 text-xs">
          <h4 class="font-bold text-gray-700">Accesos Rapidos</h4>
          <div class="grid grid-cols-2 gap-2">
            <button 
              @click="emit('navigate', 'ventas-nueva')" 
              class="bg-blue-600 hover:bg-blue-700 text-white font-bold p-2 rounded text-center cursor-pointer"
            >
              Registrar Venta
            </button>
            <button 
              @click="emit('navigate', 'productos')" 
              class="bg-gray-200 hover:bg-gray-300 text-gray-800 font-bold p-2 border border-gray-400 rounded text-center cursor-pointer"
            >
              Ver Productos
            </button>
          </div>
        </div>

      </div>

    </div>
  </div>
</template>
