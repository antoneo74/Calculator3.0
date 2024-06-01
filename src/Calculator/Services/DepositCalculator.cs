using Calculator3.Models;
using Calculator3.Models.Deposit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Calculator3.Services
{
    public class DepositCalculator : ICalculator
    {
        #region Private Members
        private double Amount { get; set; }

        private double Rate { get; set; }

        private int Duration { get; set; }

        private Constants.Constants.PaymentFrequency Frequency { get; set; }

        private DateTime Start { get; set; }

        private DateTime Last { get; set; }

        private bool Capitalization { get; set; }

        private List<Replenishment> Replenishments { get; set; } = null!;

        private double AccuredInterestForReportingPeriod { get; set; }

        private double СarryoverInterest { get; set; }

        private double Tax { get; set; }

        private DepositResponse _response = null!;

        #endregion

        #region Public Methods

        /// <summary>
        /// method calculates deposit results as requested
        /// </summary>
        /// <param name="request"></param>
        /// <returns>response</returns>
        public IDataResponse Calculate(IDataRequest request)
        {
            if (request is DepositRequest data)
            {
                _response = new DepositResponse
                {
                    Lines = new List<ResponseDepositLine>()
                };

                ParseRequest(data);

                CalculateResponseParams();

                _response.AccuredInterest = Math.Round(_response.Lines.Sum(i => i.Percent), 2);

                _response.Total = Math.Round(Amount, 2);

                _response.Tax = Math.Round(Tax, 2);
            }
            return _response;
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// method calculates pesponse parameters
        /// </summary>
        private void CalculateResponseParams()
        {
            DateTime end = Start.AddDays(Duration);

            Last = Start;

            DateTime tmpDate = Start;

            while (Duration > 0)
            {
                tmpDate = GetEndInterval(tmpDate);

                tmpDate = tmpDate < end ? tmpDate : end;

                int daysToNextReplenishment;

                while (Replenishments.Count != 0 && Start <= Replenishments[0].Date && tmpDate >= Replenishments[0].Date)
                {
                    AddReplenishment();
                }
                double percent = 0;

                if (CheckEndOfYears(tmpDate))
                {
                    if (AccuredInterestForReportingPeriod > 0) Tax += TaxCalculation();

                    percent = СarryoverInterest;

                    СarryoverInterest = 0;
                }

                if (!CheckEndOfYears(tmpDate))
                {
                    percent += СarryoverInterest;

                    СarryoverInterest = 0;
                }

                daysToNextReplenishment = (tmpDate - Start).Days;

                percent += GetAccruedDepositPercent(daysToNextReplenishment);

                AccuredInterestForReportingPeriod += percent;

                Start = tmpDate;

                Last = tmpDate;

                Duration -= daysToNextReplenishment;

                var replenishment = Capitalization ? percent : -percent;

                Amount = Capitalization ? percent + Amount : Amount;

                var line = new ResponseDepositLine(tmpDate,
                                               Math.Round(percent, 2),
                                               Math.Round(replenishment, 2),
                                               Math.Round(Amount, 2));

                _response.Lines.Add(line);
            }
            Tax += TaxCalculation();
        }

        /// <summary>
        /// method adds replenishments/withdrawals 
        /// </summary>
        private void AddReplenishment()
        {
            if (!CheckWithdrawalAmount()) { return; }

            double replenishment = Replenishments[0].Operation == 0 ? Replenishments[0].Amount : -Replenishments[0].Amount;

            int daysToNextReplenishment = (Replenishments[0].Date - Start).Days;

            СarryoverInterest += GetAccruedDepositPercent(daysToNextReplenishment);

            Amount += replenishment;

            Duration -= daysToNextReplenishment;

            var line = new ResponseDepositLine(Replenishments[0].Date,
                                               0,
                                               Math.Round(replenishment, 2),
                                               Math.Round(Amount, 2));
            _response.Lines.Add(line);

            Start = Replenishments[0].Date;

            Replenishments.RemoveAt(0);
        }

        /// <summary>
        /// Parse request and iniinitialization private fields
        /// </summary>
        /// <param name="request"></param>
        private void ParseRequest(DepositRequest request)
        {
            Duration = GetDuration(request.Start, request.Term, request.TimeUnit);

            Rate = request.Rate;

            Amount = request.Amount;

            Frequency = request.Frequency;

            Start = request.Start;

            Capitalization = request.Capitalization;

            Replenishments = request.Replenishments.OrderBy(i => i.Date).ToList();

            AccuredInterestForReportingPeriod = 0;

            СarryoverInterest = 0;

            Tax = 0;
        }

        /// <summary>
        /// method calculates common deposit duration from start to end in days
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="term"></param>
        /// <param name="timeUnit"></param>
        /// <returns></returns>
        private static int GetDuration(DateTime begin, int term, Constants.Constants.TimeFrequency timeUnit)
        {
            int duration = 0;
            DateTime end;
            switch (timeUnit)
            {
                case Constants.Constants.TimeFrequency.Days:
                    duration = term;
                    break;

                case Constants.Constants.TimeFrequency.Months:
                    end = begin.AddMonths(term);
                    duration = (end - begin).Days;
                    break;

                case Constants.Constants.TimeFrequency.Years:
                    end = begin.AddYears(term);
                    duration = (end - begin).Days;
                    break;
            }
            return duration;
        }

        /// <summary>
        /// Method calculates end date of the next billing period
        /// </summary>
        /// <param name="day"></param>
        /// <returns>DateTime</returns>
        private DateTime GetEndInterval(DateTime day)
        {
            switch (Frequency)
            {
                case Constants.Constants.PaymentFrequency.End:
                    return day.AddDays(Duration);

                case Constants.Constants.PaymentFrequency.Daily:
                    return day.AddDays(1);

                case Constants.Constants.PaymentFrequency.Weekly:
                    return day.AddDays(7);

                case Constants.Constants.PaymentFrequency.Monthly:
                    return day.AddMonths(1);

                case Constants.Constants.PaymentFrequency.Quarterly:
                    return day.AddMonths(3);

                case Constants.Constants.PaymentFrequency.HalfYearly:
                    return day.AddMonths(6);

                case Constants.Constants.PaymentFrequency.Yearly:
                    return day.AddYears(1);

                default:
                    return day;
            }
        }

        /// <summary>
        /// number of days in the current year
        /// </summary>
        /// <returns></returns>
        private int GetDaysInYear()
        {
            return DateTime.IsLeapYear(Start.Year) ? 366 : 365;
        }

        /// <summary>
        /// calculation of interest for a specified period
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        private double GetAccruedDepositPercent(int duration)
        {
            int daysToEndOfYear = (new DateTime(Start.Year, 12, 31) - Start).Days;

            double result;

            if (duration <= daysToEndOfYear)
            {
                result = Amount * Rate * duration / GetDaysInYear() / 100;
            }
            else
            {
                result = Amount * Rate * daysToEndOfYear / GetDaysInYear() / 100;

                duration -= daysToEndOfYear;

                Start = Start.AddYears(1);

                while (duration > GetDaysInYear())
                {
                    result += Amount * Rate / 100;

                    duration -= GetDaysInYear();

                    Start = Start.AddYears(1);
                }
                result += Amount * Rate * duration / GetDaysInYear() / 100;
            }
            return Math.Round(result, 2);
        }


        /// <summary>
        /// Tax Calculation
        /// </summary>
        /// <returns></returns>
        private double TaxCalculation()
        {
            double nonTaxableAmount = 1000000 * Constants.Constants.RATECBR / 100;

            double result = 0;

            if (AccuredInterestForReportingPeriod > nonTaxableAmount)
            {
                result = (AccuredInterestForReportingPeriod - nonTaxableAmount) * Constants.Constants.RATENDFL / 100;
            }
            AccuredInterestForReportingPeriod = 0;
            return result;
        }

        /// <summary>
        /// checking whether the year has changed
        /// </summary>
        /// <param name="endPeriod"></param>
        /// <returns></returns>
        private bool CheckEndOfYears(DateTime endPeriod)
        {
            return endPeriod.Year - Last.Year > 0;
        }

        /// <summary>
        /// checking whether the withdrawal exceeds the deposit amount
        /// </summary>
        /// <returns></returns>
        private bool CheckWithdrawalAmount()
        {
            if (Replenishments[0].Operation == 1 && Replenishments[0].Amount > Amount)
            {
                Replenishments.RemoveAt(0);
                return false;
            }
            return true;
        }

        #endregion
    }
}
