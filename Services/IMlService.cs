using ApiTareas.Models.DTOs;

namespace ApiTareas.Services
{
    public interface IMlService
    {
        SentimientoResponse AnalizarSentimiento(SentimientoRequest request);
    }
}
