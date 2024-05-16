using System.Collections.Generic;

namespace Calculator3.Models.Credit
{
    internal class CreditResponse : IDataResponse
    {
        public string MonthlyPayment { get; set; } = string.Empty;
        public double AccuredInterest { get; set; }
        public double Total { get; set; }
        public List<ResponseCreditLine> ResponseCreditLines { get; set; } = null!;
    }
}
