using Microsoft.ML;
using ApiTareas.Models.DTOs;
using ApiTareas.Models.ML;

namespace ApiTareas.Services
{
    public class MlService : IMlService
    {
        private readonly PredictionEngine<SentimentData, SentimentPrediction> _predictionEngine;

        public MlService(IWebHostEnvironment env)
        {
            var mlContext = new MLContext(seed: 0);

            // Cargar el dataset CSV desde la carpeta Data del proyecto
            var dataPath = Path.Combine(env.ContentRootPath, "Data", "sentiment_dataset.csv");

            IDataView dataView = mlContext.Data.LoadFromTextFile<SentimentData>(
                path: dataPath,
                hasHeader: true,
                separatorChar: ',');

            // Construir el pipeline de entrenamiento
            var pipeline = mlContext.Transforms.Text
                .FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(SentimentData.Comentario))
                .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                    labelColumnName: "Label",
                    featureColumnName: "Features"));

            // Entrenar el modelo
            var model = pipeline.Fit(dataView);

            // Crear el motor de predicción
            _predictionEngine = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
        }

        public SentimientoResponse AnalizarSentimiento(SentimientoRequest request)
        {
            var input = new SentimentData { Comentario = request.Comentario };
            var prediction = _predictionEngine.Predict(input);

            return new SentimientoResponse
            {
                Comentario = request.Comentario,
                Sentimiento = prediction.Prediction ? "Positivo" : "Negativo"
            };
        }
    }
}
