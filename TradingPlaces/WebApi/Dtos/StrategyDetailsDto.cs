using TradingPlaces.Resources;

namespace TradingPlaces.WebApi.Dtos
{
    public class StrategyDetailsDto
    {
        public string Ticker { get; set; }
        public BuySell Instruction { get; set; }
        public decimal PriceMovement { get; set; }
        public int Quantity { get; set; }
    }
}