namespace BotStock
{
    class Stock
    {
        public string Symbol { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public int Volume { get; set; }

        public override string ToString()
        {
            return this.Symbol + " quote is $" + this.Close + " per share.";
        }
    }
}