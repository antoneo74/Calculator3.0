using System;
using System.Collections.Generic;

namespace Calculator3.Models.Deposit
{
    internal class DepositRequest : IDataRequest
    {
        public double Amount { get; set; }
        public int Term { get; set; }
        public double Rate { get; set; }
        public Constants.Constants.CreditFrequency TimeUnit { get; set; }
        public DateTime Start { get; set; }
        public bool Capitalization { get; set; }
        public Constants.Constants.PaymentFrequency Frequency { get; set; }
        public List<Replenishment> Replenishments { get; set; } = null!;
    }
}
