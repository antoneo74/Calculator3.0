using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using System.Collections.Generic;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using SkiaSharp;
using System;
using ReactiveUI;
using System.Text.RegularExpressions;
using Calculator3.Models.Calculator;

namespace Calculator3.ViewModels
{
    public class ChartsViewModel : ViewModelBase
    {
        private static readonly SKColor s_gray = new(195, 195, 195);
        private static readonly SKColor s_gray1 = new(160, 160, 160);
        private static readonly SKColor s_gray2 = new(90, 90, 90);
        private static readonly SKColor s_dark3 = new(60, 60, 60);
        private ISeries[] _series = Array.Empty<ISeries>();
        public static string Fun { get; set; } = null!;
        public static string XMin { get; set; } = null!;
        public static string XMax { get; set; } = null!;
        public static string YMin { get; set; } = null!;
        public static string YMax { get; set; } = null!;

        public ISeries[] Series
        {
            get => _series;
            set => this.RaiseAndSetIfChanged(ref _series, value);
        }
        public void Refresh()
        {
            Series = new ISeries[] {
                new LineSeries<ObservablePoint>
                {
                    Values = Fetch(),
                    Stroke = new SolidColorPaint(new SKColor(33, 150, 243), 4),
                    Fill = null,
                    GeometrySize = 0
                }
            };
        }

        public Axis[] XAxes { get; set; } =
        {
            new Axis
            {
                Name = "X axis",
                NamePaint = new SolidColorPaint(s_gray1),
                TextSize = 12,
                Padding = new Padding(5, 15, 5, 5),
                LabelsPaint = new SolidColorPaint(s_gray),
                SeparatorsPaint = new SolidColorPaint
                {
                    Color = s_gray,
                    StrokeThickness = 1,
                    PathEffect = new DashEffect(new float[] { 3, 3 })
                },
                SubseparatorsPaint = new SolidColorPaint
                {
                    Color = s_gray2,
                    StrokeThickness = 0.2f
                },
                SubseparatorsCount = 9,
                ZeroPaint = new SolidColorPaint
                {
                    Color = s_gray1,
                    StrokeThickness = 2
                },
                TicksPaint = new SolidColorPaint
                {
                    Color = s_gray,
                    StrokeThickness = 1.5f
                },
                SubticksPaint = new SolidColorPaint
                {
                    Color = s_gray,
                    StrokeThickness = 1
                }
            }
        };

        public Axis[] YAxes { get; set; } =
        {
            new Axis
            {
                Name = "Y axis",
                NamePaint = new SolidColorPaint(s_gray1),
                TextSize = 12,
                Padding = new Padding(5, 0, 15, 0),
                LabelsPaint = new SolidColorPaint(s_gray),
                SeparatorsPaint = new SolidColorPaint
                {
                    Color = s_gray,
                    StrokeThickness = 1,
                    PathEffect = new DashEffect(new float[] { 3, 3 })
                },
                SubseparatorsPaint = new SolidColorPaint
                {
                    Color = s_gray2,
                    StrokeThickness = 0.2f
                },
                SubseparatorsCount = 9,
                ZeroPaint = new SolidColorPaint
                {
                    Color = s_gray1,
                    StrokeThickness = 2
                },
                TicksPaint = new SolidColorPaint
                {
                    Color = s_gray,
                    StrokeThickness = 1.5f
                },
                SubticksPaint = new SolidColorPaint
                {
                    Color = s_gray,
                    StrokeThickness = 1
                }
            }
        };

        public DrawMarginFrame Frame { get; set; } =
        new()
        {
            Fill = new SolidColorPaint(s_dark3),
            Stroke = new SolidColorPaint
            {
                Color = s_gray,
                StrokeThickness = 1
            }
        };

        private static List<ObservablePoint> Fetch()
        {
            var list = new List<ObservablePoint>();

            var _calc = LibraryImport.Constructor();

            bool res_x_min = double.TryParse(XMin, out double x_min);

            bool res_x_max = double.TryParse(XMax, out double x_max);

            x_max = (res_x_max) ? x_max : 10;

            x_min = (res_x_min) ? x_min : -10;

            x_min = (x_min >= x_max) ? x_max - 20 : x_min;

            bool res_y_min = double.TryParse(YMin, out double y_min);

            bool res_y_max = double.TryParse(YMax, out double y_max);

            y_max = (res_y_max) ? y_max : 10;

            y_min = (res_y_min) ? y_min : -10;

            y_min = (y_min >= y_max) ? y_max - 20 : y_min;

            if (Regex.IsMatch(Fun, @"\p{IsCyrillic}"))
            {
                Fun = string.Empty;
            }

            for (var x = x_min; x < x_max; x += 0.1f)
            {
                if (Fun != null)
                {
                    var y = LibraryImport.Calculate(_calc, Fun.Replace('x', 'X'), x);

                    if (y.res <= y_max && y.res >= y_min)
                        list.Add(new ObservablePoint(x, y.res));
                }
            }
            return list;
        }
    }
}
