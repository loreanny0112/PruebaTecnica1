using Microsoft.AspNetCore.Mvc;
using PruebaTecnica.Services;
using PruebaTecnica.Models;

namespace PruebaTecnica.Controllers
{
    // Controlador para gestionar los productos
    [ApiController]
    [Route("api/[controller]")]
    public class ProductoController : ControllerBase
    {
        private readonly ProductoService _service;

        // Constructor que recibe el servicio
        public ProductoController(ProductoService service)
        {
            _service = service;
        }

        //Obtener todas los productos
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

        //Agregar nuevos productos
        [HttpPost]
        public IActionResult Post([FromBody] Producto producto)
        {
            try
            {
                _service.Add(producto);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Editar productos
        [HttpPut]
        public IActionResult Put([FromBody] Producto producto)
        {
            try
            {
                _service.Update(producto);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Eliminar productos
        [HttpDelete ("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _service.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
    
}
