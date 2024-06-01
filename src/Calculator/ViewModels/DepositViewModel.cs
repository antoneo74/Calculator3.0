using Calculator3.Attributies;
using Calculator3.Models;
using Calculator3.Models.Deposit;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive;

namespace Calculator3.ViewModels
{
    internal class DepositViewModel : ViewModelBase
    {
        #region Private Members
        private readonly ICalculator _depositCalculator;

        private string _amount = string.Empty;

        private string _term = string.Empty;

        private string _rate = string.Empty;

        private int _timeUnit = 1;

        private DateTime _start;

        private int _frequency = 3;

        private bool _capitalization = false;

        private bool _isOpen = false;

        private DepositResponse _response;

        private double _amountRequestParam;

        private int _termRequestParam;

        private double _rateRequestParam;

        private ObservableCollection<Replenishment> _replenishments;

        private int _replenishmentOperation = 0;

        private DateTime _replenishmentDate;

        private string _replenishmentAmount = string.Empty;

        private bool _visibility = false;

        private double _tax;

        private double _accuredInterest;

        private double _total;

        private ObservableCollection<ResponseDepositLine> _listItems;

        #endregion

        #region Public Members

        public ObservableCollection<ResponseDepositLine> ListItems
        {
            get => _listItems;
            set => this.RaiseAndSetIfChanged(ref _listItems, value);
        }

        public double Tax
        {
            get => _tax;
            set => this.RaiseAndSetIfChanged(ref _tax, value);
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

        public bool Visibility
        {
            get => _visibility;
            set => this.RaiseAndSetIfChanged(ref _visibility, value);
        }

        public int ReplenishmentOperation
        {
            get => _replenishmentOperation;
            set => this.RaiseAndSetIfChanged(ref _replenishmentOperation, value);
        }


        public DateTime ReplenishmentDate
        {
            get => _replenishmentDate;
            set => this.RaiseAndSetIfChanged(ref _replenishmentDate, value);
        }

        [Required(ErrorMessage = "")]
        [Amount]
        public string ReplenishmentAmount
        {
            get => _replenishmentAmount;
            set => this.RaiseAndSetIfChanged(ref _replenishmentAmount, value);
        }

        public DepositResponse Response
        {
            get => _response;
            set => this.RaiseAndSetIfChanged(ref _response, value);
        }
        public bool IsOpen
        {
            get => _isOpen;
            set => this.RaiseAndSetIfChanged(ref _isOpen, value);
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
        public int TimeUnit
        {
            get => _timeUnit;
            set
            {
                this.RaiseAndSetIfChanged(ref _timeUnit, value);

                Constants.Constants.TimeUnitParam = (Constants.Constants.TimeFrequency)value;
            }
        }
        public DateTime Start
        {
            get => _start;
            set => this.RaiseAndSetIfChanged(ref _start, value);
        }
        public int Frequency
        {
            get => _frequency;
            set => this.RaiseAndSetIfChanged(ref _frequency, value);
        }
        public bool Capitalization
        {
            get => _capitalization;
            set => this.RaiseAndSetIfChanged(ref _capitalization, value);
        }

        public ObservableCollection<Replenishment> Replenishments
        {
            get => _replenishments;
            set => this.RaiseAndSetIfChanged(ref _replenishments, value);
        }

        #endregion

        #region Reactive commands
        public ReactiveCommand<Unit, Unit> CalculateCommand { get; }

        public ReactiveCommand<Unit, Unit> SideBarOpenCommand { get; }

        public ReactiveCommand<Unit, Unit> AddCommand { get; }

        public ReactiveCommand<Unit, Unit> ResetCommand { get; }
        #endregion

        #region Constructor
        public DepositViewModel(ICalculator depositCalculator)
        {
            _depositCalculator = depositCalculator;

            _response = new DepositResponse();

            _replenishments = new ObservableCollection<Replenishment>();

            _listItems = new ObservableCollection<ResponseDepositLine>();

            _start = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));

            _replenishmentDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));

            var isValidObservable = this.WhenAnyValue(
                x => x.Amount,
                x => x.Term,
                x => x.Rate,
                (amount, term, rate) => !string.IsNullOrWhiteSpace(amount)
                                        && !string.IsNullOrWhiteSpace(term)
                                        && !string.IsNullOrWhiteSpace(rate));

            CalculateCommand = ReactiveCommand.Create(Calculate, isValidObservable);

            var isValidReplenishment = this.WhenAnyValue(
                x => x.ReplenishmentAmount,
                (amount) => !string.IsNullOrEmpty(amount));

            AddCommand = ReactiveCommand.Create(AddReplenishment, isValidReplenishment);

            SideBarOpenCommand = ReactiveCommand.Create(SideBarOpen);

            ResetCommand = ReactiveCommand.Create(ResetReplenishments);
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

                var eee = Replenishments.ToList();
                IDataRequest request = new DepositRequest()
                {
                    Amount = _amountRequestParam,

                    Term = _termRequestParam,

                    Rate = _rateRequestParam,

                    TimeUnit = (Constants.Constants.TimeFrequency)TimeUnit,

                    Frequency = (Constants.Constants.PaymentFrequency)Frequency,

                    Start = Start,

                    Capitalization = Capitalization,

                    Replenishments = Replenishments.ToList(),
                };

                Response = (DepositResponse)_depositCalculator.Calculate(request);

                ParseResponce();

                IsOpen = true;
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Parse responce
        /// </summary>
        private void ParseResponce()
        {
            Tax = Response.Tax;

            AccuredInterest = Response.AccuredInterest;

            Total = Response.Total;

            ListItems = new(Response.Lines);
        }

        /// <summary>
        /// Validation input params
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
                (TimeUnit == 1 && _termRequestParam > Constants.Constants.MAXMONTHS) ||
                (TimeUnit == 2 && _termRequestParam > Constants.Constants.MAXDAYS))

                throw new Exception("Invalid value");
        }

        /// <summary>
        /// Method adds replenishment/withdrawal
        /// </summary>
        private void AddReplenishment()
        {
            if (double.TryParse(ReplenishmentAmount, out double amount) && ReplenishmentDate > Start)
            {
                Replenishments.Add(new Replenishment(ReplenishmentOperation, ReplenishmentDate, amount));

                Visibility = true;
            }
        }

        /// <summary>
        /// Open/close the window with calculation results
        /// </summary>
        private void SideBarOpen()
        {
            IsOpen = !IsOpen;
        }

        /// <summary>
        /// Reset replenishments list
        /// </summary>
        private void ResetReplenishments()
        {
            Replenishments.Clear();
        }
        #endregion
    }
}
