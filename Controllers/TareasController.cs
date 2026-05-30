using ApiTareas.Data;
using ApiTareas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiTareas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TareasController : ControllerBase
    {
        private readonly TareasDbContext _context;

        public TareasController(TareasDbContext context)
        {
            _context = context;
        }

        // GET: api/tareas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tarea>>> GetTareas(
            [FromQuery] string? estado,
            [FromQuery] string? prioridad,
            [FromQuery] DateTime? fechaInicio,
            [FromQuery] DateTime? fechaFin)
        {
            var query = _context.Tareas.AsQueryable();

            // Validación y filtrado por Estado
            if (!string.IsNullOrEmpty(estado))
            {
                if (!System.Enum.TryParse<EstadoTarea>(estado, true, out var estadoEnum))
                {
                    return BadRequest(new { mensaje = $"El estado '{estado}' no es válido. Valores permitidos: Pendiente, EnProceso, Completada." });
                }
                query = query.Where(t => t.Estado == estadoEnum);
            }

            // Validación y filtrado por Prioridad
            if (!string.IsNullOrEmpty(prioridad))
            {
                if (!System.Enum.TryParse<PrioridadTarea>(prioridad, true, out var prioridadEnum))
                {
                    return BadRequest(new { mensaje = $"La prioridad '{prioridad}' no es válida. Valores permitidos: Baja, Media, Alta." });
                }
                query = query.Where(t => t.Prioridad == prioridadEnum);
            }

            // Validación y filtrado por Rango de Fechas
            if (fechaInicio.HasValue && fechaFin.HasValue && fechaInicio.Value > fechaFin.Value)
            {
                return BadRequest(new { mensaje = "La fechaInicio no puede ser mayor que la fechaFin." });
            }

            if (fechaInicio.HasValue)
            {
                query = query.Where(t => t.FechaVencimiento.Date >= fechaInicio.Value.Date);
            }

            if (fechaFin.HasValue)
            {
                query = query.Where(t => t.FechaVencimiento.Date <= fechaFin.Value.Date);
            }

            var tareas = await query.ToListAsync();
            return Ok(tareas);
        }

        // GET: api/tareas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tarea>> GetTarea(int id)
        {
            var tarea = await _context.Tareas.FindAsync(id);

            if (tarea == null)
            {
                return NotFound(new { mensaje = $"No se encontró ninguna tarea con el ID {id}." });
            }

            return Ok(tarea);
        }

        // POST: api/tareas
        [HttpPost]
        public async Task<ActionResult<Tarea>> PostTarea([FromBody] Tarea tarea)
        {
            // La validación del modelo (título, prioridad, estado obligatorios, fecha vencimiento >= hoy) 
            // se maneja automáticamente por el atributo [ApiController] y IValidatableObject.
            
            _context.Tareas.Add(tarea);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTarea), new { id = tarea.Id }, tarea);
        }

        // PUT: api/tareas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTarea(int id, [FromBody] Tarea tareaModificada)
        {
            if (id != tareaModificada.Id)
            {
                return BadRequest(new { mensaje = "El ID de la ruta no coincide con el ID de la tarea." });
            }

            var tareaExistente = await _context.Tareas.FindAsync(id);
            if (tareaExistente == null)
            {
                return NotFound(new { mensaje = $"No se encontró ninguna tarea con el ID {id}." });
            }

            // Actualizar campos
            tareaExistente.Titulo = tareaModificada.Titulo;
            tareaExistente.Descripcion = tareaModificada.Descripcion;
            tareaExistente.Estado = tareaModificada.Estado;
            tareaExistente.Prioridad = tareaModificada.Prioridad;
            tareaExistente.FechaVencimiento = tareaModificada.FechaVencimiento;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TareaExists(id))
                {
                    return NotFound(new { mensaje = $"No se encontró ninguna tarea con el ID {id}." });
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/tareas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTarea(int id)
        {
            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null)
            {
                return NotFound(new { mensaje = $"No se encontró ninguna tarea con el ID {id}." });
            }

            _context.Tareas.Remove(tarea);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TareaExists(int id)
        {
            return _context.Tareas.Any(e => e.Id == id);
        }
    }
}
