using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Calculator3.Attributies
{
    internal class TermAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is string v)
            {
                string pattern = @"^[1-9]([0-9]*)?$";

                Regex regex = new(pattern);

                if (regex.IsMatch(v))
                {
                    if (Constants.Constants.TimeUnitParam == Constants.Constants.CreditFrequency.Years
                        && double.Parse(v, CultureInfo.InvariantCulture) > Constants.Constants.MAXYEARS)
                    {
                        ErrorMessage = $"Maximum {Constants.Constants.MAXYEARS}";
                    }
                    else if (Constants.Constants.TimeUnitParam == Constants.Constants.CreditFrequency.Months
                        && double.Parse(v, CultureInfo.InvariantCulture) > Constants.Constants.MAXMONTHS)
                    {
                        ErrorMessage = $"Maximum {Constants.Constants.MAXMONTHS}";
                    }
                    else if (Constants.Constants.TimeUnitParam == Constants.Constants.CreditFrequency.Days
                        && double.Parse(v, CultureInfo.InvariantCulture) > Constants.Constants.MAXDAYS)
                    {
                        ErrorMessage = $"Maximum {Constants.Constants.MAXDAYS}";
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                    ErrorMessage = "invalid value";
            }
            return false;
        }
    }
}

