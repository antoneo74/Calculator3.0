using System.Collections.Generic;

namespace Calculator3.Models.Deposit
{
    internal class DepositResponse : IDataResponse
    {
        public double AccuredInterest { get; set; }
        public double Total { get; set; }
        public double Tax { get; set; }
        public List<ResponseDepositLine> Lines { get; set; } = null!;
    }
}
