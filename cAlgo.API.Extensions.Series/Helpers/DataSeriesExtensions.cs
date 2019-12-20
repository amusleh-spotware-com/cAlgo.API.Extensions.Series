using System;

namespace cAlgo.API.Extensions.Series.Helpers
{
    public static class DataSeriesExtensions
    {
        /// <summary>
        /// Returns the maximum value between start and end (inclusive) index in a dataseries
        /// </summary>
        /// <param name="dataSeries"></param>
        /// <param name="startIndex">Start index (Ex: 1)</param>
        /// <param name="endIndex">End index (Ex: 10)</param>
        /// <returns>double</returns>
        public static double Maximum(this DataSeries dataSeries, int startIndex, int endIndex)
        {
            double max = double.NegativeInfinity;

            for (int i = startIndex; i <= endIndex; i++)
            {
                max = Math.Max(dataSeries[i], max);
            }

            return max;
        }

        /// <summary>
        /// Returns the minimum value between start and end (inclusive) index in a dataseries
        /// </summary>
        /// <param name="dataSeries"></param>
        /// <param name="startIndex">Start index (Ex: 1)</param>
        /// <param name="endIndex">End index (Ex: 10)</param>
        /// <returns>double</returns>
        public static double Minimum(this DataSeries dataSeries, int startIndex, int endIndex)
        {
            double min = double.PositiveInfinity;

            for (int i = startIndex; i <= endIndex; i++)
            {
                min = Math.Min(dataSeries[i], min);
            }

            return min;
        }
    }
}