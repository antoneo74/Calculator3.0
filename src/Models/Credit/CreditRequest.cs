namespace Calculator3.Models.Credit
{
    internal class CreditRequest : IDataRequest
    {
        public double Amount { get; set; }
        public int Term { get; set; }
        public double Rate { get; set; }
        public Constants.Constants.CreditFrequency TimeUnit { get; set; }
        public bool Annuitet { get; set; }
    }
}
