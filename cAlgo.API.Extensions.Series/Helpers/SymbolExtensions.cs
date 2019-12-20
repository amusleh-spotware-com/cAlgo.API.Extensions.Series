using cAlgo.API.Internals;
using System;

namespace cAlgo.API.Extensions.Series.Helpers
{
    public static class SymbolExtensions
    {
        /// <summary>
        /// Returns a symbol pip value
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns>double</returns>
        public static double GetPip(this Symbol symbol)
        {
            return (symbol.TickSize / symbol.PipSize) * Math.Pow(10, symbol.Digits);
        }

        /// <summary>
        /// Returns a price value in terms of pips
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="price">The price level</param>
        /// <returns>double</returns>
        public static double ToPips(this Symbol symbol, double price)
        {
            return price * symbol.GetPip();
        }

        /// <summary>
        /// Returns a price value in terms of ticks
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="price">The price level</param>
        /// <returns>double</returns>
        public static double ToTicks(this Symbol symbol, double price)
        {
            return price * Math.Pow(10, symbol.Digits);
        }

        /// <summary>
        /// Rounds a price level to the number of symbol digits
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="price">The price level</param>
        /// <returns>double</returns>
        public static double Round(this Symbol symbol, double price)
        {
            return Math.Round(price, symbol.Digits);
        }
    }
}