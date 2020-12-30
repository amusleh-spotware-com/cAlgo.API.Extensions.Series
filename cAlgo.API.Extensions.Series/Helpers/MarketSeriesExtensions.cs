using cAlgo.API.Extensions.Enums;
using cAlgo.API.Extensions.Models;
using cAlgo.API.Internals;
using System;

namespace cAlgo.API.Extensions.Series.Helpers
{
    public static class MarketSeriesExtensions
    {
        /// <summary>
        /// Returns the last bar index in a market series
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <returns>int</returns>
        public static int GetIndex(this Bars marketSeries)
        {
            return marketSeries.ClosePrices.Count > 0 ? marketSeries.ClosePrices.Count - 1 : marketSeries.ClosePrices.Count;
        }

        /// <summary>
        /// Returns the bar type
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="index">Index of bar</param>
        /// <returns>BarType</returns>
        public static BarType GetBarType(this Bars marketSeries, int index)
        {
            if (marketSeries.ClosePrices[index] > marketSeries.OpenPrices[index])
            {
                return BarType.Bullish;
            }
            else if (marketSeries.ClosePrices[index] < marketSeries.OpenPrices[index])
            {
                return BarType.Bearish;
            }
            else
            {
                return BarType.Neutral;
            }
        }

        /// <summary>
        /// Returns the range of a bar in a market series
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="index">Bar index in market series</param>
        /// <param name="useBarBody">Use bar open and close price instead of high and low?</param>
        /// <returns>double</returns>
        public static double GetBarRange(this Bars marketSeries, int index, bool useBarBody = false)
        {
            return useBarBody ? Math.Abs(marketSeries.OpenPrices[index] - marketSeries.ClosePrices[index])
                : marketSeries.HighPrices[index] - marketSeries.LowPrices[index];
        }

        /// <summary>
        /// Returns a market series specific data series based on provided series type
        /// </summary>
        /// <param name="marketSeries">The market series</param>
        /// <param name="seriesType">Series type</param>
        /// <returns>DataSeries</returns>
        public static DataSeries GetSeries(this Bars marketSeries, SeriesType seriesType)
        {
            switch (seriesType)
            {
                case SeriesType.Open:
                    return marketSeries.OpenPrices;

                case SeriesType.High:
                    return marketSeries.HighPrices;

                case SeriesType.Low:
                    return marketSeries.LowPrices;

                case SeriesType.Close:
                    return marketSeries.ClosePrices;

                case SeriesType.Median:
                    return marketSeries.MedianPrices;

                case SeriesType.TickVolume:
                    return marketSeries.TickVolumes;

                case SeriesType.Typical:
                    return marketSeries.TypicalPrices;

                case SeriesType.WeightedClose:
                    return marketSeries.WeightedPrices;

                default:
                    return null;
            }
        }

        /// <summary>
        /// Returns a bar object from a market series
        /// </summary>
        /// <param name="marketSeries">Market series</param>
        /// <param name="index">The bar index in market series</param>
        /// <returns>Bar</returns>
        public static OhlcBar GetBar(this Bars marketSeries, int index)
        {
            var result = marketSeries.ClosePrices.Count > index ? new OhlcBar
            {
                Index = index,
                Time = marketSeries.OpenTimes[index],
                Open = marketSeries.OpenPrices[index],
                High = marketSeries.HighPrices[index],
                Low = marketSeries.LowPrices[index],
                Close = marketSeries.ClosePrices[index],
                Volume = marketSeries.TickVolumes[index],
            } : null;

            return result;
        }
    }
}