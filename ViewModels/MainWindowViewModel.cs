using Avalonia.Controls;
using ReactiveUI;
using System.Windows.Input;

namespace Calculator3.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase _calculatorPage;
        private ViewModelBase _chartsPage;
        private ViewModelBase _creditPage;
        private ViewModelBase _depositPage;


        public MainWindowViewModel()
        {            
            _calculatorPage = new CalculatorViewModel();            
            _chartsPage = new ChartsViewModel();
            _creditPage = new CreditViewModel();
            _depositPage = new DepositViewModel();            
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