using System.Globalization;
using Calculator3.Models.Calculator;
using Calculator3.Models.Credit;
using Calculator3.Models.Deposit;
using Calculator3.Services;

namespace CalculatorTests
{
    public class UnitTest1
    {
        CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture("en-US");
        double exp = 0.02;

        IntPtr obj = LibraryImport.Constructor();

        string str = string.Empty;

        LibraryImport.Result result;

        [Fact]
        public void Give_CreditRequest_When_AnnuitetTrue_Then_ReturnCreditResponse()
        {
            var request = new CreditRequest()
            {
                Amount = 55000,
                Term = 50,
                Rate = 17.8,
                TimeUnit = Calculator3.Constants.Constants.TimeFrequency.Months,
                Annuitet = true
            };
            CreditCalculator creditCalculator = new();

            var resp = (CreditResponse)creditCalculator.Calculate(request);

            double.TryParse(resp.MonthlyPayment, NumberStyles.Any, cultureInfo, out double result);
            Assert.NotNull(resp);
            Assert.InRange(result, 1565.66, 1565.66 + exp);
            Assert.InRange(resp.AccuredInterest, 23283, 23283 + exp);
            Assert.InRange(resp.Total, 78283, 78283 + exp);
            Assert.Equal(50, resp.ResponseCreditLines.Count);
            DateTime nextMonth = DateTime.Now.AddMonths(50);
            Assert.Equal($"{nextMonth:Y}", resp.ResponseCreditLines[49].Date);
            Assert.InRange(resp.ResponseCreditLines[49].PrincipalPayment, 1542.77, 1542.77 + exp);
            Assert.InRange(resp.ResponseCreditLines[49].InterestPayment, 22.89, 22.89 + exp);
            Assert.InRange(resp.ResponseCreditLines[49].DebtBalance, 0.12, 0.12 + exp);
        }

        [Fact]
        public void Give_CreditRequest_When_AnnuitetFalse_Then_ReturnCreditResponse()
        {
            var request = new CreditRequest()
            {
                Amount = 55000,
                Term = 3,
                Rate = 16.45,
                TimeUnit = Calculator3.Constants.Constants.TimeFrequency.Years,
                Annuitet = false
            };
            CreditCalculator creditCalculator = new();

            var resp = (CreditResponse)creditCalculator.Calculate(request);

            Assert.NotNull(resp);
            Assert.InRange(resp.AccuredInterest, 13948.23, 13948.23 + exp);
            Assert.InRange(resp.Total, 68948.23, 68948.23 + exp);
            Assert.Equal(36, resp.ResponseCreditLines.Count);
            Assert.InRange(resp.ResponseCreditLines[32].MonthlyPayment, 1611.55, 1611.55 + exp);
            Assert.InRange(resp.ResponseCreditLines[32].PrincipalPayment, 1527.78, 1527.78 + exp);
            Assert.InRange(resp.ResponseCreditLines[32].InterestPayment, 83.77, 83.77 + exp);
            Assert.InRange(resp.ResponseCreditLines[32].DebtBalance, 4583.33, 4583.33 + exp);
        }

        [Fact]
        public void Give_DepositRequest_When_CapitalizationTrue_Then_ReturnDepositResponse()
        {
            var request = new DepositRequest()
            {
                Amount = 1500000,
                Term = 27,
                Rate = 12.7,
                TimeUnit = Calculator3.Constants.Constants.TimeFrequency.Months,
                Start = new DateTime(2024, 05, 20),
                Capitalization = true,
                Frequency = Calculator3.Constants.Constants.PaymentFrequency.Quarterly,
                Replenishments = new List<Calculator3.Models.Replenishment>()
            };
            DepositCalculator depositCalculator = new();

            var resp = (DepositResponse)depositCalculator.Calculate(request);

            Assert.NotNull(resp);
            Assert.NotEmpty(resp.Lines);
            Assert.InRange(resp.AccuredInterest, 487380.56, 487380.56 + exp);
            Assert.InRange(resp.Total, 1987380.56, 1987380.56 + exp);
            Assert.Equal(9, resp.Lines.Count);
            Assert.InRange(resp.Tax, 9110.58, 9110.58 + exp);

            var date = new DateTime(2024, 05, 20);
            Assert.Equal(date.AddMonths(18), resp.Lines[5].Date);
            Assert.InRange(resp.Lines[5].Percent, 56141.24, 56141.24 + exp);
            Assert.InRange(resp.Lines[5].Balance, 1809954.28, 1809954.28 + exp);
        }

