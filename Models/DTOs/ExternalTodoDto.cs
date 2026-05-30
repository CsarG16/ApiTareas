namespace ApiTareas.Models.DTOs
{
    public class ExternalTodoDto
    {
        public int ExternalId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public bool Completado { get; set; }
    }
}
