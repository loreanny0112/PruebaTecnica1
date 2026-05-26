SISTEMA DE GESTIÓN DE VENTAS

Descripción
Este proyecto consiste en el desarrollo de una API REST para la gestión de productos, clientes y ventas. Permite realizar operaciones CRUD completas y gestionar relaciones entre entidades.

Tecnologías Utilizadas
- C#
- .NET 8
- Entity Framework Core
- SQL Server
- Swagger (para pruebas de API)

Arquitectura
El proyecto está estructurado en capas:

- Controllers: Manejo de las solicitudes HTTP
- Services: Lógica de negocio
- Repositories: Acceso a datos
- Models: Entidades del sistema

Funcionalidades

Productos
- Crear producto
- Listar productos
- Actualizar producto
- Eliminar producto

Clientes
- Crear cliente
- Listar clientes
- Actualizar cliente
- Eliminar cliente

Ventas
- Crear venta
- Listar ventas
- Actualizar venta
- Eliminar venta

Manejo de Errores

Se implementó manejo de errores mediante bloques try-catch en los controladores para garantizar respuestas claras ante fallos.

Base de Datos
- SQL Server
- Uso de Entity Framework Core con migraciones

