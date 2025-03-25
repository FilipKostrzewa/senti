namespace Senti.Web.Views.Correlation2;

using System;
using System.Collections.Generic;
using System.Linq;

public class CorrelationCalculator
{
    static void Main()
    {
        var dict1 = new Dictionary<int, int>
        {
            { 1, 10 },
            { 2, 20 },
            { 3, 30 },
            { 4, 40 }
        };

        var dict2 = new Dictionary<int, int>
        {
            { 101, 3 },
            { 102, 6 },
            { 103, 9 },
            { 104, 12 },
            { 105, 15 },
            { 106, 18 }
        };

        // Wyciągamy wartości i dopasowujemy długości
        var values1 = dict1.Values.ToArray();
        var values2 = dict2.Values.ToArray();

        int length = Math.Min(values1.Length, values2.Length);

        var x = values1.Take(length).Select(v => (double)v).ToArray();
        var y = values2.Take(length).Select(v => (double)v).ToArray();

        double correlation = CalculatePearsonCorrelation(x, y);
        Console.WriteLine($"Korelacja: {correlation}");
    }

    static double CalculatePearsonCorrelation(double[] x, double[] y)
    {
        int n = x.Length;
        if (n != y.Length || n == 0)
            throw new ArgumentException("arrays");

        double meanX = x.Average();
        double meanY = y.Average();

        double numerator = 0;
        double sumSqX = 0;
        double sumSqY = 0;

        for (int i = 0; i < n; i++)
        {
            double dx = x[i] - meanX;
            double dy = y[i] - meanY;

            numerator += dx * dy;
            sumSqX += dx * dx;
            sumSqY += dy * dy;
        }

        double denominator = Math.Sqrt(sumSqX * sumSqY);
        if (denominator == 0)
            return 0;

        return numerator / denominator;
    }
}

