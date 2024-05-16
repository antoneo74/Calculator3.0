namespace Calculator3.Models
{
    internal interface IDataRequest
    {
        public double Amount { get; set; }
        public int Term { get; set; }
        public double Rate { get; set; }
        public Constants.Constants.CreditFrequency TimeUnit { get; set; }
    }
}
