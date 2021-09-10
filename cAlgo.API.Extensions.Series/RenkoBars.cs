using cAlgo.API.Extensions.Enums;
using cAlgo.API.Extensions.Models;
using cAlgo.API.Extensions.Series.Helpers;
using cAlgo.API.Internals;
using System;

namespace cAlgo.API.Extensions.Series
{
    public class RenkoBars : IndicatorBars
    {
        private readonly Symbol _symbol;
        private readonly decimal _size;
        private readonly decimal _doubleSize;

        private double _lastPrice;

        public RenkoBars(double sizeInPips, Symbol symbol, Algo algo) : base(TimeFrame.Minute, symbol.Name, new IndicatorTimeSeries(), algo)
        {
            _symbol = symbol;

            _symbol.Tick += Symbol_Tick;

            _size = Convert.ToDecimal(sizeInPips * _symbol.PipSize);

            _doubleSize = _size * 2;
        }

        public event OnBarHandler OnBar;

        private void Symbol_Tick(SymbolTickEventArgs obj)
        {
            double price = _symbol.Bid;

            if (price == _lastPrice || (_lastPrice == 0 && Convert.ToDecimal(price) % _size > 0)) return;

            if (_lastPrice == 0)
            {
                Insert(0, price, price, price, price, 0, Algo.Server.TimeInUtc);
            }

            _lastPrice = price;

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

            decimal range = Math.Abs(Convert.ToDecimal(OpenPrices[Index]) - Convert.ToDecimal(ClosePrices[Index]));

            if ((range >= _size && (Index == 0 || this.GetBarType(Index) == this.GetBarType(Index - 1))) || (range >= _doubleSize))
            {
                var currentBar = this.GetBar(Index);

                if (Index > 0)
                {
                    var previousBar = this.GetBar(Index - 1);

                    if (currentBar.Type == previousBar.Type)
                    {
                        Insert(Index, currentBar.Type == BarType.Bullish ? currentBar.Open + decimal.ToDouble(_size) : currentBar.Open - decimal.ToDouble(_size), SeriesType.Close);
                    }
                    else
                    {
                        Insert(Index, previousBar.Open, SeriesType.Open);

                        Insert(Index, currentBar.Type == BarType.Bullish ? currentBar.Open + decimal.ToDouble(_doubleSize) : currentBar.Open - decimal.ToDouble(_doubleSize), SeriesType.Close);
                    }
                }
                else
                {
                    Insert(Index, currentBar.Type == BarType.Bullish ? currentBar.Open + decimal.ToDouble(_size) : currentBar.Open - decimal.ToDouble(_size), SeriesType.Close);
                }

                OhlcBar newBar = new OhlcBar
                {
                    Index = Index + 1,
                    Open = ClosePrices[Index],
                    High = ClosePrices[Index],
                    Low = ClosePrices[Index],
                    Close = ClosePrices[Index],
                    Volume = 0,
                    Time = Algo.Server.TimeInUtc
                };

                Insert(newBar);

                OnBar?.Invoke(this, newBar, this.GetBar(Index - 1));
            }
        }
    }
}