
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.TimeSeries;
using Microsoft.ML.Transforms.TimeSeries;

class Program
{
    static void Main()
    {
        var mlContext = new MLContext();

        var data = new List<ModelInput>
        {
            new ModelInput { Day = 1, Price = 100 },
            new ModelInput { Day = 2, Price = 101 },
            new ModelInput { Day = 3, Price = 102 },
            new ModelInput { Day = 4, Price = 103 },
            new ModelInput { Day = 5, Price = 104 },
            new ModelInput { Day = 6, Price = 105 }
        };

        var dataView = mlContext.Data.LoadFromEnumerable(data);

        var pipeline = mlContext.Forecasting.ForecastBySsa(
            outputColumnName: "ForecastedPrice",
            inputColumnName: "Price",
            windowSize: 3,
            seriesLength: 6,
            trainSize: 5,
            horizon: 3,
            confidenceLevel: 0.95f,
            confidenceLowerBoundColumn: "LowerBoundPrice",
            confidenceUpperBoundColumn: "UpperBoundPrice"
        );

        var transformer = pipeline.Fit(dataView);

        var forecastEngine = transformer.CreateTimeSeriesEngine<ModelInput, ModelForecast>(mlContext);

        var predictions = forecastEngine.Predict();

        for (int i = 0; i < predictions.ForecastedPrice.Length; i++)
        {
            Console.WriteLine($"Predicted Price for Day {data.Count + i + 1}: {predictions.ForecastedPrice[i]}");
        }
    }
}

public class ModelInput
{
    public float Day { get; set; }
    public float Price { get; set; }
}

public class ModelForecast
{
    public float[] ForecastedPrice { get; set; }
    public float[] LowerBoundPrice { get; set; }
    public float[] UpperBoundPrice { get; set; }
}

