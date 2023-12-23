using Avalonia.Controls;
using ReactiveUI;
using System.Windows.Input;

namespace Calculator3.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ICommand NavigateCalculator { get; }
        public ICommand NavigateCharts { get; }
        public ICommand NavigateCredit { get; }
        public ICommand NavigateDeposit { get; }

        

        public MainWindowViewModel()
        {
            // Set current page to first on start up
            CurrentPage = new CalculatorViewModel();            

            NavigateCalculator = ReactiveCommand.Create(Calculator);
            NavigateCharts = ReactiveCommand.Create(Charts);
            NavigateCredit = ReactiveCommand.Create(Credit);
            NavigateDeposit = ReactiveCommand.Create(Deposit);
        }

        private void Deposit()
        {
            CurrentPage = new DepositViewModel();
        }

        private void Credit()
        {
            CurrentPage = new CreditViewModel();
        }

        private void Charts()
        {
            CurrentPage = new ChartsViewModel();
        }

        private void Calculator()
        {
            CurrentPage = new CalculatorViewModel();
        }

        // The default is the first page
        private ViewModelBase _CurrentPage;

        /// <summary>
        /// Gets the current page. The property is read-only
        /// </summary>
        public ViewModelBase CurrentPage
        {
            get { return _CurrentPage; }
            private set { this.RaiseAndSetIfChanged(ref _CurrentPage, value); }
        }
    }
}