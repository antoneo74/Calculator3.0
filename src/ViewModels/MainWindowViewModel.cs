using Calculator3.Models;
using ReactiveUI;

namespace Calculator3.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase _calculatorPage;
        private ViewModelBase _chartsPage;
        private ViewModelBase _creditPage;
        private ViewModelBase _depositPage;


        public MainWindowViewModel(ICalculator creditCalculator, ICalculator depositCalculator)
        {
            _calculatorPage = new CalculatorViewModel();
            _chartsPage = new ChartsViewModel();
            _creditPage = new CreditViewModel(creditCalculator);
            _depositPage = new DepositViewModel(depositCalculator);
        }


        public ViewModelBase CalculatorPage
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