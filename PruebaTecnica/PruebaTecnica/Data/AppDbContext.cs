using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Models;

namespace PruebaTecnica.Data
{
    // Contexto de base de datos que gestiona la conexión con SQL Server
    //y permite acceder a las tablas mediante Entity Framework
    public class AppDbContext : DbContext
    {
        // Constructor que recibe la configuración de la base de datos
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        {         
        }
        // Representación de las tablas Productos, Clientes y Vnetas en la base de datos
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Venta> Ventas { get; set; }


    }
}