        [Fact]
        public void Give_DepositRequest_When_CapitalizationFalse_Then_ReturnDepositResponse()
        {
            var request = new DepositRequest()
            {
                Amount = 1500000,
                Term = 718,
                Rate = 24.3,
                TimeUnit = Calculator3.Constants.Constants.TimeFrequency.Days,
                Start = new DateTime(2024, 05, 22),
                Capitalization = false,
                Frequency = Calculator3.Constants.Constants.PaymentFrequency.Monthly,
                Replenishments = new List<Calculator3.Models.Replenishment>()
                {
                    new(0, new DateTime(2025, 01, 06), 260000),
                    new(1, new DateTime(2025, 08, 13), 700000),
                }
            };
            DepositCalculator depositCalculator = new();

            var resp = (DepositResponse)depositCalculator.Calculate(request);

            Assert.NotNull(resp);
            Assert.NotEmpty(resp.Lines);
            Assert.InRange(resp.AccuredInterest, 675224.5, 675224.51);
            Assert.InRange(resp.Total, 1060000, 1060000 + exp);
            Assert.Equal(26, resp.Lines.Count);
            Assert.InRange(resp.Tax, 33427.21, 33427.21 + exp);
            Assert.InRange(resp.Lines[10].Percent, 32808.33, 32808.33 + exp);
            Assert.InRange(resp.Lines[10].ChangeBalance, -32808.33, -32808.33 + exp);
        }

        [Fact]
        public void Give_DepositRequest_When_CapitalizationFalse_Then_ReturnDepositResponse1()
        {
            var request = new DepositRequest()
            {
                Amount = 1500000,
                Term = 38,
                Rate = 14.5,
                TimeUnit = Calculator3.Constants.Constants.TimeFrequency.Months,
                Start = new DateTime(2024, 05, 22),
                Capitalization = false,
                Frequency = Calculator3.Constants.Constants.PaymentFrequency.End,
                Replenishments = new List<Calculator3.Models.Replenishment>()
                {
                    new(1, new DateTime(2024, 05, 23), 1700000)
                }
            };
            DepositCalculator depositCalculator = new();

            var resp = (DepositResponse)depositCalculator.Calculate(request);

            Assert.NotNull(resp);
            Assert.NotEmpty(resp.Lines);
            Assert.InRange(resp.AccuredInterest, 688486.25, 688486.25 + exp);
            Assert.InRange(resp.Total, 1500000, 1500000 + exp);
            Assert.Equal(1, resp.Lines.Count);
            Assert.InRange(resp.Tax, 68703.21, 68703.21 + exp);
        }

        [Fact]
        public void Give_DepositRequest_When_CapitalizationTrue_Then_ReturnDepositResponse2()
        {
            var request = new DepositRequest()
            {
                Amount = 1500000,
                Term = 38,
                Rate = 14.5,
                TimeUnit = Calculator3.Constants.Constants.TimeFrequency.Months,
                Start = new DateTime(2024, 05, 22),
                Capitalization = true,
                Frequency = Calculator3.Constants.Constants.PaymentFrequency.Yearly,
                Replenishments = new List<Calculator3.Models.Replenishment>()
                {
                    new(1, new DateTime(2024, 12, 13), 400000),
                    new(0, new DateTime(2024, 12, 23), 400000),
                }
            };
            DepositCalculator depositCalculator = new();

            var resp = (DepositResponse)depositCalculator.Calculate(request);

            Assert.NotNull(resp);
            Assert.NotEmpty(resp.Lines);
            Assert.InRange(resp.AccuredInterest, 803634.8, 803634.8 + exp);
            Assert.InRange(resp.Total, 2303634.8, 2303634.8 + exp);
            Assert.Equal(6, resp.Lines.Count);
            Assert.InRange(resp.Tax, 42072.52, 42072.52 + exp);
        }

