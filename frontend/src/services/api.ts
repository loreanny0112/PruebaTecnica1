import axios from 'axios';
import { ref } from 'vue';
import type { Producto, Cliente, Venta, User } from '../types';

// State and configuration for API mode
export const apiMode = ref<'mock' | 'real'>(
  (localStorage.getItem('api_mode') as 'mock' | 'real') || 'mock'
);

export const apiBaseUrl = ref<string>(
  localStorage.getItem('api_base_url') || 'https://localhost:44314/api'
);

export const token = ref<string | null>(
  localStorage.getItem('jwt_token')
);

export const currentUser = ref<string | null>(
  localStorage.getItem('current_user') || (token.value ? 'Admin User' : null)
);

// Configure custom axios instance
export const axiosInstance = axios.create({
  baseURL: apiBaseUrl.value,
  timeout: 10000,
});

// Axios interceptor for JWT
axiosInstance.interceptors.request.use(
  (config) => {
    if (token.value) {
      config.headers.Authorization = `Bearer ${token.value}`;
    }
    // Update dynamic base URL just in case
    config.baseURL = apiBaseUrl.value;
    return config;
  },
  (error) => Promise.reject(error)
);

// Toggle service helper
export function saveApiSettings(mode: 'mock' | 'real', baseUrl: string) {
  apiMode.value = mode;
  apiBaseUrl.value = baseUrl;
  localStorage.setItem('api_mode', mode);
  localStorage.setItem('api_base_url', baseUrl);
  axiosInstance.defaults.baseURL = baseUrl;
}

// SIMULATED EXPERT SEED DATA (For mock mode)
const DEFAULT_PRODUCTS: Producto[] = [
  { id: 101, nombre: 'Laptop Dell XPS 15', descripcion: 'Intel Core i9, 32GB RAM, 1TB SSD, RTX 4500', precio: 125000, stock: 12 },
  { id: 102, nombre: 'Mouse Logitech MX Master 3S', descripcion: 'Ratón inalámbrico silencioso para productividad ultra-preciso', precio: 5800, stock: 24 },
  { id: 103, nombre: 'Teclado Keychron Q2 Pro', descripcion: 'Teclado mecánico custom de aluminio 65% inalámbrico', precio: 11900, stock: 8 },
  { id: 104, nombre: 'Monitor Asus ProArt 27"', descripcion: 'Pantalla 4K IPS profesional calibrada de fábrica, sRGB 100%', precio: 34500, stock: 5 },
  { id: 105, nombre: 'Auriculares Sony WH-1000XM5', descripcion: 'Audífonos inalámbricos premium con cancelación activa de ruido', precio: 22000, stock: 15 },
  { id: 106, nombre: 'Cargador Anker GaNPrime 120W', descripcion: 'Cargador rápido multipuerto de pared de nitruro de galio', precio: 3800, stock: 40 }
];

const DEFAULT_CLIENTS: Cliente[] = [
  { id: 201, nombre: 'Juan Pérez', email: 'juan.perez@email.com', telefono: '809-555-0199' },
  { id: 202, nombre: 'María Rodríguez', email: 'maria.rod@email.com', telefono: '829-444-0122' },
  { id: 203, nombre: 'Carlos Gómez', email: 'carlos.gomez@email.com', telefono: '809-333-0145' },
  { id: 204, nombre: 'Ana Martínez', email: 'ana.martinez@email.com', telefono: '849-222-0188' }
];

const DEFAULT_SALES: Venta[] = [
  {
    id: 301,
    fecha: '2026-05-24T18:30:00Z',
    cliente: { id: 201, nombre: 'Juan Pérez', email: 'juan.perez@email.com', telefono: '809-555-0199' },
    productos: [
      { id: 102, nombre: 'Mouse Logitech MX Master 3S', precio: 5800, cantidad: 1 },
      { id: 106, nombre: 'Cargador Anker GaNPrime 120W', precio: 3800, cantidad: 2 }
    ],
    total: 13400
  },
  {
    id: 302,
    fecha: '2026-05-25T10:15:00Z',
    cliente: { id: 203, nombre: 'Carlos Gómez', email: 'carlos.gomez@email.com', telefono: '809-333-0145' },
    productos: [
      { id: 103, nombre: 'Teclado Keychron Q2 Pro', precio: 11900, cantidad: 1 }
    ],
    total: 11900
  }
];

