<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { apiService } from '../services/api';
import type { Producto, Cliente, Venta, ProductoVendido } from '../types';

const emit = defineEmits(['sale-completed']);

// Datos cargados de la base de datos
const loading = ref(true);
const productsList = ref<Producto[]>([]);
const clientsList = ref<Cliente[]>([]);
const errorMsg = ref('');

// Estado de la venta actual
const selectedCliente = ref<Cliente | null>(null);
const cartItems = ref<ProductoVendido[]>([]);

// Busqueda y dropdowns
const productQuery = ref('');
const showProductDropdown = ref(false);

const clientQuery = ref('');
const showClientDropdown = ref(false);

// Recibo de venta exitosa
const successInvoice = ref<Venta | null>(null);
const isSubmittingSale = ref(false);

const loadData = async () => {
  loading.value = true;
  errorMsg.value = '';
  try {
    const [pRes, cRes] = await Promise.all([
      apiService.productos.getAll(),
      apiService.clientes.getAll()
    ]);
    productsList.value = pRes.filter(p => p.stock > 0);
    clientsList.value = cRes;
  } catch (err) {
    console.error(err);
    errorMsg.value = 'No se pudieron recuperar los datos de productos y clientes. Por favor intente re-auth.';
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  loadData();
});

const filteredProductsDropdown = computed(() => {
  if (!productQuery.value.trim()) return productsList.value;
  const q = productQuery.value.toLowerCase().trim();
  return productsList.value.filter(p => 
    p.nombre.toLowerCase().includes(q) && p.stock > 0
  );
});

const filteredClientsDropdown = computed(() => {
  if (!clientQuery.value.trim()) return clientsList.value;
  const q = clientQuery.value.toLowerCase().trim();
  return clientsList.value.filter(c => 
    c.nombre.toLowerCase().includes(q) || c.email.toLowerCase().includes(q)
  );
});

const addToCart = (product: Producto) => {
  const existing = cartItems.value.find(item => item.id === product.id);
  
  if (existing) {
    if (existing.cantidad >= product.stock) {
      alert('No hay mas stock de este producto para agregar.');
      return;
    }
    existing.cantidad++;
  } else {
    cartItems.value.push({
      id: product.id!,
      nombre: product.nombre,
      precio: product.precio,
      cantidad: 1
    });
  }
  
  productQuery.value = '';
  showProductDropdown.value = false;
};

const updateQuantity = (itemId: number, action: 'add' | 'subtract') => {
  const item = cartItems.value.find(i => i.id === itemId);
  if (!item) return;
  
  const originalProduct = productsList.value.find(p => p.id === itemId);
  if (!originalProduct) return;

  if (action === 'add') {
    if (item.cantidad >= originalProduct.stock) {
      alert('Límite del stock disponible ya alcanzado.');
      return;
    }
    item.cantidad++;
  } else if (action === 'subtract') {
    item.cantidad--;
    if (item.cantidad <= 0) {
      cartItems.value = cartItems.value.filter(i => i.id !== itemId);
    }
  }
};

const removeFromCart = (itemId: number) => {
  cartItems.value = cartItems.value.filter(i => i.id !== itemId);
};

const selectClient = (client: Cliente) => {
  selectedCliente.value = client;
  clientQuery.value = client.nombre;
  showClientDropdown.value = false;
};

const deselectClient = () => {
  selectedCliente.value = null;
  clientQuery.value = '';
};

const totalAmount = computed(() => {
  return cartItems.value.reduce((sum, item) => sum + (item.precio * item.cantidad), 0);
});

