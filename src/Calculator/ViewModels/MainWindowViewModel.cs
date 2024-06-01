using Calculator3.Models;
using ReactiveUI;
using System;
using System.IO;
using System.Reactive;
using System.Text.Json;

namespace Calculator3.ViewModels
{
    internal partial class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase? _calculatorPage;

        private ViewModelBase _chartsPage;

        private ViewModelBase _creditPage;

        private ViewModelBase _depositPage;

        public ReactiveCommand<Unit, Unit> ExitCommand { get; }

        public MainWindowViewModel(ICalculator creditCalculator, ICalculator depositCalculator)
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string path = Path.Combine(appDirectory, "history.json");

            if (File.Exists(path))
            {
                string dataJson = File.ReadAllText(path);

                _calculatorPage = JsonSerializer.Deserialize<CalculatorViewModel>(dataJson);
            }
            else
            {
                _calculatorPage = new CalculatorViewModel();
            }

            _chartsPage = new ChartsViewModel();

            _creditPage = new CreditViewModel(creditCalculator);

            _depositPage = new DepositViewModel(depositCalculator);

            ExitCommand = ReactiveCommand.Create(Exit);
        }

        private void Exit()
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string path = Path.Combine(appDirectory, "history.json");

            using (FileStream fs = new(path, FileMode.Create))
            {
                JsonSerializer.Serialize(fs, (CalculatorViewModel?)CalculatorPage);
            }
        }

        public ViewModelBase? CalculatorPage
        {
            get { return _calculatorPage; }
            private set { this.RaiseAndSetIfChanged(ref _calculatorPage, value); }
        }

        public ViewModelBase ChartsPage
        {
            get { return _chartsPage; }
            private set { this.RaiseAndSetIfChanged(ref _chartsPage, value); }
        }

        public ViewModelBase CreditPage
        {
            get { return _creditPage; }
            private set { this.RaiseAndSetIfChanged(ref _creditPage, value); }
        }

        public ViewModelBase DepositPage
        {
            get { return _depositPage; }
            private set { this.RaiseAndSetIfChanged(ref _depositPage, value); }
        }
    }
}