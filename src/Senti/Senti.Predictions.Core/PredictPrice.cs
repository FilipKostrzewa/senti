namespace Senti.Predictions.Core;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using Newtonsoft.Json;

public class TimeSeriesPrediction
{
    [ColumnName("Score")]
    public float ForecastedValue { get; set; }
}

public class TimeSeriesData
{
    public float Value { get; set; }
}

public class AzureFunctionTimeSeries
{
    private static readonly string ModelPath = "MLModels/time_series_model.zip";
    private static MLContext _mlContext;
    private static PredictionEngine<TimeSeriesData, TimeSeriesPrediction> _predictionEngine;

    static AzureFunctionTimeSeries()
    {
        _mlContext = new MLContext();
        LoadModel();
    }

    private static void LoadModel()
    {
        DataViewSchema modelSchema;
        var trainedModel = _mlContext.Model.Load(ModelPath, out modelSchema);
        _predictionEngine = _mlContext.Model.CreatePredictionEngine<TimeSeriesData, TimeSeriesPrediction>(trainedModel);
    }

    //[Function("PredictTimeSeries")]
    //public async Task<IActionResult> Run(
    //    [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
    //    ILogger log)
    public async Task Run()
    {

        //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        //var inputData = JsonConvert.DeserializeObject<List<float>>(requestBody);

        // Apply Lag-Llama for feature engineering
        //var processedData = ApplyLagFeatures(inputData);

        //var predictions = processedData
        //    .Select(data => _predictionEngine.Predict(data).ForecastedValue)
        //    .ToList();
    }

    //private List<TimeSeriesData> ApplyLagFeatures(List<float> rawData)
    //{
    //    //var lagLlama = new LagLlamaProcessor();
    //    //List<TimeSeriesData> processedData = new List<TimeSeriesData>();

    //    //foreach (var value in lagLlama.ApplyLags(rawData, 3)) // Example with 3 lags
    //    //{
    //    //    processedData.Add(new TimeSeriesData { Value = value });
    //    //}
    //    //return processedData;
    //}
}

