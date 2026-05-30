using System.Net.Http;
using System.Text.Json;
using ApiTareas.Models.DTOs;

namespace ApiTareas.Services
{
    public class TareasExternasService : ITareasExternasService
    {
        private readonly HttpClient _httpClient;

        public TareasExternasService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ExternalTodoDto>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync("/todos");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var todos = JsonSerializer.Deserialize<List<JsonPlaceholderTodoDto>>(json)
                ?? new List<JsonPlaceholderTodoDto>();

            // Mapear del DTO externo crudo a nuestro DTO propio
            var result = todos.Select(t => new ExternalTodoDto
            {
                ExternalId = t.Id,
                Titulo = t.Title,
                Completado = t.Completed
            }).ToList();

            return result;
        }

        public async Task<ExternalTodoDto?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/todos/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var todo = JsonSerializer.Deserialize<JsonPlaceholderTodoDto>(json);

            if (todo == null || todo.Id == 0)
            {
                return null;
            }

            // Mapear del DTO externo crudo a nuestro DTO propio
            return new ExternalTodoDto
            {
                ExternalId = todo.Id,
                Titulo = todo.Title,
                Completado = todo.Completed
            };
        }
    }
}
