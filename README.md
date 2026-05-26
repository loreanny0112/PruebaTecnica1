# Sistema de Gestión de Productos y Ventas

# Descripción

Este proyecto consiste en el desarrollo de una aplicación Full-Stack para la gestión de productos, clientes y ventas, permitiendo administrar de manera eficiente las operaciones básicas de un negocio.

La aplicación permite realizar operaciones CRUD (Crear, Leer, Actualizar y Eliminar) sobre las entidades principales del sistema, integrando un backend desarrollado en .NET y un frontend en Vue.js.

# Tecnologías Utilizadas

# Backend
- C#
- ASP.NET Core Web API
- Entity Framework Core (Code First)
- SQL Server (mediante migraciones)
- Arquitectura en capas (Controllers, Services, Repositories)

# Frontend
- Vue.js (Composition API)
- Axios
- HTML, CSS

# Funcionalidades Implementadas

# Gestión de Productos
- Crear productos
- Listar productos
- Editar productos
- Eliminar productos

# Gestión de Clientes
- Crear clientes
- Listar clientes
- Editar clientes
- Eliminar clientes

# Gestión de Ventas
- Registro de ventas
- Asociación de productos y clientes
- Generación de factura al realizar una venta
- Cálculo automático del total

# Historial de Ventas
- Visualización de todas las ventas realizadas
- Consulta de información relevante de cada transacción

# Autenticación

Se implementó un sistema básico de autenticación utilizando JWT (JSON Web Token) para validar el acceso al sistema.

# Comunicación Frontend - Backend

El frontend consume la API REST mediante Axios, realizando solicitudes HTTP:

- GET → Obtener datos
- POST → Crear registros
- PUT → Actualizar registros
- DELETE → Eliminar registros

# Base de Datos

Se utilizó SQL Server junto con Entity Framework Core bajo el enfoque Code First.

Las entidades principales son:

- Productos
- Clientes
- Ventas

Se aplicaron migraciones para la creación de la base de datos.

# Arquitectura

El backend sigue una arquitectura en capas:

- Controllers → Manejo de endpoints
- Services → Lógica de negocio
- Repositories → Acceso a datos

Esto permite una mejor organización, mantenimiento y escalabilidad del sistema.

# Cómo ejecutar el proyecto

# Backend (.NET)

1. Abrir el proyecto en **Visual Studio**
2. Seleccionar el proyecto como inicio (Startup Project)
3. Ejecutar la API presionando el botón verde (IIS Express)
4. Verificar que la API esté corriendo en Swagger:

https://localhost:44314/swagger/index.html

# Frontend (Vue)
1. Abrir la carpeta **frontend** en Visual Studio Code
2. Abrar la terminal dentro de esa carpeta
3. Instalar dependencias (solo la primera vez): npm install
4. Ejecutar el proyecto: npm run dev
5. Acceder en el navegador: http://localhost:3000/

# Nota Importante

- El backend debe estar corriendo antes de iniciar el frontend
- El frontend consume la API mediante Axios usando la URL local del backend

# IMPORTANTE 

# Consideraciones Finales

Debido a limitaciones de tiempo, hay aspectos que pueden ser mejorados en futuras versiones:

- No se implementaron pruebas unitarias
- Aplicación parcial de principios SOLID
- La autenticación JWT es básica y puede ser fortalecida
