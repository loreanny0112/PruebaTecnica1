using PruebaTecnica.Data;
using PruebaTecnica.Models;

namespace PruebaTecnica.Repositories
{
    // Repositorio encargado de acceder a la base de datos
    public class ClienteRepository
    {
        private readonly AppDbContext _context;

        public ClienteRepository(AppDbContext context)
        {
            _context = context;
        }

        // Obtener todos los clientes desde la base de datos
        public List<Cliente> GetAll()
        {
            return _context.Clientes.ToList();
        }

        // Método para agregar un cliente a la base de datos
        public void Add(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            _context.SaveChanges();
        }

        // Método para actualizar un cliente existente
        public void Update(Cliente cliente)
        {
            _context.Clientes.Update(cliente);
            _context.SaveChanges();
        }

        // Método para eliminar un cliente por su ID
        public void Delete(int id)
        {
            var cliente = _context.Clientes.Find(id);

            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
                _context.SaveChanges();
            }
        }
    }
}
