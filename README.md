# cAlgo.API.Extensions.Series

You can use this library to back test your cTrader cBots on Renko/Range charts inside cTrader back tester.

By using this library you easily create any symbol Renko/Range bars data with few lines of code.


# Getting start

1. Download the latest version of library DLL file from Github repositoy [releases](https://github.com/afhacker/cAlgo.API.Extensions.Series/releases) page
2. Reference the downloaded DLL file on your cBot via cTrader automate Reference manager or Visual Studio references
3. Use the sample below to implement it on your cBot code
4. For backtesting you have to use Tick data, it will not work on bars data

Sample cBot:

```csharp

using cAlgo.API;
using cAlgo.API.Extensions.Enums;
using cAlgo.API.Extensions.Models;
using cAlgo.API.Extensions.Series;
using cAlgo.API.Internals;

namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.FullAccess)]
    public class RenkoBot : Robot
    {
        // Use RangeBars for Range bars
        private RenkoBars _renkoBars;
        // Rernko bar/box size in Pips
        [Parameter("Size (Pips)", DefaultValue = 10)]
        public double RankoSizeInPips { get; set; }

        protected override void OnStart()
        {
            // You can pass any symbol and it will create its Renko/Range bars for you
            _renkoBars = new RenkoBars(RankoSizeInPips, Symbol, this);

            _renkoBars.OnBar += RenkoBars_OnBar;
        }

        // This method is called on new Renko/Range bars, you should use this method instead of OnBar method of your Robot
        // The old bar is the latest completed/closed Renko/Range bar and the new bar it the current open bar
        private void RenkoBars_OnBar(object sender, OhlcBar newBar, OhlcBar oldBar)
        {
            var positionsToClose = Positions.FindAll("RenkoBot");

            if (oldBar.Type == BarType.Bullish)
            {
                foreach (var position in positionsToClose)
                {
                    if (position.TradeType == TradeType.Buy) continue;

                    ClosePosition(position);
                }

                ExecuteMarketOrder(TradeType.Buy, Symbol.Name, 1000, "RenkoBot", 10, 20);
            }
            else if (oldBar.Type == BarType.Bearish)
            {
                foreach (var position in positionsToClose)
                {
                    if (position.TradeType == TradeType.Sell) continue;

                    ClosePosition(position);
                }

                ExecuteMarketOrder(TradeType.Sell, Symbol.Name, 1000, "RenkoBot", 10, 20);
            }
        }
    }
}

```
