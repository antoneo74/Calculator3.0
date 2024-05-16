using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;


namespace Calculator3.Attributies
{
    internal class RateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is string v)
            {
                string pattern = @"^((0\.[0-9][0-9]?)|([1-9][0-9]*\.?[0-9]?[0-9]?))$";

                Regex regex = new(pattern);

                if (regex.IsMatch(v))
                {
                    if (double.Parse(v, CultureInfo.InvariantCulture) > Constants.Constants.MAXRATE)
                    {
                        ErrorMessage = $"Maximum {Constants.Constants.MAXRATE}";
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
