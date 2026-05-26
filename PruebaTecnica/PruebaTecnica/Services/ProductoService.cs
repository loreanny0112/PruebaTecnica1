using PruebaTecnica.Repositories;
using PruebaTecnica.Models;

namespace PruebaTecnica.Services
{
    // Servicio encargado de manejar la lógica de negocio de los productos
    // Actúa como intermediario entre el Controller y el Repository
    public class ProductoService
    {
        private readonly ProductoRepository _repository;

        public ProductoService(ProductoRepository repository)
        {
            _repository = repository;
        }
        // Obtiene todas los productos
        public List<Producto> GetAll()
        {
            return _repository.GetAll();
        }
        // Agrega un nuevo producto
        public void Add(Producto producto)
        {
            _repository.Add(producto);
        }

        //Actualiza un producto
        public void Update(Producto producto)
        {
            _repository.Update(producto);
        }

        //Elimina el producto x su id
        public void Delete(int id)
        {
            _repository.Delete(id);
        }
    }
}
