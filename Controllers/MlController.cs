using ApiTareas.Models.DTOs;
using ApiTareas.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiTareas.Controllers
{
    [ApiController]
    [Route("api/ml")]
    public class MlController : ControllerBase
    {
        private readonly IMlService _mlService;

        public MlController(IMlService mlService)
        {
            _mlService = mlService;
        }

        // POST: api/ml/sentimiento
        [HttpPost("sentimiento")]
        public ActionResult<SentimientoResponse> AnalizarSentimiento([FromBody] SentimientoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Comentario))
            {
                return BadRequest(new { mensaje = "El comentario es obligatorio." });
            }

            var resultado = _mlService.AnalizarSentimiento(request);
            return Ok(resultado);
        }
    }
}
