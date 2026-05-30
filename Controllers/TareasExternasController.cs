using ApiTareas.Models.DTOs;
using ApiTareas.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiTareas.Controllers
{
    [ApiController]
    [Route("api/tareas-externas")]
    public class TareasExternasController : ControllerBase
    {
        private readonly ITareasExternasService _service;

        public TareasExternasController(ITareasExternasService service)
        {
            _service = service;
        }

        // GET: api/tareas-externas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExternalTodoDto>>> GetTareasExternas()
        {
            try
            {
                var tareas = await _service.GetAllAsync();
                return Ok(tareas);
            }
            catch (HttpRequestException)
            {
                return StatusCode(503, new { mensaje = "No se pudo conectar con la API externa. Intente más tarde." });
            }
        }

        // GET: api/tareas-externas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExternalTodoDto>> GetTareaExterna(int id)
        {
            try
            {
                var tarea = await _service.GetByIdAsync(id);

                if (tarea == null)
                {
                    return NotFound(new { mensaje = $"No se encontró ninguna tarea externa con el ID {id}." });
                }

                return Ok(tarea);
            }
            catch (HttpRequestException)
            {
                return StatusCode(503, new { mensaje = "No se pudo conectar con la API externa. Intente más tarde." });
            }
        }
    }
}
