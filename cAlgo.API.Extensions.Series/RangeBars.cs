using cAlgo.API.Extensions.Enums;
using cAlgo.API.Extensions.Models;
using cAlgo.API.Extensions.Series.Helpers;
using cAlgo.API.Internals;
using System;

namespace cAlgo.API.Extensions.Series
{
    public class RangeBars : IndicatorBars
    {
        private readonly Symbol _symbol;

        private readonly double _size;

        private double _previousBidPrice;

        public RangeBars(double sizeInPips, Symbol symbol, Algo algo) : base(TimeFrame.Minute, symbol.Name, new IndicatorTimeSeries(), algo)
        {
            _symbol = symbol;

            _symbol.Tick += Symbol_Tick;

            _size = sizeInPips * _symbol.PipSize;
        }

        public event OnBarHandler OnBar;

        private void Symbol_Tick(SymbolTickEventArgs obj)
        {
            double price = _symbol.Bid;

            if (price == _previousBidPrice || (Count == 0 && price % _size > 0)) return;

            _previousBidPrice = price;

            if (double.IsNaN(OpenPrices.LastValue))
            {
                Insert(0, price, price, price, price, 0, Algo.Server.TimeInUtc);
            }

            double range = Math.Abs(this.GetBarRange(Index, true));

            if (range >= _size)
            {
                OhlcBar bar = new OhlcBar
                {
                    Index = Index + 1,
                    Open = ClosePrices[Index],
                    High = ClosePrices[Index],
                    Low = ClosePrices[Index],
                    Close = ClosePrices[Index],
                    Volume = 0,
                    Time = Algo.Server.TimeInUtc
                };

                Insert(bar);

                OnBar?.Invoke(this, bar, this.GetBar(Index - 1));
            }

            Insert(Index, TickVolumes.LastValue + 1, SeriesType.TickVolume);

            Insert(Index, price, SeriesType.Close);

            if (price > HighPrices[Index])
            {
                Insert(Index, price, SeriesType.High);
            }

            if (price < LowPrices[Index])
            {
                Insert(Index, price, SeriesType.Low);
            }
        }
    }
}