using Avalonia.Controls;
using Calculator3.Models;
using Calculator3.Services;
using Calculator3.ViewModels;

namespace Calculator3.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Design.SetDataContext(this, new MainWindowViewModel(new CreditCalculator(), new DepositCalculator()));
        }
    }
}