using System.Text.Json.Serialization;

namespace ApiTareas.Models.DTOs
{
    /// <summary>
    /// Clase interna para deserializar la respuesta cruda de jsonplaceholder.
    /// No se expone directamente al cliente.
    /// </summary>
    public class JsonPlaceholderTodoDto
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("completed")]
        public bool Completed { get; set; }
    }
}
