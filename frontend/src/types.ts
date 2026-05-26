export interface Producto {
  id?: number;
  nombre: string;
  descripcion: string;
  precio: number;
  stock: number;
}

export interface Cliente {
  id?: number;
  nombre: string;
  email: string;
  telefono: string;
}

export interface ProductoVendido {
  id: number;
  nombre: string;
  precio: number;
  cantidad: number;
}

export interface Venta {
  id?: number;
  fecha: string;
  cliente: Cliente;
  productos: ProductoVendido[];
  total: number;
}

export interface User {
  username: string;
  token?: string;
}
