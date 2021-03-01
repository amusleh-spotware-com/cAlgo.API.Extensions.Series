using cAlgo.API.Extensions.Enums;
using cAlgo.API.Extensions.Models;
using cAlgo.API.Extensions.Series.Helpers;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace cAlgo.API.Extensions.Series
{
    public class IndicatorBars : Bars
    {
        #region Fields

        private readonly Algo _algo;

        private readonly IndicatorDataSeries _openPrices;

        private readonly IndicatorDataSeries _closePrices;

        private readonly IndicatorDataSeries _highPrices;

        private readonly IndicatorDataSeries _lowPrices;

        private readonly IndicatorDataSeries _tickVolumes;

        private readonly IndicatorDataSeries _medianPrices;

        private readonly IndicatorDataSeries _typicalPrices;

        private readonly IndicatorDataSeries _weightedPrices;

        private readonly TimeSeries _openTimes;

        private readonly Dictionary<int, Bar> _bars = new Dictionary<int, Bar>();

        private readonly TimeFrame _timeFrame;

        private readonly string _symbolName;

        public event Action<BarsHistoryLoadedEventArgs> HistoryLoaded;

        public event Action<BarsHistoryLoadedEventArgs> Reloaded;

        public event Action<BarsTickEventArgs> Tick;

        public event Action<BarOpenedEventArgs> BarOpened;

        #endregion Fields

        #region Constructor

        public IndicatorBars(TimeFrame timeFrame, string symbolName)
        {
            _openPrices = new CustomDataSeries();
            _closePrices = new CustomDataSeries();
            _highPrices = new CustomDataSeries();
            _lowPrices = new CustomDataSeries();
            _tickVolumes = new CustomDataSeries();
            _medianPrices = new CustomDataSeries();
            _typicalPrices = new CustomDataSeries();
            _weightedPrices = new CustomDataSeries();

            _openTimes = new IndicatorTimeSeries();

            _timeFrame = timeFrame;

            _symbolName = symbolName;
        }

        public IndicatorBars(TimeFrame timeFrame, string symbolName, Algo algo) : this(timeFrame, symbolName, new IndicatorTimeSeries(), algo)
        {
        }

        public IndicatorBars(TimeFrame timeFrame, string symbolName, TimeSeries timeSeries, Algo algo)
        {
            _algo = algo;

            _openPrices = algo.CreateDataSeries();
            _closePrices = algo.CreateDataSeries();
            _highPrices = algo.CreateDataSeries();
            _lowPrices = algo.CreateDataSeries();
            _tickVolumes = algo.CreateDataSeries();
            _medianPrices = algo.CreateDataSeries();
            _typicalPrices = algo.CreateDataSeries();
            _weightedPrices = algo.CreateDataSeries();

            _openTimes = timeSeries;

            _timeFrame = timeFrame;

            _symbolName = symbolName;
        }

        #endregion Constructor

        #region Properties

        protected Algo Algo
        {
            get
            {
                return _algo;
            }
        }

        public DataSeries OpenPrices
        {
            get
            {
                return _openPrices;
            }
        }

        public DataSeries HighPrices
        {
            get
            {
                return _highPrices;
            }
        }

        public DataSeries LowPrices
        {
            get
            {
                return _lowPrices;
            }
        }

        public DataSeries ClosePrices
        {
            get
            {
                return _closePrices;
            }
        }

        public DataSeries TickVolumes
        {
            get
            {
                return _tickVolumes;
            }
        }

        public DataSeries MedianPrices
        {
            get
            {
                return _medianPrices;
            }
        }

        public DataSeries TypicalPrices
        {
            get
            {
                return _typicalPrices;
            }
        }

        public DataSeries WeightedPrices
        {
            get
            {
                return _weightedPrices;
            }
        }

        public TimeSeries OpenTimes
        {
            get
            {
                return _openTimes;
            }
        }

        public TimeFrame TimeFrame
        {
            get
            {
                return _timeFrame;
            }
        }

        public int Index
        {
            get
            {
                return ClosePrices.Count - 1;
            }
        }

        public string SymbolName
        {
            get
            {
                return _symbolName;
            }
        }

        public string SymbolCode
        {
            get
            {
                return SymbolName;
            }
        }

        public Bar LastBar => _bars[_bars.Keys.Max()];

        public int Count => ClosePrices.Count;

        public Bar this[int index] => _bars[index];

        #endregion Properties

        #region Methods

        public void Insert(int index, double value, SeriesType seriesType)
        {
            ((IndicatorDataSeries)this.GetSeries(seriesType))[index] = value;
        }

        public void Insert(OhlcBar bar)
        {
            Insert(bar.Index, bar.Open, bar.High, bar.Low, bar.Close, bar.Volume, bar.Time);
        }

        public void Insert(int index, double open, double high, double low, double close, double volume, DateTime openTime)
        {
            Insert(index, open, SeriesType.Open);
            Insert(index, high, SeriesType.High);
            Insert(index, low, SeriesType.Low);
            Insert(index, close, SeriesType.Close);
            Insert(index, volume, SeriesType.TickVolume);

            if (_openTimes is IndicatorTimeSeries)
            {
                (_openTimes as IndicatorTimeSeries).Insert(index, openTime);
            }
        }

        public void CalculateHeikenAshi(Bars marketSeries, int index, int periods = 1)
        {
            int seriesIndex = marketSeries.OpenTimes.GetIndexByTime(marketSeries.OpenTimes[index]);

            double barOhlcSum = marketSeries.OpenPrices[seriesIndex] + marketSeries.LowPrices[seriesIndex] +
                marketSeries.HighPrices[seriesIndex] + marketSeries.ClosePrices[seriesIndex];

            _closePrices[seriesIndex] = _algo.Symbol.Round(barOhlcSum / 4);

            if (OpenPrices.Count < periods || double.IsNaN(_openPrices[seriesIndex - 1]))
            {
                _openPrices[seriesIndex] = _algo.Symbol.Round((marketSeries.OpenPrices[seriesIndex] + marketSeries.ClosePrices[seriesIndex]) / 2);
                _highPrices[seriesIndex] = marketSeries.HighPrices[seriesIndex];
                _lowPrices[seriesIndex] = marketSeries.LowPrices[seriesIndex];
            }
            else
            {
                _openPrices[seriesIndex] = _algo.Symbol.Round((_openPrices[seriesIndex - periods] + _closePrices[seriesIndex - periods]) / 2);
                _highPrices[seriesIndex] = Math.Max(marketSeries.HighPrices[seriesIndex], Math.Max(_openPrices[seriesIndex], _closePrices[seriesIndex]));
                _lowPrices[seriesIndex] = Math.Min(marketSeries.LowPrices[seriesIndex], Math.Min(_openPrices[seriesIndex], _closePrices[seriesIndex]));
            }
        }

        public void CalculateHeikenAshi(Bars marketSeries, int index, int maPeriods, MovingAverageType maType, int periods = 1)
        {
            int seriesIndex = marketSeries.OpenTimes.GetIndexByTime(marketSeries.OpenTimes[index]);

            if (seriesIndex <= maPeriods)
            {
                return;
            }

            double barMaOpen = GetSeriesMovingAverageValue(marketSeries, SeriesType.Open, maPeriods, maType, seriesIndex);
            double barMaHigh = GetSeriesMovingAverageValue(marketSeries, SeriesType.High, maPeriods, maType, seriesIndex);
            double barMaLow = GetSeriesMovingAverageValue(marketSeries, SeriesType.Low, maPeriods, maType, seriesIndex);
            double barMaClose = GetSeriesMovingAverageValue(marketSeries, SeriesType.Close, maPeriods, maType, seriesIndex);

            _closePrices[seriesIndex] = (barMaOpen + barMaClose + barMaHigh + barMaLow) / 4;

            if (seriesIndex < periods || double.IsNaN(_openPrices[seriesIndex - 1]))
            {
                _openPrices[seriesIndex] = (barMaOpen + barMaClose) / 2;
                _highPrices[seriesIndex] = barMaHigh;
                _lowPrices[seriesIndex] = barMaLow;
            }
            else
            {
                _openPrices[seriesIndex] = (_openPrices[seriesIndex - periods] + _closePrices[seriesIndex - periods]) / 2;
                _highPrices[seriesIndex] = Math.Max(barMaHigh, Math.Max(_openPrices[seriesIndex], _closePrices[seriesIndex]));
                _lowPrices[seriesIndex] = Math.Min(barMaLow, Math.Min(_openPrices[seriesIndex], _closePrices[seriesIndex]));
            }
        }

        public double GetSeriesMovingAverageValue(
            Bars marketSeries, SeriesType seriesType, int periods, MovingAverageType type, int index)
        {
            DataSeries series = marketSeries.GetSeries(seriesType);

            MovingAverage ma = _algo.Indicators.MovingAverage(series, periods, type);

            return ma.Result[index];
        }

        public Bar Last(int index) => _bars[_bars.Keys.Max() - index];

        public int LoadMoreHistory()
        {
            throw new NotImplementedException();
        }

        public void LoadMoreHistoryAsync()
        {
            throw new NotImplementedException();
        }

        public void LoadMoreHistoryAsync(Action<BarsHistoryLoadedEventArgs> callback)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<Bar> GetEnumerator() => _bars.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion Methods
    }
}