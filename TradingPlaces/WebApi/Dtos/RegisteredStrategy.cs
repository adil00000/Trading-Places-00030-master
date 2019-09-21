using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradingPlaces.Resources;

namespace TradingPlaces.WebApi.Dtos
{
    public class RegisteredStrategy
    {
        public StrategyDetailsDto strategyDetailsDto { get; set; }
        private decimal? registeredPrice;
        public decimal? RegisteredPrice
        {
            get { return registeredPrice; }
            set
            {
                registeredPrice = value;

                if (registeredPrice.HasValue)
                {
                    switch (strategyDetailsDto.Instruction)
                    {
                        case BuySell.Buy:
                            {
                                SettlementPrice = registeredPrice - ((strategyDetailsDto.PriceMovement / 100) * registeredPrice);
                                break;
                            }
                        case BuySell.Sell:
                            {
                                SettlementPrice = registeredPrice + ((strategyDetailsDto.PriceMovement / 100) * registeredPrice);
                                break;
                            }
                        default:
                            {
                                SettlementPrice = null;
                                break;
                            }
                    }
                }
                
            }
        }
        public decimal? SettlementPrice { get; set; }
        public decimal? PurchasePrice { get; set; }


    }
}
