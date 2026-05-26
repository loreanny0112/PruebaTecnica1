<script setup lang="ts">
import { ref, computed } from 'vue';
import Login from './components/Login.vue';
import Dashboard from './components/Dashboard.vue';
import Products from './components/Products.vue';
import Clients from './components/Clients.vue';
import NewSale from './components/NewSale.vue';
import SalesHistory from './components/SalesHistory.vue';

// Import reactive state from API service
import { token, currentUser, apiMode, apiBaseUrl, saveApiSettings, apiService } from './services/api';

// Navigational tabs
const activeTab = ref<'dashboard' | 'productos' | 'clientes' | 'ventas-nueva' | 'ventas-historial'>('dashboard');

// Modal API Settings State
const isSettingsOpen = ref(false);
const settingsMode = ref<'mock' | 'real'>(apiMode.value);
const settingsUrl = ref(apiBaseUrl.value);

// Map components for dynamic loading
const currentView = computed(() => {
  switch (activeTab.value) {
    case 'dashboard':
      return Dashboard;
    case 'productos':
      return Products;
    case 'clientes':
      return Clients;
    case 'ventas-nueva':
      return NewSale;
    case 'ventas-historial':
      return SalesHistory;
    default:
      return Dashboard;
  }
});

const navigateTo = (tabName: 'dashboard' | 'productos' | 'clientes' | 'ventas-nueva' | 'ventas-historial') => {
  activeTab.value = tabName;
};

const handleLoginSuccess = () => {
  // Re-sync local values mapping
  settingsMode.value = apiMode.value;
  settingsUrl.value = apiBaseUrl.value;
  activeTab.value = 'dashboard';
};

const handleLogout = () => {
  apiService.auth.logout();
  activeTab.value = 'dashboard';
};

const triggerSaveSettings = () => {
  saveApiSettings(settingsMode.value, settingsUrl.value);
  isSettingsOpen.value = false;
  // Trigger table reloads by quickly cycling the active tab, or simply letting the user re-query
  const prev = activeTab.value;
  activeTab.value = 'dashboard';
  setTimeout(() => { activeTab.value = prev; }, 50);
};
</script>

