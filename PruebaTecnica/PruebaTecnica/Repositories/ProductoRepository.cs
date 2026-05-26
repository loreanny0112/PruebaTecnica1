using PruebaTecnica.Data;
using PruebaTecnica.Models;

namespace PruebaTecnica.Repositories
{
    // Repositorio encargado de acceder a la base de datos
    public class ProductoRepository
    {
        private readonly AppDbContext _context;

        public ProductoRepository(AppDbContext context)

        {
            _context = context;
        }
        // Obtener todos los productos desde la base de datos
        public List<Producto> GetAll()
        {
            return _context.Productos.ToList();
        }
        // Método para agregar un producto a la base de datos
        public void Add(Producto producto)
        {
            _context.Productos.Add(producto);
            _context.SaveChanges();
        }

        // Método para actualizar un producto existente
        public void Update(Producto producto)
        {
            _context.Productos.Update(producto);
            _context.SaveChanges();
        }

        // Método para eliminar un producto por su ID
        public void Delete(int id)
        {
            var producto = _context.Productos.Find(id);

            if (producto != null)
            {
                _context.Productos.Remove(producto);
                _context.SaveChanges();
            }
        }
    }

    
}
