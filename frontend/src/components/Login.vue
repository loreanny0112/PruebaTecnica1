<script setup lang="ts">
import { ref } from 'vue';
import { apiService, apiMode, apiBaseUrl, saveApiSettings } from '../services/api';

const emit = defineEmits(['auth-success']);

const username = ref('loreanny');
const password = ref('12345678');
const isLoading = ref(false);
const errorMsg = ref('');
const showSettings = ref(false);

const localMode = ref<'mock' | 'real'>(apiMode.value);
const localBaseUrl = ref(apiBaseUrl.value);

const handleLogin = async () => {
  if (!username.value.trim() || !password.value.trim()) {
    errorMsg.value = 'Por favor, ingrese el usuario y la contraseña.';
    return;
  }

  isLoading.value = true;
  errorMsg.value = '';

  try {
    saveApiSettings(localMode.value, localBaseUrl.value);
    await apiService.auth.login(username.value, password.value);
    emit('auth-success');
  } catch (err: any) {
    console.error(err);
    errorMsg.value = err.response?.data?.message || err.message || 'Error de conexión. Verifique los datos o configure el modo demo.';
  } finally {
    isLoading.value = false;
  }
};

const useDemoSettings = () => {
  localMode.value = 'mock';
  username.value = 'admin';
  password.value = 'admin123';
  showSettings.value = false;
};
</script>

<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-200 px-4 py-12 font-sans text-gray-850">
    <div class="max-w-md w-full space-y-6 bg-white p-6 rounded border border-gray-300 shadow">
      <div>
        <h2 class="text-center text-2xl font-bold text-gray-800">
          Control de Acceso
        </h2>
        <p class="mt-1 text-center text-xs text-gray-650">
          Formulario de autenticacion para entrar al sistema
        </p>
      </div>

      <!-- Alerta de Error -->
      <div v-if="errorMsg" class="bg-red-100 border border-red-300 text-red-700 px-3 py-2 rounded text-xs">
        <strong>Error:</strong> {{ errorMsg }}
      </div>

      <form class="space-y-4" @submit.prevent="handleLogin">
        <div class="space-y-3">
          <div>
            <label for="username" class="block text-xs font-bold text-gray-600 mb-1">Nombre de Usuario:</label>
            <input
              id="username"
              v-model="username"
              type="text"
              required
              class="block w-full px-3 py-2 border border-gray-300 rounded text-sm bg-white focus:outline-none focus:ring-1 focus:ring-blue-500"
              placeholder="Escriba el usuario (Ej: admin)"
            />
          </div>

          <div>
            <label for="password" class="block text-xs font-bold text-gray-600 mb-1">Contraseña:</label>
            <input
              id="password"
              v-model="password"
              type="password"
              required
              class="block w-full px-3 py-2 border border-gray-300 rounded text-sm bg-white focus:outline-none focus:ring-1 focus:ring-blue-500"
              placeholder="••••••••"
            />
          </div>
        </div>

        <!-- Ajustes de Conexion -->
        <div class="border-t border-gray-200 pt-3">
          <div class="flex items-center justify-between mb-2">
            <button
              type="button"
              @click="showSettings = !showSettings"
              class="text-xs text-blue-600 hover:underline"
            >
              {{ showSettings ? '[ Ocultar Ajustes de API ]' : '[ Cambiar IP / Servidor o Modo Demo ]' }}
            </button>
            <span class="text-xs font-bold px-1.5 py-0.5 rounded" :class="localMode === 'mock' ? 'bg-yellow-200 text-yellow-800' : 'bg-green-200 text-green-800'">
              {{ localMode === 'mock' ? 'Demo local' : 'API C# Activo' }}
            </span>
          </div>

          <div v-if="showSettings" class="bg-gray-100 p-3 rounded border border-gray-300 space-y-3 mt-1 text-xs">
            <div>
              <span class="block font-bold text-gray-700 mb-1">Modo de Funcionamiento</span>
              <div class="flex gap-2">
                <button
                  type="button"
                  @click="localMode = 'mock'"
                  class="flex-1 py-1.5 text-center font-bold rounded border"
                  :class="localMode === 'mock' ? 'bg-yellow-500 border-yellow-600 text-black' : 'bg-white border-gray-300 text-gray-700'"
                >
                  Modo Demo (localStorage)
                </button>
                <button
                  type="button"
                  @click="localMode = 'real'"
                  class="flex-1 py-1.5 text-center font-bold rounded border"
                  :class="localMode === 'real' ? 'bg-blue-600 border-blue-700 text-white' : 'bg-white border-gray-300 text-gray-700'"
                >
                  API C# Real
                </button>
              </div>
            </div>

            <div v-if="localMode === 'real'">
              <label class="block font-bold text-gray-700 mb-1">Dirección URL de la API:</label>
              <input
                v-model="localBaseUrl"
                type="text"
                class="w-full px-2 py-1 border border-gray-300 rounded text-xs bg-white focus:outline-none focus:ring-1 focus:ring-blue-500"
              />
              <p class="text-[10px] text-gray-500 mt-1">Ej: http://localhost:5034/api o https://localhost:44314/api</p>
            </div>
            
            <div v-else class="text-[10px] text-gray-500 leading-normal">
              El <strong>Modo Demo</strong> no necesita API encendido. Es para probar rapido los modulos. Guarda en localStorage.
            </div>
          </div>
        </div>

        <div>
          <button
            type="submit"
            :disabled="isLoading"
            class="w-full py-2 px-4 rounded text-sm font-bold text-white bg-blue-600 hover:bg-blue-700 disabled:opacity-50 cursor-pointer"
          >
            <span v-if="isLoading">Cargando... Un momento...</span>
            <span v-else>Entrar al Sistema</span>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>