const handleCheckout = async () => {
  if (!selectedCliente.value) {
    alert('Debe asignar un cliente para hacer la venta.');
    return;
  }
  
  if (cartItems.value.length === 0) {
    alert('Debe agregar al menos un producto al carrito.');
    return;
  }

  isSubmittingSale.value = true;
  try {
    const freshSale: Partial<Venta> = {
      fecha: new Date().toISOString(),
      cliente: selectedCliente.value,
      productos: cartItems.value,
      total: totalAmount.value
    };

    const result = await apiService.ventas.create(freshSale);
    successInvoice.value = result;
    
    // Limpiar carrito
    cartItems.value = [];
    selectedCliente.value = null;
    clientQuery.value = '';
    
    emit('sale-completed');
  } catch (err: any) {
    console.error(err);
    alert('Error al guardar la venta. Intente nuevamente.');
  } finally {
    isSubmittingSale.value = false;
  }
};

const formatCurrency = (val: number) => {
  return 'RD$ ' + val.toLocaleString('es-DO', { minimumFractionDigits: 2 });
};

const closeInvoice = () => {
  successInvoice.value = null;
  loadData();
};
</script>

<template>
  <div class="space-y-4 font-sans text-gray-850">
    
    <!-- Encabezado simple -->
    <div class="border-b border-gray-300 pb-3">
      <h1 class="text-xl font-bold text-gray-800">Formulario de Ventas / Facturacion</h1>
      <p class="text-xs text-gray-600">Modulo para registrar una nueva venta y ver el comprobante.</p>
    </div>

    <!-- Alertas -->
    <div v-if="errorMsg" class="bg-red-100 text-red-700 p-2 border border-red-300 rounded text-xs">
      {{ errorMsg }}
    </div>

    <div v-if="loading" class="text-center py-10 text-xs text-gray-500">
      Inicializando modulo de caja...
    </div>

    <div v-else class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      
      <!-- Lado izquierdo (Cliente y Productos) -->
      <div class="lg:col-span-2 space-y-4">
        
        <!-- SECCION 1: SELECCIONAR CLIENTE -->
        <div class="bg-white p-4 border border-gray-300 rounded space-y-3">
          <h3 class="font-bold text-xs text-gray-700 uppercase">Paso 1: Seleccionar el Cliente</h3>

          <div class="relative">
            <!-- Vista de cliente seleccionado -->
            <div v-if="selectedCliente" class="bg-blue-50 border border-blue-200 p-3 rounded flex items-center justify-between text-xs">
              <div>
                <p class="font-bold text-gray-800">Cliente actual: {{ selectedCliente.nombre }}</p>
                <p class="text-gray-500 mt-1">Correo: {{ selectedCliente.email }} | Telefono: {{ selectedCliente.telefono }}</p>
              </div>
              <button 
                @click="deselectClient"
                class="px-2 py-1 bg-white hover:bg-gray-100 border border-gray-300 rounded text-xs cursor-pointer font-bold"
              >
                Cambiar Cliente
              </button>
            </div>

            <!-- Entrada de busqueda de cliente -->
            <div v-else>
              <input 
                v-model="clientQuery"
                @focus="showClientDropdown = true"
                type="text"
                placeholder="Busque un cliente por nombre o email..."
                class="w-full px-2 py-1.5 border border-gray-300 rounded text-xs focus:outline-none focus:ring-1 focus:ring-blue-500 bg-white"
              />
              
              <!-- Dropdown de busqueda -->
              <div 
                v-if="showClientDropdown" 
                class="absolute left-0 right-0 z-10 bg-white border border-gray-300 rounded shadow mt-1 max-h-40 overflow-y-auto divide-y divide-gray-200"
              >
                <div v-if="filteredClientsDropdown.length === 0" class="p-2 text-xs text-gray-500 italic text-center">
                  Ningun cliente coincide con la busqueda.
                </div>
                <div 
                  v-else
                  v-for="c in filteredClientsDropdown" 
                  :key="c.id"
                  @click="selectClient(c)"
                  class="p-2 hover:bg-gray-100 text-xs flex justify-between items-center cursor-pointer"
                >
                  <div>
                    <span class="font-bold text-gray-800">{{ c.nombre }}</span>
                    <span class="text-gray-500 ml-2">({{ c.email }})</span>
                  </div>
                  <span class="text-[10px] text-blue-600 underline">Asignar</span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- SECCION 2: SELECCIONAR PRODUCTOS -->
        <div class="bg-white p-4 border border-gray-300 rounded space-y-3">
          <h3 class="font-bold text-xs text-gray-700 uppercase">Paso 2: Buscar y Agregar Productos</h3>

          <div class="relative font-sans">
            <input 
              v-model="productQuery"
              @focus="showProductDropdown = true"
              type="text"
              placeholder="Escriba el nombre del producto para agregarlo..."
              class="w-full px-2 py-1.5 border border-gray-300 rounded text-xs focus:outline-none focus:ring-1 focus:ring-blue-500 bg-white"
            />
            
            <!-- Dropdown de productos -->
            <div 
              v-if="showProductDropdown" 
              class="absolute left-0 right-0 z-10 bg-white border border-gray-300 rounded shadow mt-1 max-h-48 overflow-y-auto divide-y divide-gray-200"
            >
              <div v-if="filteredProductsDropdown.length === 0" class="p-2 text-xs text-gray-500 italic text-center">
                No se encontraron productos disponibles.
              </div>
              <div 
                v-else
                v-for="p in filteredProductsDropdown" 
                :key="p.id"
                @click="addToCart(p)"
                class="p-2 hover:bg-gray-100 text-xs flex justify-between items-center cursor-pointer"
              >
                <div>
                  <span class="font-bold text-gray-800">{{ p.nombre }}</span>
                  <span class="text-gray-500 ml-2">- Precio: {{ formatCurrency(p.precio) }}</span>
                </div>
                <div class="text-right flex items-center gap-2">
                  <span class="text-[10px] bg-gray-150 px-1 border rounded text-gray-600">Disp: {{ p.stock }}</span>
                  <span class="text-xs text-blue-600 font-bold underline">Agregar</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Listado del Carrito actual -->
          <div class="pt-1">
            <div v-if="cartItems.length === 0" class="text-center py-6 bg-gray-50 border border-gray-200 rounded text-xs text-gray-500">
              El carrito de venta esta vacio. Busque arriba un item.
            </div>

            <div v-else class="space-y-1.5">
              <table class="w-full text-left text-xs border border-gray-200 rounded overflow-hidden">
                <thead>
                  <tr class="bg-gray-100 text-gray-700 font-bold border-b border-gray-200">
                    <th class="p-2">Producto</th>
                    <th class="p-2 w-28 text-center">Cantidad</th>
                    <th class="p-2 w-28 text-right">Subtotal</th>
                    <th class="p-2 w-16 text-center">Quitar</th>
                  </tr>
                </thead>
                <tbody>
                  <tr 
                    v-for="item in cartItems" 
                    :key="item.id" 
                    class="border-b border-gray-200 bg-white hover:bg-gray-50"
                  >
                    <td class="p-2 font-semibold text-gray-800">{{ item.nombre }}</td>
                    <td class="p-2 text-center">
                      <div class="inline-flex items-center border border-gray-300 rounded overflow-hidden bg-white">
                        <button 
                          type="button" 
                          @click="updateQuantity(item.id, 'subtract')"
                          class="px-2 py-0.5 font-bold hover:bg-gray-100 border-r border-gray-300 cursor-pointer"
                        >
                          -
                        </button>
                        <span class="px-2 font-mono font-bold">{{ item.cantidad }}</span>
                        <button 
                          type="button" 
                          @click="updateQuantity(item.id, 'add')"
                          class="px-2 py-0.5 font-bold hover:bg-gray-100 border-l border-gray-300 cursor-pointer"
                        >
                          +
                        </button>
                      </div>
                    </td>
                    <td class="p-2 text-right font-mono text-gray-800 font-bold">
                      {{ formatCurrency(item.precio * item.cantidad) }}
                    </td>
                    <td class="p-2 text-center">
                      <button 
                        @click="removeFromCart(item.id)" 
                        class="text-red-650 hover:underline font-bold"
                      >
                        Quitar
                      </button>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>

      <!-- Lado derecho (Resumen totalizadores) -->
      <div class="space-y-4">
        <div class="bg-white p-4 border border-gray-300 rounded space-y-4">
          <h3 class="font-bold text-xs text-gray-700 uppercase border-b border-gray-200 pb-2">Resumen de Venta</h3>

          <div class="space-y-2 text-xs">
            <div class="flex justify-between">
              <span class="text-gray-500">Cliente asignado:</span>
              <span class="font-bold text-gray-800">{{ selectedCliente ? selectedCliente.nombre : 'Ninguno' }}</span>
            </div>
            
            <div class="flex justify-between">
              <span class="text-gray-500">Lineas en carrito:</span>
              <span class="font-semibold text-gray-800 font-mono">{{ cartItems.length }} lineas</span>
            </div>

            <div class="flex justify-between">
              <span class="text-gray-500">Unidades totales:</span>
              <span class="font-semibold text-gray-800 font-mono">
                {{ cartItems.reduce((sum, item) => sum + item.cantidad, 0) }} uni.
              </span>
            </div>

            <div class="flex justify-between border-t border-gray-200 pt-2 text-sm">
              <span class="font-bold text-gray-800">Monto Neto:</span>
              <span class="font-bold text-blue-700 font-mono">{{ formatCurrency(totalAmount) }}</span>
            </div>
          </div>

          <button 
            @click="handleCheckout"
            :disabled="isSubmittingSale || cartItems.length === 0 || !selectedCliente"
            class="w-full py-2 bg-blue-600 hover:bg-blue-700 text-white font-bold text-xs rounded border border-blue-700 cursor-pointer disabled:opacity-40"
          >
            {{ isSubmittingSale ? 'Procesando registro...' : 'Guardar y Generar Factura' }}
          </button>
        </div>
      </div>
    </div>

    <!-- MODAL DE EXITO COMPROBANTE -->
    <div v-if="successInvoice" class="fixed inset-0 z-50 bg-black/50 flex items-center justify-center p-4">
      <div class="bg-white rounded max-w-sm w-full border border-gray-300 p-4 space-y-3 shadow-lg">
        
        <div class="text-center border-b pb-2 border-gray-200">
          <h3 class="font-bold text-gray-800 text-sm uppercase">Comprobante de Venta</h3>
          <p class="text-[10px] text-gray-500">ID de Factura: #{{ successInvoice.id }}</p>
          <p class="text-[10px] text-gray-500">Fecha: {{ new Date(successInvoice.fecha).toLocaleString() }}</p>
        </div>

        <div class="text-xs space-y-1">
          <p class="text-gray-500 font-bold">Cliente:</p>
          <p class="font-bold text-gray-800">{{ successInvoice.cliente.nombre }}</p>
          <p class="text-gray-600">Email: {{ successInvoice.cliente.email }}</p>
          <p class="text-gray-600">Tel: {{ successInvoice.cliente.telefono }}</p>
        </div>

        <div class="border-t border-b py-2 border-dashed border-gray-300 text-xs space-y-1">
          <p class="text-gray-500 font-bold mb-1">Detalle de Articulos:</p>
          <div 
            v-for="item in successInvoice.productos" 
            :key="item.id" 
            class="flex justify-between"
          >
            <span>{{ item.nombre }} (x{{ item.cantidad }})</span>
            <span class="font-mono">{{ formatCurrency(item.precio * item.cantidad) }}</span>
          </div>
        </div>

        <div class="text-xs flex justify-between font-bold text-gray-800 pt-1">
          <span>Total Pagado:</span>
          <span class="font-mono text-blue-750">{{ formatCurrency(successInvoice.total) }}</span>
        </div>

        <div class="pt-2 border-t border-gray-200">
          <button 
            @click="closeInvoice"
            class="w-full py-1.5 bg-gray-200 hover:bg-gray-300 text-gray-850 font-bold rounded text-xs border border-gray-400 cursor-pointer"
          >
            Cerrar Recibo
          </button>
        </div>
      </div>
    </div>

  </div>
</template>
