using PruebaTecnica.Repositories;
using PruebaTecnica.Models;

namespace PruebaTecnica.Services
{
    // Servicio encargado de manejar la lógica de negocio de las ventas
    // Actúa como intermediario entre el Controller y el Repository
    public class VentaService
    {
        private readonly VentaRepository _repository;

        public VentaService(VentaRepository repository)
        {
            _repository = repository;
        }

        // Obtiene todas las ventas
        public List<Venta> GetAll()
        {
            return _repository.GetAll();
        }

        // Agrega una nueva venta
        public void Add(Venta venta)
        {
            _repository.Add(venta);
        }

        // Actualiza una venta existente
        public void Update(Venta venta)
        {
            _repository.Update(venta);
        }

        // Elimina una venta x id
        public void Delete(int id)
        {
            _repository.Delete(id);
        }
    }
}
