using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using System.Collections.Generic;

using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using SkiaSharp;
using Calculator3.Models;
using System;
using ReactiveUI;

namespace Calculator3.ViewModels
{
    public class ChartsViewModel : ViewModelBase
    {
        private static readonly SKColor s_gray = new(195, 195, 195);
        private static readonly SKColor s_gray1 = new(160, 160, 160);
        private static readonly SKColor s_gray2 = new(90, 90, 90);
        private static readonly SKColor s_dark3 = new(60, 60, 60);
        private ISeries[] _series = Array.Empty<ISeries>();
        public static string Fun { get; set; } = string.Empty;

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

            var _calc = LibraryImport_x64.Constructor();

            for (var x = -10f; x < 10f; x += 0.1f)
            {
                if (Fun != null)
                {
                    var y = LibraryImport_x64.Calculate(_calc, Fun.Replace('x','X'), x);

                    list.Add(new ObservablePoint(x, y.res));
                }
            }
            return list;
        }
    }
}