        [Fact]
        public void Give_DepositRequest_When_CapitalizationTrueWithReplenishments_Then_ReturnDepositResponse()
        {
            var request = new DepositRequest()
            {
                Amount = 1500000,
                Term = 4,
                Rate = 18.9,
                TimeUnit = Calculator3.Constants.Constants.TimeFrequency.Years,
                Start = new DateTime(2024, 05, 22),
                Capitalization = true,
                Frequency = Calculator3.Constants.Constants.PaymentFrequency.Daily,
                Replenishments = new List<Calculator3.Models.Replenishment>()
                {
                    new(0, new DateTime(2024, 12, 30), 260000),
                    new(0, new DateTime(2025, 01, 06), 260000),
                    new(1, new DateTime(2025, 01, 17), 700000),
                    new(1, new DateTime(2025, 08, 13), 700000),
                }
            };
            DepositCalculator depositCalculator = new();

            var resp = (DepositResponse)depositCalculator.Calculate(request);

            Assert.NotNull(resp);
            Assert.NotEmpty(resp.Lines);
            Assert.InRange(resp.AccuredInterest, 1060374.7, 1060374.7 + exp);
            Assert.InRange(resp.Total, 1680374.7, 1680374.7 + exp);
            Assert.InRange(resp.Tax, 39102.59, 39102.59 + exp);
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test1()
        {
            str = "2+3";
            result = LibraryImport.Calculate(obj, str, 0);
            Assert.Equal(5, result.res);
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test2()
        {
            str = "-5";
            result = LibraryImport.Calculate(obj, str, 0);
            Assert.Equal(-5, result.res);
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test3()
        {
            str = "+8";
            result = LibraryImport.Calculate(obj, str, 0);
            Assert.Equal(8, result.res);
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test4()
        {
            str = "(-(-(-5)))*(-(-5))";
            result = LibraryImport.Calculate(obj, str, 0);
            Assert.Equal(-25, result.res);
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test5()
        {
            str = "(-(-(-5)))";
            result = LibraryImport.Calculate(obj, str, 0);
            Assert.Equal(-5, result.res);
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test6()
        {
            str = "2+X";
            result = LibraryImport.Calculate(obj, str, 2);
            Assert.Equal(4, result.res);
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test7()
        {
            str = "2+4*5^2-4/2-5mod2";
            result = LibraryImport.Calculate(obj, str, 0);
            Assert.Equal(99, result.res);
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test8()
        {
            str = "(-2^(3+4*5)*(2*2)+2+6/3)";
            result = LibraryImport.Calculate(obj, str, 0);
            var expected = (Math.Pow(-2, (3 + 4 * 5)) * (2 * 2) + 2 + 6 / 3);
            Assert.Equal(expected, result.res);
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test9()
        {
            str = "(-816.484*5.2422)51.81(4849.3*401.9244)";
            result = LibraryImport.Calculate(obj, str, 0);
            var expected = (-816.484 * 5.2422) * 51.81 * (4849.3 * 401.9244);
            Assert.Equal(expected, result.res);
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test10()
        {
            str = "(-0.38227*7856.815-0.1)*(7759.3*(-51507.96))";
            result = LibraryImport.Calculate(obj, str, 0);
            var expected = (-0.38227 * 7856.815 - 0.1) * (7759.3 * (-51507.96));
            Assert.Equal(expected, result.res);
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test11()
        {
            str = "-25.9655*(-(-626.93*508.657)*(85.108*400.162))";
            result = LibraryImport.Calculate(obj, str, 0);
            var expected = -25.9655 * (-(-626.93 * 508.657) * (85.108 * 400.162));
            Assert.Equal(expected, result.res);
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test12()
        {
            str = "-(-356.081*4598.63)803.928(70.592*0.1569)*-36.1566";
            result = LibraryImport.Calculate(obj, str, 0);
            var expected = -(-356.081 * 4598.63) * 803.928 * (70.592 * 0.1569) * -36.1566;
            Assert.Equal(expected, result.res);
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test13()
        {
            str = "(sin(1)+cos(4))*tan(1)-acos(1)+asin(1)*atan(1)";
            result = LibraryImport.Calculate(obj, str, 0);
            var expected =
                (Math.Sin(1) + Math.Cos(4)) * Math.Tan(1)
                - Math.Acos(1)
                + Math.Asin(1) * Math.Atan(1);
            Assert.Equal(expected, result.res);
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test14()
        {
            str = "2^(2^3)";
            result = LibraryImport.Calculate(obj, str, 0);
            var expected = (Math.Pow(2, Math.Pow(2, 3)));
            Assert.Equal(expected, result.res);
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test15()
        {
            str = "17mod(863*(-173))";
            result = LibraryImport.Calculate(obj, str, 0);
            Math.DivRem(17, 863 * (-173), out int expected);
            Assert.Equal(expected, result.res);
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test16()
        {
            str = "sqrt(9)+log(10)-ln(10)";
            result = LibraryImport.Calculate(obj, str, 0);
            var expected = Math.Sqrt(9) + Math.Log10(10) - Math.Log(10);
            Assert.Equal(expected, result.res);
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test17()
        {
            str = "19+3^2*((4*2+5-1)-(14+4*4)/(4+2*3))";
            result = LibraryImport.Calculate(obj, str, 0);
            Assert.Equal(100, result.res);
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test18()
        {
            str =
                "((5.6- 8.9 + 18 * 5 / 3 ^ 4 ^ 0.5) mod 2.8 - 2.5 ) / 7 * 2 ^ (15 mod 4) + 9 + 16/10";
            result = LibraryImport.Calculate(obj, str, 0);
            Assert.Equal(9, result.res);
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test19()
        {
            str = "(tan(sin( log( sqrt9 * sqrt9 ) ) ) + cos(ln(10))+1)";
            result = LibraryImport.Calculate(obj, str, 0);
            var expected = (
                Math.Tan(Math.Sin(Math.Log10(Math.Sqrt(9) * Math.Sqrt(9))))
                + Math.Cos(Math.Log(10))
                + 1
            );
            Assert.Equal(expected, result.res);
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test20()
        {
            str = "atan(1) + asin(1) + acos(-1)";
            result = LibraryImport.Calculate(obj, str, 0);
            var expected = Math.Atan(1) + Math.Asin(1) + Math.Acos(-1);
            Assert.Equal(expected, result.res);
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test21()
        {
            str = "(X+X-3)*X+X*X";
            result = LibraryImport.Calculate(obj, str, 5);
            var expected = (5 + 5 - 3) * 5 + 5 * 5;
            Assert.Equal(expected, result.res);
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test22()
        {
            str = "asin(sin(4))+atan(cos(56))*acos(tan(3))-(24mod25+ln(2))";
            result = LibraryImport.Calculate(obj, str, 0);
            var expected =
                Math.Asin(Math.Sin(4))
                + Math.Atan(Math.Cos(56)) * Math.Acos(Math.Tan(3))
                - ((24 % 25) + Math.Log(2));
            Assert.Equal(Math.Round(expected, 8), Math.Round(result.res, 8));
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test23()
        {
            str = "-3.4*4+(656.5334-0.0000543)/-4";
            result = LibraryImport.Calculate(obj, str, 0);
            var expected = -3.4 * 4 + (656.5334 - 0.0000543) / -4;
            Assert.Equal(Math.Round(expected, 8), Math.Round(result.res, 8));
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test24()
        {
            str = "-(asin(+0.3465346)/2)";
            result = LibraryImport.Calculate(obj, str, 0);
            var expected = -(Math.Asin(+0.3465346) / 2);
            Assert.Equal(Math.Round(expected, 8), Math.Round(result.res, 8));
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test25()
        {
            str = "-sin(13.4+atan(7)*56.4-17/4)*(cos(tan(2^4)))";
            result = LibraryImport.Calculate(obj, str, 0);
            var expected =
                -Math.Sin(13.4 + Math.Atan(7) * 56.4 - 17 / 4.0)
                * (Math.Cos(Math.Tan(Math.Pow(2, 4))));
            Assert.Equal(Math.Round(expected, 8), Math.Round(result.res, 8));
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test26()
        {
            str = "-sin(13.4+atan(7)*56.4-17/4)*(cos(tan(2^4)))";
            result = LibraryImport.Calculate(obj, str, 0);
            var expected =
                -Math.Sin(13.4 + Math.Atan(7) * 56.4 - 17 / 4.0)
                * (Math.Cos(Math.Tan(Math.Pow(2, 4))));
            Assert.Equal(Math.Round(expected, 8), Math.Round(result.res, 8));
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test27()
        {
            str = "(1-3).2";
            result = LibraryImport.Calculate(obj, str, 0);
            var expected = (1 - 3) * 0.2;
            Assert.Equal(Math.Round(expected, 8), Math.Round(result.res, 8));
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test28()
        {
            str = "X2-1";
            result = LibraryImport.Calculate(obj, str, 2);
            Assert.Equal(3, result.res);
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test29()
        {
            str = "3+XX";
            result = LibraryImport.Calculate(obj, str, 2);
            Assert.Equal(7, result.res);
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test30()
        {
            str = "atan(cos(3.1415))+(+sqrt(36)-X)2";
            result = LibraryImport.Calculate(obj, str, -5.05);
            var expected = Math.Atan(Math.Cos(3.1415)) + (+Math.Sqrt(36) + 5.05) * 2;
            Assert.Equal(Math.Round(expected, 8), Math.Round(result.res, 8));
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test31()
        {
            str = "1.14^96.88";
            result = LibraryImport.Calculate(obj, str, 0);
            var expected = Math.Pow(1.14, 96.88);
            Assert.Equal(Math.Round(expected, 8), Math.Round(result.res, 8));
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test32()
        {
            str = "sin(cos(sin(cos(sqrt(3.1415)))))";
            result = LibraryImport.Calculate(obj, str, 0);
            var expected = Math.Sin(Math.Cos(Math.Sin(Math.Cos(Math.Sqrt(3.1415)))));
            Assert.Equal(Math.Round(expected, 8), Math.Round(result.res, 8));
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test33()
        {
            str = "ln(log(acos(asin(tan(sin(cos(tan(1))))))))";
            result = LibraryImport.Calculate(obj, str, 0);
            var expected = Math.Log(
                Math.Log10(Math.Acos(Math.Asin(Math.Tan(Math.Sin(Math.Cos(Math.Tan(1)))))))
            );
            Assert.Equal(Math.Round(expected, 8), Math.Round(result.res, 8));
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test34()
        {
            str = "sin(cos(23/454))+tan(23)+asin(0.2525)+acos(0.5353453)+atan(0.3453455)";
            result = LibraryImport.Calculate(obj, str, 0);
            Assert.Equal(4.0225954, Math.Round(result.res, 7));
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test35()
        {
            str = "2/-5+2*-5+2-5+2mod-5";
            result = LibraryImport.Calculate(obj, str, 0);
            Assert.Equal(-11.4, Math.Round(result.res, 7));
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test36()
        {
            str = "(15/(7-(1+1))*3-(2+(1+1-1+1*2/2))+15/(7-(1+1))*3-(2+(1+1+1-1*2/2)))";
            result = LibraryImport.Calculate(obj, str, 0);
            Assert.Equal(10, Math.Round(result.res, 7));
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test37()
        {
            str = "acos(2*X)";
            result = LibraryImport.Calculate(obj, str, 0.0019);
            Assert.Equal(1.5669963, Math.Round(result.res, 7));
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test38()
        {
            str = "()";
            result = LibraryImport.Calculate(obj, str, 0);
            Assert.Equal(0, result.res);
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test39()
        {
            str = "3-(-3)";
            result = LibraryImport.Calculate(obj, str, 0);
            Assert.Equal(6, result.res);
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test40()
        {
            str = "+3-(+3)";
            result = LibraryImport.Calculate(obj, str, 0);
            Assert.Equal(0, result.res);
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test42()
        {
            str = "31.2-(+3))";
            result = LibraryImport.Calculate(obj, str, 0);
            Assert.True(result.error);
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test43()
        {
            str = "((31.2-(+3))";
            result = LibraryImport.Calculate(obj, str, 0);
            Assert.True(result.error);
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test44()
        {
            str = "3(mod^+2sin.";
            result = LibraryImport.Calculate(obj, str, 0);
            Assert.True(result.error);
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test45()
        {
            str = "5-8..(sin(2))";
            result = LibraryImport.Calculate(obj, str, 0);
            Assert.True(result.error);
        }

        [Fact]
        public void Give_String_Then_ReturnResult_Test46()
        {
            str = "5++";
            result = LibraryImport.Calculate(obj, str, 0);
            Assert.True(result.error);
        }
    }
}
