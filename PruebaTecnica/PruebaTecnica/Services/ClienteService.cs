using PruebaTecnica.Repositories;
using PruebaTecnica.Models;

namespace PruebaTecnica.Services
{
    // Servicio encargado de manejar la lógica de negocio de los clientes
    // Actúa como intermediario entre el Controller y el Repository
    public class ClienteService
    {
        private readonly ClienteRepository _repository;

        public ClienteService(ClienteRepository repository)
        {
            _repository = repository;
        }

        // Obtiene todas los clientes
        public List<Cliente> GetAll()
        {
            return _repository.GetAll();
        }

        //Agrega un cliente
        public void Add(Cliente cliente)
        {
            _repository.Add(cliente);
        }

        //Actualiza un cliente existente
        public void Update(Cliente cliente)
        {
            _repository.Update(cliente);
        }

        //Elimina un cliente x su id
        public void Delete(int id)
        {
           _repository.Delete(id);
        }
    }
}