// Load utilities for localStorage Mock Mode
function getMockData<T>(key: string, defaultVal: T[]): T[] {
  const data = localStorage.getItem(key);
  if (!data) {
    localStorage.setItem(key, JSON.stringify(defaultVal));
    return defaultVal;
  }
  return JSON.parse(data);
}

function saveMockData<T>(key: string, data: T[]) {
  localStorage.setItem(key, JSON.stringify(data));
}

// Latency simulation helper to mirror real APIs
const simulateLatency = () => new Promise(resolve => setTimeout(resolve, 250));

export const apiService = {
  // === AUTHENTICATION SERVICES ===
  auth: {
    async login(username: string, password: string): Promise<User> {
      await simulateLatency();
      if (apiMode.value === 'mock') {
        if (username.trim().toLowerCase() === 'admin' || username.includes('@')) {
          // Mock JWT token generation
          const mockToken = 'mock_jwt_token_header_payload_sign_admin_123456';
          const mockUser = 'Administrador Sistema';
          token.value = mockToken;
          currentUser.value = mockUser;
          localStorage.setItem('jwt_token', mockToken);
          localStorage.setItem('current_user', mockUser);
          return { username, token: mockToken };
        } else {
          throw new Error('Credenciales inválidas en modo demo. Use "admin" como usuario.');
        }
      } else {
        // Real API JWT Login
        const response = await axiosInstance.post<{ token: string; username?: string }>('/Auth/login', {
          username,
          password
        });
        const finalToken = response.data.token;
        const userLabel = response.data.username || username;
        token.value = finalToken;
        currentUser.value = userLabel;
        localStorage.setItem('jwt_token', finalToken);
        localStorage.setItem('current_user', userLabel);
        return { username: userLabel, token: finalToken };
      }
    },

    logout() {
      token.value = null;
      currentUser.value = null;
      localStorage.removeItem('jwt_token');
      localStorage.removeItem('current_user');
    }
  },

  // === PRODUCT CRUD SERVICES ===
  productos: {
    async getAll(): Promise<Producto[]> {
      await simulateLatency();
      if (apiMode.value === 'mock') {
        return getMockData<Producto>('mock_productos', DEFAULT_PRODUCTS);
      } else {
        const response = await axiosInstance.get<Producto[]>('/Producto');
        return response.data;
      }
    },

    async create(producto: Producto): Promise<Producto> {
      await simulateLatency();
      if (apiMode.value === 'mock') {
        const list = getMockData<Producto>('mock_productos', DEFAULT_PRODUCTS);
        const newId = list.length > 0 ? Math.max(...list.map(p => p.id || 0)) + 1 : 1;
        const fresh = { ...producto, id: newId };
        list.push(fresh);
        saveMockData('mock_productos', list);
        return fresh;
      } else {
        const response = await axiosInstance.post<Producto>('/Producto', {
          nombre: producto.nombre,
          descripcion: producto.descripcion || 'Sin descripción',
          precio: Number(producto.precio),
          stock: Number(producto.stock)
        });
        return response.data;
      }
    },

    async update(producto: Producto): Promise<Producto> {
      await simulateLatency();
      if (apiMode.value === 'mock') {
        const list = getMockData<Producto>('mock_productos', DEFAULT_PRODUCTS);
        const index = list.findIndex(p => p.id === producto.id);
        if (index === -1) throw new Error('Producto no encontrado');
        list[index] = { ...producto };
        saveMockData('mock_productos', list);
        return producto;
      } else {
        // Note: The user's code uses a PUT to /Producto taking full object
        const response = await axiosInstance.put<Producto>('/Producto', {
          id: producto.id,
          nombre: producto.nombre,
          descripcion: producto.descripcion || 'editado',
          precio: Number(producto.precio),
          stock: Number(producto.stock)
        });
        return response.data;
      }
    },

    async delete(id: number): Promise<void> {
      await simulateLatency();
      if (apiMode.value === 'mock') {
        const list = getMockData<Producto>('mock_productos', DEFAULT_PRODUCTS);
        const filtered = list.filter(p => p.id !== id);
        saveMockData('mock_productos', filtered);
      } else {
        await axiosInstance.delete(`/Producto/${id}`);
      }
    }
  },

  // === CLIENTS CRUD SERVICES ===
  clientes: {
    async getAll(): Promise<Cliente[]> {
      await simulateLatency();
      if (apiMode.value === 'mock') {
        return getMockData<Cliente>('mock_clientes', DEFAULT_CLIENTS);
      } else {
        // The standard REST patterns suggest /Cliente
        const response = await axiosInstance.get<Cliente[]>('/Cliente');
        return response.data;
      }
    },

    async create(cliente: Cliente): Promise<Cliente> {
      await simulateLatency();
      if (apiMode.value === 'mock') {
        const list = getMockData<Cliente>('mock_clientes', DEFAULT_CLIENTS);
        const newId = list.length > 0 ? Math.max(...list.map(c => c.id || 0)) + 1 : 1;
        const fresh = { ...cliente, id: newId };
        list.push(fresh);
        saveMockData('mock_clientes', list);
        return fresh;
      } else {
        const response = await axiosInstance.post<Cliente>('/Cliente', {
          nombre: cliente.nombre,
          email: cliente.email,
          telefono: cliente.telefono
        });
        return response.data;
      }
    },

    async update(cliente: Cliente): Promise<Cliente> {
      await simulateLatency();
      if (apiMode.value === 'mock') {
        const list = getMockData<Cliente>('mock_clientes', DEFAULT_CLIENTS);
        const index = list.findIndex(c => c.id === cliente.id);
        if (index === -1) throw new Error('Cliente no encontrado');
        list[index] = { ...cliente };
        saveMockData('mock_clientes', list);
        return cliente;
      } else {
        const response = await axiosInstance.put<Cliente>('/Cliente', {
          id: cliente.id,
          nombre: cliente.nombre,
          email: cliente.email,
          telefono: cliente.telefono
        });
        return response.data;
      }
    },

    async delete(id: number): Promise<void> {
      await simulateLatency();
      if (apiMode.value === 'mock') {
        const list = getMockData<Cliente>('mock_clientes', DEFAULT_CLIENTS);
        const filtered = list.filter(c => c.id !== id);
        saveMockData('mock_clientes', filtered);
      } else {
        await axiosInstance.delete(`/Cliente/${id}`);
      }
    }
  },

  // === SALES REGISTER AND HISTORY SERVICES ===
  ventas: {
    async getAll(): Promise<Venta[]> {
      await simulateLatency();
      if (apiMode.value === 'mock') {
        return getMockData<Venta>('mock_ventas', DEFAULT_SALES);
      } else {
        const response = await axiosInstance.get<Venta[]>('/Venta');
        return response.data;
      }
    },

    async create(venta: Partial<Venta>): Promise<Venta> {
      await simulateLatency();
      if (apiMode.value === 'mock') {
        const list = getMockData<Venta>('mock_ventas', DEFAULT_SALES);
        const newId = list.length > 0 ? Math.max(...list.map(v => v.id || 0)) + 1 : 1;
        
        // Decrement stock for purchased products in Mock database
        const prods = getMockData<Producto>('mock_productos', DEFAULT_PRODUCTS);
        venta.productos?.forEach(vp => {
          const match = prods.find(p => p.id === vp.id);
          if (match) {
            match.stock = Math.max(0, match.stock - vp.cantidad);
          }
        });
        saveMockData('mock_productos', prods);

        const fresh: Venta = {
          id: newId,
          fecha: new Date().toISOString(),
          cliente: venta.cliente as Cliente,
          productos: venta.productos || [],
          total: venta.total || 0
        };
        list.push(fresh);
        saveMockData('mock_ventas', list);
        return fresh;
      } else {
        // Send payload in the exact mapping preferred by typical C# controllers
        const response = await axiosInstance.post<Venta>('/Venta', {
          fecha: new Date().toISOString(),
          clienteId: venta.cliente?.id,
          cliente: venta.cliente,
          // Transmit items
          productos: venta.productos?.map(p => ({
            productoId: p.id,
            cantidad: p.cantidad,
            precioUnitario: p.precio
          })),
          total: venta.total
        });
        return response.data;
      }
    }
  }
};
