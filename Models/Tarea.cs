using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiTareas.Models
{
    public class Tarea : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        public string Titulo { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EstadoTarea Estado { get; set; }

        [Required(ErrorMessage = "La prioridad es obligatoria.")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PrioridadTarea Prioridad { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "La fecha de vencimiento es obligatoria.")]
        public DateTime FechaVencimiento { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Comparar solo la parte de la fecha (año, mes, día) para evitar problemas de minutos/segundos.
            if (FechaVencimiento.Date < DateTime.Today)
            {
                yield return new ValidationResult(
                    "La fecha de vencimiento no puede ser menor a la fecha actual.",
                    new[] { nameof(FechaVencimiento) }
                );
            }
        }
    }
}
