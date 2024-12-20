namespace CryptoexchangeMarketDepth.Services
{
    public class ApiOrder
    {
        public double Price { get; set; }
        public double Quantity { get; set; }
    }

    public class ApiOrderDTO
    {
        public double Price { get; set; }
        public double Amount { get; set; }
    }
}