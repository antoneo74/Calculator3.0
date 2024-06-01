namespace Calculator3.Models
{
    public record ResponseCreditLine(int Id,
                                     string Date,
                                     double MonthlyPayment,
                                     double PrincipalPayment,
                                     double InterestPayment,
                                     double DebtBalance);
}
