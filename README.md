# Sistema de Gestión de Productos y Ventas

# Descripción

Este proyecto consiste en el desarrollo de una aplicación Full-Stack para la gestión de productos, clientes y ventas.

La aplicación permite realizar operaciones CRUD (Crear, Leer, Actualizar y Eliminar) sobre las entidades principales del sistema, integrando un backend desarrollado en .NET y un frontend en Vue.js.

# Tecnologías Utilizadas

# Backend
- C#
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
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
- CRUD de clientes

# Gestión de Ventas
- Registro de ventas
- Relación con clientes y productos

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

El proyecto backend está estructurado en capas:

- Controllers → Manejo de solicitudes HTTP
- Services → Lógica de negocio
- Repositories → Acceso a datos

Esto permite una mejor organización y mantenimiento del código.

# Cómo ejecutar el proyecto

# Backend (.NET)

1. Abrir el proyecto en Visual Studio
2. Ejecutar las migraciones:
bash
Add-Migration InitialCreate
Update-Database
3. Ejecutar la API
4. Acceder a
   
# Frontend (Vue)
1. Abrir el proyecto en VS Code
2. Instalar dependencias: npm install
3. Ejecutar: npm run dev
4. Acceder en:

# IMPORTANTE 

No realizado por falta de tiempo:
- JWT  
- pruebas unitarias 
- ventas completas UI 
