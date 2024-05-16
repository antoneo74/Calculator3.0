using System;

namespace Calculator3.Models
{
    public record ResponseDepositLine(
        DateTime Date,
        double Percent,
        double ChangeBalance,
        double Balance);
}
