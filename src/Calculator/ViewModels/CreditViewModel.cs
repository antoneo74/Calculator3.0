using Calculator3.Attributies;
using Calculator3.Models;
using Calculator3.Models.Credit;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Reactive;

namespace Calculator3.ViewModels
{
    internal class CreditViewModel : ViewModelBase
    {
        #region Public members       

        public ObservableCollection<ResponseCreditLine> ListItems
        {
            get => _listItems;

            set => this.RaiseAndSetIfChanged(ref _listItems, value);
        }

        [Required(ErrorMessage = "")]
        [Amount]
        public string Amount
        {
            get => _amount;

            set => this.RaiseAndSetIfChanged(ref _amount, value);
        }

        [Required(ErrorMessage = "")]
        [Term]
        public string Term
        {
            get => _term;

            set => this.RaiseAndSetIfChanged(ref _term, value);
        }

        [Required(ErrorMessage = "")]
        [Rate]
        public string Rate
        {
            get => _rate;

            set => this.RaiseAndSetIfChanged(ref _rate, value);
        }

        public bool IsOpen
        {
            get => _isOpen;

            set => this.RaiseAndSetIfChanged(ref _isOpen, value);
        }

        public int TimeUnit
        {
            get => _timeUnit;

            set
            {
                this.RaiseAndSetIfChanged(ref _timeUnit, value);

                Constants.Constants.TimeUnitParam = (Constants.Constants.TimeFrequency)value;
            }
        }

        public bool Annuitet
        {
            get => _annuitet;

            set => this.RaiseAndSetIfChanged(ref _annuitet, value);
        }

        public CreditResponse Response
        {
            get => _response;

            set => this.RaiseAndSetIfChanged(ref _response, value);
        }

        public string MonthlyPayment
        {
            get => _monthlyPayment;

            set => this.RaiseAndSetIfChanged(ref _monthlyPayment, value);
        }

        public double AccuredInterest
        {
            get => _accuredInterest;

            set => this.RaiseAndSetIfChanged(ref _accuredInterest, value);
        }

        public double Total
        {
            get => _total;

            set => this.RaiseAndSetIfChanged(ref _total, value);
        }

        #endregion

        #region Private Members

        readonly private ICalculator _creditCalculator;

        #region Request

        private string _amount = string.Empty;

        private string _term = string.Empty;

        private string _rate = string.Empty;

        private int _timeUnit;

        private bool _annuitet = true;

        private double _amountRequestParam;

        private int _termRequestParam;

        private double _rateRequestParam;

        private bool _isOpen;

        #endregion

        #region Response

        private CreditResponse _response;

        private string _monthlyPayment = string.Empty;

        private double _accuredInterest;

        private double _total;

        private ObservableCollection<ResponseCreditLine> _listItems;

        #endregion

        #endregion

        #region Reactive Commands
        public ReactiveCommand<Unit, Unit> CalculateCommand { get; }

        public ReactiveCommand<Unit, Unit> SideBarOpenCommand { get; }
        #endregion

        #region Constructor
        public CreditViewModel(ICalculator creditCalculator)
        {
            _listItems = new ObservableCollection<ResponseCreditLine>();

            _response = new CreditResponse();

            _creditCalculator = creditCalculator;

            var isValidObservable = this.WhenAnyValue(
                x => x.Amount,
                x => x.Term,
                x => x.Rate,
                (amount, term, rate) => !string.IsNullOrWhiteSpace(amount)
                                        && !string.IsNullOrWhiteSpace(term)
                                        && !string.IsNullOrWhiteSpace(rate));

            CalculateCommand = ReactiveCommand.Create(Calculate, isValidObservable);

            SideBarOpenCommand = ReactiveCommand.Create(SideBarOpen);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// the method prepares a request and receives a response
        /// </summary>
        private void Calculate()
        {
            try
            {
                InputValidation();

                IDataRequest request = new CreditRequest()
                {
                    Amount = _amountRequestParam,

                    Term = _termRequestParam,

                    Rate = _rateRequestParam,

                    TimeUnit = (Constants.Constants.TimeFrequency)TimeUnit,

                    Annuitet = Annuitet
                };
                Response = (CreditResponse)_creditCalculator.Calculate(request);

                ParseResponce();

                IsOpen = true;
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Check input params
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void InputValidation()
        {
            _amountRequestParam = double.Parse(Amount);

            _rateRequestParam = double.Parse(Rate);

            _termRequestParam = int.Parse(Term);

            // Credit amount cannot be < 0.01
            if (_amountRequestParam < 0.01
                || _amountRequestParam > Constants.Constants.MAXAMOUNT) throw new Exception("Invalid value");

            // Credit rate cannot be < 0.01
            if (_rateRequestParam < 0.01
                || _rateRequestParam > Constants.Constants.MAXRATE) throw new Exception("Invalid value");

            // Credit duration must be > 0            
            if (_termRequestParam < 1 ||
                (TimeUnit == 0 && _termRequestParam > Constants.Constants.MAXYEARS) ||
                (TimeUnit == 1 && _termRequestParam > Constants.Constants.MAXMONTHS))

                throw new Exception("Invalid value");
        }


        /// <summary>
        /// Parse responce
        /// </summary>
        private void ParseResponce()
        {
            MonthlyPayment = Response.MonthlyPayment;

            AccuredInterest = Response.AccuredInterest;

            Total = Response.Total;

            ListItems = new(Response.ResponseCreditLines);
        }

        /// <summary>
        /// Open/close the window with calculation results
        /// </summary>
        private void SideBarOpen()
        {
            IsOpen = !IsOpen;
        }

        #endregion
    }
}
