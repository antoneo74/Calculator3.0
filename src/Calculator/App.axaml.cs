using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Calculator3.Models;
using Calculator3.Services;
using Calculator3.ViewModels;
using Calculator3.Views;
using System.Globalization;
using System.Threading;

namespace Calculator3
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            Thread.CurrentThread.CurrentCulture = new CultureInfo("EN-US");
        }

        public override void OnFrameworkInitializationCompleted()
        {
            ICalculator creditCalculator = new CreditCalculator();
            ICalculator depositCalculator = new DepositCalculator();


            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(creditCalculator, depositCalculator),
                };                
            }

            base.OnFrameworkInitializationCompleted();
        }        
    }
}