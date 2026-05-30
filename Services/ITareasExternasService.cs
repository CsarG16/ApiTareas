using ApiTareas.Models.DTOs;

namespace ApiTareas.Services
{
    public interface ITareasExternasService
    {
        Task<List<ExternalTodoDto>> GetAllAsync();
        Task<ExternalTodoDto?> GetByIdAsync(int id);
    }
}
