using Microsoft.AspNetCore.Mvc;
using PruebaTecnica.Services;
using PruebaTecnica.Models;

namespace PruebaTecnica.Controllers
{
    // Controlador para gestionar las operaciones de ventas
    [ApiController]
    [Route("api/[controller]")]
    public class VentaController : ControllerBase
    {
        private readonly VentaService _service;

        // Constructor que recibe el servicio
        public VentaController(VentaService service)
        {
            _service = service;
        }

        //Obtener todas las ventas
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

        //Agregar ventas
        [HttpPost]
        public IActionResult Post([FromBody] Venta venta)
        {
            try
            {
                _service.Add(venta);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Editar ventas
        [HttpPut]
        public IActionResult Put([FromBody] Venta venta)
        {
            try
            {
                _service.Update(venta);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Eliminar ventas
        [HttpDelete("{id}")]
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
