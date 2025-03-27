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

    public static double Calculate(int[] quotes, int[] values)
    {
        var interpolated = Interpolate(values, quotes.Length);
        var corr = CalculateCorrelation(quotes, interpolated);

        return corr;
    }

    private static double[] Interpolate(int[] quotes, int newLength)
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
}
