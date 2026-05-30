using Microsoft.ML.Data;

namespace ApiTareas.Models.ML
{
    /// <summary>
    /// Clase de entrada para el pipeline de ML.NET.
    /// Representa una fila del dataset de entrenamiento.
    /// </summary>
    public class SentimentData
    {
        [LoadColumn(0)]
        public string? Comentario { get; set; }

        [LoadColumn(1), ColumnName("Label")]
        public bool Sentimiento { get; set; }
    }

    /// <summary>
    /// Clase de salida con la predicción del modelo.
    /// </summary>
    public class SentimentPrediction : SentimentData
    {
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }

        public float Probability { get; set; }

        public float Score { get; set; }
    }
}
