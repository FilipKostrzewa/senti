namespace Senti.Web.Views.Correlation3;

using System;
using System.Linq;

public class CorrelationCalculator
{
    public static double Calculate(int[] quotes, double[] values)
    {
        var interpolated = Interpolate(values, quotes.Length);
        var corr = CalculateCorrelation(quotes, interpolated);

        return corr;
    }

    private static double[] Interpolate(double[] quotes, int newLength)
    {
        var result = new double[newLength];
        int oldLength = quotes.Length;
        for (int i = 0; i < newLength; i++)
        {
            double position = (double)i / (newLength - 1) * (oldLength - 1);
            int leftIndex = (int)Math.Floor(position);
            int rightIndex = (int)Math.Ceiling(position);
            if (leftIndex == rightIndex)
            {
                result[i] = quotes[leftIndex];
            }
            else
            {
                double weight = position - leftIndex;
                double interpolated = (double)quotes[leftIndex] * (1 - weight) + (double)quotes[rightIndex] * weight;
                result[i] = interpolated;
            }
        }
        return result.ToArray();
    }

    private static double CalculateCorrelation(int[] quotes, double[] values)
    {
        if (quotes.Length != values.Length)
            throw new ArgumentException("Array length");

        int n = quotes.Length;
        double avgX = (double)quotes.Average();
        double avgY = values.Average();

        double sumXY = 0, sumX2 = 0, sumY2 = 0;

        for (int i = 0; i < n; i++)
        {
            double dx = quotes[i] - avgX;
            double dy = values[i] - avgY;
            sumXY += dx * dy;
            sumX2 += dx * dx;
            sumY2 += dy * dy;
        }

        double denominator = Math.Sqrt(sumX2 * sumY2);
        return denominator == 0 ? 0 : sumXY / denominator;
    }

    //static void Main()
    //{
    //    // Przykładowe dane – zastąp swoimi
    //    var sentymentPozytywny = Enumerable.Range(0, 100).Select(i => Math.Sin(i * 0.1)).ToList();
    //    var sentymentNegatywny = Enumerable.Range(0, 100).Select(i => Math.Cos(i * 0.1)).ToList();
    //    var sentymentNeutralny = Enumerable.Range(0, 100).Select(i => 0.5).ToList();

    //    var cenyAkcji = Enumerable.Range(0, 60).Select(i => 100 + 10 * Math.Sin(i * 0.15)).ToList();

    //    // Interpolujemy ceny akcji do długości 100
    //    var cenyAkcjiInterpolowane = Interpolate(cenyAkcji, sentymentPozytywny.Count);

    //    // Liczymy korelacje
    //    double corrPos = PearsonCorrelation(sentymentPozytywny, cenyAkcjiInterpolowane);
    //    double corrNeg = PearsonCorrelation(sentymentNegatywny, cenyAkcjiInterpolowane);
    //    double corrNeu = PearsonCorrelation(sentymentNeutralny, cenyAkcjiInterpolowane);

    //    // Wyniki
    //    Console.WriteLine($"Korelacja (Pozytywny, Ceny): {corrPos:F4}");
    //    Console.WriteLine($"Korelacja (Negatywny, Ceny): {corrNeg:F4}");
    //    Console.WriteLine($"Korelacja (Neutralny, Ceny): {corrNeu:F4}");
    //}
}