<template>
  <div class="min-h-screen bg-gray-100 flex flex-col font-sans antialiased text-gray-800">
    
    <!-- AUTH GUARD LAYER -->
    <Login v-if="!token" @auth-success="handleLoginSuccess" />

    <!-- MAIN PANEL MANAGEMENT FRAME -->
    <div v-else class="flex flex-col flex-1">
      
      <!-- SIMPLE CLASSIC HEADER NAVBAR -->
      <header class="bg-blue-700 text-white shadow-md">
        <div class="max-w-7xl mx-auto px-4 py-3 flex flex-col md:flex-row items-center justify-between gap-4">
          <!-- Logo banner -->
          <div class="flex items-center gap-2">
            <div>
              <h2 class="font-bold text-lg tracking-tight">Sistema de Ventas y Facturacion</h2>
              <p class="text-xs text-blue-200">Gestión de ventas de productos</p>
            </div>
          </div>

          <!-- Connection Info -->
          <div class="flex items-center gap-4 text-xs">
            <span 
              class="font-bold px-2.5 py-0.5 rounded text-[11px]"
              :class="apiMode === 'mock' ? 'bg-yellow-500 text-black' : 'bg-green-500 text-white'"
            >
              {{ apiMode === 'mock' ? 'Modo Demo local' : 'Modo API activo' }}
            </span>
            <button 
              @click="isSettingsOpen = true"
              class="px-2 py-1 bg-blue-800 hover:bg-blue-900 border border-blue-500 rounded text-xs text-white"
            >
              Configurar IP/Red
            </button>
            <div class="text-right text-[11px] text-blue-250 hidden lg:block border-l border-blue-600 pl-4">
              <p>Usuario: <strong>{{ currentUser }}</strong></p>
              <button @click="handleLogout" class="text-red-350 hover:underline font-bold">Cerrar Sesion</button>
            </div>
          </div>
        </div>

        <!-- TOP NAVIGATION MENUS -->
        <div class="bg-blue-800 border-t border-blue-900">
          <div class="max-w-7xl mx-auto px-2 flex flex-wrap gap-1">
            <!-- INICIO / TABLERO -->
            <button 
              @click="navigateTo('dashboard')"
              class="px-4 py-2.5 text-xs font-bold uppercase tracking-wider hover:bg-blue-905 cursor-pointer border-b-2"
              :class="activeTab === 'dashboard' ? 'border-yellow-405 bg-blue-900 text-yellow-300' : 'border-transparent text-blue-100 hover:text-white'"
            >
              Inicio
            </button>

            <!-- PRODUCTOS CRUD -->
            <button 
              @click="navigateTo('productos')"
              class="px-4 py-2.5 text-xs font-bold uppercase tracking-wider hover:bg-blue-905 cursor-pointer border-b-2"
              :class="activeTab === 'productos' ? 'border-yellow-405 bg-blue-900 text-yellow-300' : 'border-transparent text-blue-100 hover:text-white'"
            >
              Productos
            </button>

            <!-- CLIENTES CRUD -->
            <button 
              @click="navigateTo('clientes')"
              class="px-4 py-2.5 text-xs font-bold uppercase tracking-wider hover:bg-blue-905 cursor-pointer border-b-2"
              :class="activeTab === 'clientes' ? 'border-yellow-405 bg-blue-900 text-yellow-300' : 'border-transparent text-blue-100 hover:text-white'"
            >
              Clientes 
            </button>

            <!-- REGISTRAR VENTA -->
            <button 
              @click="navigateTo('ventas-nueva')"
              class="px-4 py-2.5 text-xs font-bold uppercase tracking-wider hover:bg-blue-905 cursor-pointer border-b-2"
              :class="activeTab === 'ventas-nueva' ? 'border-yellow-405 bg-blue-900 text-yellow-300' : 'border-transparent text-blue-100 hover:text-white'"
            >
              Registrar Venta
            </button>

            <!-- HISTORIAL DE VENTAS -->
            <button 
              @click="navigateTo('ventas-historial')"
              class="px-4 py-2.5 text-xs font-bold uppercase tracking-wider hover:bg-blue-905 cursor-pointer border-b-2"
              :class="activeTab === 'ventas-historial' ? 'border-yellow-405 bg-blue-900 text-yellow-300' : 'border-transparent text-blue-100 hover:text-white'"
            >
              Historial de Ventas
            </button>
          </div>
        </div>
      </header>

      <!-- VIEW CANVAS CORE -->
      <main class="flex-1 p-4 max-w-7xl w-full mx-auto pb-12">
        <!-- Info on Mobile for logout -->
        <div class="lg:hidden flex items-center justify-between mb-2 bg-white p-2 border border-gray-200 rounded text-xs">
          <span>Usuario: <strong>{{ currentUser }}</strong></span>
          <button @click="handleLogout" class="text-red-650 hover:underline font-bold">Cerrar Sesion</button>
        </div>

        <!-- Dynamic component mounting block -->
        <div class="bg-white p-5 border border-gray-200 rounded shadow-sm">
          <component :is="currentView" @navigate="navigateTo" />
        </div>
      </main>

    </div>

    <!-- API CONFIGURATOR MODAL -->
    <div v-if="isSettingsOpen" class="fixed inset-0 z-50 bg-black/50 flex items-center justify-center p-4">
      <div class="bg-white rounded-lg max-w-sm w-full shadow-lg relative p-5 border border-gray-300 space-y-4">
        <div class="flex items-center justify-between pb-2 border-b border-gray-200">
          <h3 class="font-bold text-gray-800 text-sm">Ajustes del Servidor</h3>
          <button @click="isSettingsOpen = false" class="text-gray-400 hover:text-gray-600 text-xl font-bold font-mono cursor-pointer">×</button>
        </div>

        <div class="space-y-4 text-xs font-semibold text-gray-600">
          <!-- Choose type of data source -->
          <div class="space-y-1.5">
            <label class="block text-[10px] uppercase tracking-wider text-gray-400 font-bold">Modo de Operacion</label>
            <div class="flex gap-2">
              <button 
                type="button" 
                @click="settingsMode = 'mock'"
                class="flex-1 py-2 font-bold text-center rounded border cursor-pointer"
                :class="settingsMode === 'mock' ? 'bg-yellow-500 text-black border-yellow-600' : 'bg-white text-gray-650 hover:bg-gray-55 border-gray-200'"
              >
                Local (Demo)
              </button>
              <button 
                type="button" 
                @click="settingsMode = 'real'"
                class="flex-1 py-2 font-bold text-center rounded border cursor-pointer"
                :class="settingsMode === 'real' ? 'bg-blue-600 text-white border-blue-700' : 'bg-white text-gray-650 hover:bg-gray-55 border-gray-200'"
              >
                API C# (.NET)
              </button>
            </div>
          </div>

          <!-- URL Inputs -->
          <div v-if="settingsMode === 'real'" class="space-y-1.5">
            <label for="settings-url-input" class="block text-[10px] uppercase tracking-wider text-gray-400 font-bold">Direccion URL Base de tu API</label>
            <input 
              id="settings-url-input"
              v-model="settingsUrl"
              type="text"
              class="w-full px-2 py-1.5 border border-gray-300 rounded text-gray-700 font-mono text-xs focus:outline-none focus:ring-1 focus:ring-blue-500 bg-gray-50"
            />
            <p class="text-[9px] text-gray-400 font-normal leading-tight">Ejemplo: https://localhost:44314/api o http://localhost:5034/api</p>
          </div>

          <div v-else class="text-[10px] text-gray-400 font-normal leading-relaxed">
            El <strong>Modo Demo</strong> guarda y lee todo del localStorage de este navegador, ideal si no tienes tu API backend C# en ejecucion.
          </div>
        </div>

        <!-- Confirm action buttons -->
        <div class="flex gap-2.5 pt-3 border-t border-gray-200">
          <button 
            @click="isSettingsOpen = false"
            class="flex-1 py-1.5 text-xs border border-gray-300 font-bold hover:bg-gray-50 text-gray-600 rounded cursor-pointer"
          >
            Cerrar
          </button>
          <button 
            @click="triggerSaveSettings"
            class="flex-1 py-1.5 text-xs bg-blue-600 hover:bg-blue-700 text-white rounded font-bold cursor-pointer"
          >
            Guardar Cambios
          </button>
        </div>
      </div>
    </div>

  </div>
</template>

