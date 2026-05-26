using PruebaTecnica.Data;
using PruebaTecnica.Models;
using Microsoft.EntityFrameworkCore;

namespace PruebaTecnica.Repositories
{
    // Repositorio encargado de acceder a la base de datos
    public class VentaRepository
    {
        private readonly AppDbContext _context;

        public VentaRepository(AppDbContext context)
        {
            _context = context;
        }
        // Obtener todas las ventas desde la base de datos
        public List<Venta> GetAll()
        {
            return _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Producto)
                .ToList();
        }

        // Método para agregar una nueva venta a la base de datos
        public void Add(Venta venta)
        {
            _context.Ventas.Add(venta);
            _context.SaveChanges();
        }

        // Método para actualizar una venta existente
        public void Update(Venta venta)
        {
            _context.Ventas.Update(venta);
            _context.SaveChanges();
        }

        // Método para eliminar una venta por su ID
        public void Delete(int id)
        {
            var venta = _context.Ventas.Find(id);

            if (venta != null)
            {
                _context.Ventas.Remove(venta);
                _context.SaveChanges();
            }
        }
    }
}
