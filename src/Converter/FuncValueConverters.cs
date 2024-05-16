using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Calculator3.Converter
{
    public static class FuncValueConverters
    {
        public static FuncValueConverter<int, IBrush?> BackGroundConverter { get; } =
        new FuncValueConverter<int, IBrush?>(s =>
        {
            // define output variable
            IBrush color;
            if (s == 0)
            {
                color = Brushes.LightGreen;
            }
            else
            {
                Color.TryParse($"#f79999", out Color c);
                color = new SolidColorBrush(c);
            }
            return color;
        });

        public static FuncValueConverter<double, IBrush?> ChangeBalanceConverter { get; } =
        new FuncValueConverter<double, IBrush?>(s =>
        {
            // define output variable
            IBrush color = Brushes.LightYellow;
            if (s > 0)
            {
                color = Brushes.LightGreen;
            }
            else if (s < 0)
            {
                Color.TryParse($"#f79999", out Color c);
                color = new SolidColorBrush(c);
            }
            return color;
        });

        public static FuncValueConverter<int, string> TextConverter { get; } =
        new FuncValueConverter<int, string>(s =>
        {
            // define output variable
            string text;
            text = s == 0 ? "+" : "-";
            return text;
        });
    }
}
