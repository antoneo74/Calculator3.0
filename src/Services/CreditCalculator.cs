using Calculator3.Models;
using Calculator3.Models.Credit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Calculator3.Services
{
    internal class CreditCalculator : ICalculator
    {
        #region Private Members
        private int Months { get; set; }

        private double Amount { get; set; }

        private double Rate { get; set; }

        private CreditResponse _response = null!;

        #endregion

        #region Public methods

        /// <summary>
        /// method calculates credit results as requested
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IDataResponse Calculate(IDataRequest request)
        {
            //_response = new CreditResponse();

            if (request is CreditRequest data)
            {
                ParseRequest(data);

                if (data.Annuitet == true)
                {
                    AnnuitetPaymentCalculate();
                }
                else
                {
                    DiffPaymentCalculate();
                }
            }
            return _response;
        }

        #endregion

        #region Private methods
        /// <summary>
        /// Parse request and iniinitialization private fields
        /// </summary>
        /// <param name="request"></param>
        private void ParseRequest(CreditRequest request)
        {
            Months = request.TimeUnit == 0 ? request.Term * 12 : request.Term;

            Rate = request.Rate;

            Amount = request.Amount;
        }

        /// <summary>
        /// calculation of annuity payment
        /// </summary>
        /// <returns></returns>
        private double Anuitet()
        {
            double monthlyPercent = Rate / 1200;

            return
                Amount * monthlyPercent / (1 - Math.Pow(1 + monthlyPercent, -Months));
        }

        /// <summary>
        /// loan calculation with annuity payments
        /// </summary>
        /// <returns></returns>
        private void AnnuitetPaymentCalculate()
        {
            double monthlyPayment = Math.Round(Anuitet(), 2);

            _response = new CreditResponse
            {
                MonthlyPayment = monthlyPayment.ToString(),

                AccuredInterest = Math.Round(monthlyPayment * Months - Amount, 2)
            };

            _response.Total = Math.Round(Amount + _response.AccuredInterest, 2);

            _response.ResponseCreditLines = new List<ResponseCreditLine>();

            DateTime nextMonth = DateTime.Now.AddMonths(1);

            int index = 1;

            while (Amount > 1)
            {
                double interestPayment = Math.Round(Amount * Rate / 1200, 2);

                double principalPayment = Math.Round(monthlyPayment - interestPayment, 2);

                Amount -= principalPayment;

                var line = new ResponseCreditLine(
                    index++,
                    $"{nextMonth:Y}",
                    monthlyPayment,
                    principalPayment,
                    interestPayment,
                    Math.Round(Amount, 2));

                _response.ResponseCreditLines.Add(line);

                nextMonth = nextMonth.AddMonths(1);
            }
        }


        /// <summary>
        /// loan calculation with differentiated payments
        /// </summary>
        /// <returns></returns>
        private void DiffPaymentCalculate()
        {
            _response = new CreditResponse
            {
                AccuredInterest = 0,
                Total = Amount
            };

            int index = 1;

            _response.ResponseCreditLines = new List<ResponseCreditLine>();

            DateTime nextMonth = DateTime.Now.AddMonths(1);

            double principalPayment = Amount / Months;

            while (Amount > 1)
            {
                double interestPayment = Amount * Rate / 1200;

                _response.AccuredInterest += interestPayment;

                double monthlyPayment = principalPayment + interestPayment;

                Amount -= principalPayment;

                var line = new ResponseCreditLine(
                    index++,
                    $"{nextMonth:Y}",
                    Math.Round(monthlyPayment, 2),
                    Math.Round(principalPayment, 2),
                    Math.Round(interestPayment, 2),
                    Math.Round(Amount, 2));

                _response.ResponseCreditLines.Add(line);

                nextMonth = nextMonth.AddMonths(1);
            }

            _response.AccuredInterest = Math.Round(_response.AccuredInterest, 2);

            _response.Total += _response.AccuredInterest;

            var first = _response.ResponseCreditLines.First().MonthlyPayment;

            var last = _response.ResponseCreditLines.Last().MonthlyPayment;

            _response.MonthlyPayment = $"{first}...{last}";
        }
        #endregion
    }
}
