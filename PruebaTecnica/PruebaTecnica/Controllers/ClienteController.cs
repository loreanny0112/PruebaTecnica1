using Microsoft.AspNetCore.Mvc;
using PruebaTecnica.Services;
using PruebaTecnica.Models;

namespace PruebaTecnica.Controllers
{
    // Controlador para gestionar las operaciones del cliente
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly ClienteService _service;

        // Constructor que recibe el servicio
        public ClienteController(ClienteService service)
        {
            _service = service;
        }

        //Obtener clientes
        [HttpGet]
        public IActionResult Get()
        {
            try 
            { 
                return Ok(_service.GetAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Agregar un nuevo cliente
        [HttpPost]
        public IActionResult Post([FromBody] Cliente cliente)
        {
            try
            {
                _service.Add(cliente);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Editar un cliente
        [HttpPut]
        public IActionResult Put([FromBody] Cliente cliente)
        {
            try
            {
                _service.Update(cliente);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Eliminar un cliente
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _service.Delete(id);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

