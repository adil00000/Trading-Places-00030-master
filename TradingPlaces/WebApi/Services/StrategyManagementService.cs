using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TradingPlaces.Resources;
using TradingPlaces.WebApi.Dtos;
using System.Collections.Generic;
using Reutberg;

namespace TradingPlaces.WebApi.Services
{
    internal class StrategyManagementService : TradingPlacesBackgroundServiceBase, IStrategyManagementService
    {
        private const int TickFrequencyMilliseconds = 1000;
        ReutbergService reutbergService = new ReutbergService();
        private static readonly object _locker = new object();

        static private List<RegisteredStrategy> registeredStrategy;

        static public List<RegisteredStrategy> RegisteredStrategy
        {
            get { return registeredStrategy; }
            set { registeredStrategy = value; }
        }


        public StrategyManagementService(ILogger<StrategyManagementService> logger)
            : base(TimeSpan.FromMilliseconds(TickFrequencyMilliseconds), logger)
        {
            RegisteredStrategy = new List<RegisteredStrategy>();
        }


        public void ResgisterStrategy(StrategyDetailsDto strategyDetailsDto)
        {
            decimal? startingprice = null;

            try
            {
                startingprice = reutbergService.GetQuote(strategyDetailsDto.Ticker.ToUpper());
            }
            catch (Exception ex)
            {
                // log message for Ticker not being available - Report back to user
                startingprice = null;
            }

            finally
            {
                lock(_locker)
                {
                    // even if RegisteredPrice comes back as null, we still register it and 
                    // future enhancement - RegisteredPrice should get price when GetQuote becomes available
                    RegisteredStrategy.Add(new RegisteredStrategy()
                    {
                        strategyDetailsDto = strategyDetailsDto,
                        RegisteredPrice = startingprice
                    });
                }
            }
        }

        public void UnregisterStrategy(string id)
        {
            var strategy = RegisteredStrategy.Find(x => x.strategyDetailsDto.Instruction.Equals(id));

            if (strategy != null)
            {
                lock (_locker)
                {
                    RegisteredStrategy.Remove(strategy);
                }
            }
        }
        protected override Task CheckStrategies()
        {
            try
            {

                foreach (var strategy in RegisteredStrategy)
                {
                    if (!strategy.PurchasePrice.HasValue && strategy.RegisteredPrice != null && strategy.SettlementPrice != null)
                    {
                        var currentPrice = reutbergService.GetQuote(strategy.strategyDetailsDto.Ticker.ToUpper());

                        lock (_locker)
                        {
                                // BUY then it is assumed that the trader will buy only with a drop in price
                                // SELL then it is assumed that the trader will sell on with a price increase

                            if (strategy.strategyDetailsDto.Instruction.Equals(BuySell.Buy) && strategy.SettlementPrice.Value >= currentPrice) // we should buy now
                            {
                                strategy.PurchasePrice = reutbergService.Sell(strategy.strategyDetailsDto.Ticker, strategy.strategyDetailsDto.Quantity);
                            }
                            if (strategy.strategyDetailsDto.Instruction.Equals(BuySell.Sell) && strategy.SettlementPrice.Value <= currentPrice) // we should sell now
                            {
                                strategy.PurchasePrice = reutbergService.Buy(strategy.strategyDetailsDto.Ticker, strategy.strategyDetailsDto.Quantity);
                            }
                        }
                    }
                }

                lock (_locker)
                {
                    RegisteredStrategy.RemoveAll(x => x.PurchasePrice != null); // remove all strageties that have been purchased
                }

            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }

             return Task.CompletedTask;
        }
    }
}
