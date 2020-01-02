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
        public static int GetIndex(this MarketSeries marketSeries)
        {
            return marketSeries.Close.Count > 0 ? marketSeries.Close.Count - 1 : marketSeries.Close.Count;
        }

        /// <summary>
        /// Returns the bar type
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="index">Index of bar</param>
        /// <returns>BarType</returns>
        public static BarType GetBarType(this MarketSeries marketSeries, int index)
        {
            if (marketSeries.Close[index] > marketSeries.Open[index])
            {
                return BarType.Bullish;
            }
            else if (marketSeries.Close[index] < marketSeries.Open[index])
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
        public static double GetBarRange(this MarketSeries marketSeries, int index, bool useBarBody = false)
        {
            return useBarBody ? Math.Abs(marketSeries.Open[index] - marketSeries.Close[index])
                : marketSeries.High[index] - marketSeries.Low[index];
        }

        /// <summary>
        /// Returns a market series specific data series based on provided series type
        /// </summary>
        /// <param name="marketSeries">The market series</param>
        /// <param name="seriesType">Series type</param>
        /// <returns>DataSeries</returns>
        public static DataSeries GetSeries(this MarketSeries marketSeries, SeriesType seriesType)
        {
            switch (seriesType)
            {
                case SeriesType.Open:
                    return marketSeries.Open;

                case SeriesType.High:
                    return marketSeries.High;

                case SeriesType.Low:
                    return marketSeries.Low;

                case SeriesType.Close:
                    return marketSeries.Close;

                case SeriesType.Median:
                    return marketSeries.Median;

                case SeriesType.TickVolume:
                    return marketSeries.TickVolume;

                case SeriesType.Typical:
                    return marketSeries.Typical;

                case SeriesType.WeightedClose:
                    return marketSeries.WeightedClose;

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
        public static OhlcBar GetBar(this MarketSeries marketSeries, int index)
        {
            var result = marketSeries.Close.Count > index ? new OhlcBar
            {
                Index = index,
                Time = marketSeries.OpenTime[index],
                Open = marketSeries.Open[index],
                High = marketSeries.High[index],
                Low = marketSeries.Low[index],
                Close = marketSeries.Close[index],
                Volume = marketSeries.TickVolume[index],
            } : null;

            return result;
        }
    }
}