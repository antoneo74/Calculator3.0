using Calculator3.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

namespace Calculator3.ViewModels
{
    public class CalculatorViewModel:ViewModelBase
    {
        private ObservableCollection<string> _history;
        public ObservableCollection<string> History
        {
            get => _history;
            set => this.RaiseAndSetIfChanged(ref _history, value); 
        }

        private List<string> _tokens;

        private string _expression;

        private string _xValue;

        private int _open_brackets = 0;

        private IntPtr calc;
        public ReactiveCommand<string, Unit> AddCommand { get; }
        public ReactiveCommand<Unit, Unit> ClearCommand { get; }
        public ReactiveCommand<Unit, Unit> BackSpaceCommand { get; }
        public ReactiveCommand<Unit, Unit> EqualCommand { get; }
        
        //public ReactiveCommand<Unit, Unit> HistoryCommand { get; }

        public string ShownExpression
        {
            get => _expression;

            set => this.RaiseAndSetIfChanged(ref _expression, value);
        }

        public string? XValue
        {
            get => _xValue;
            set => this.RaiseAndSetIfChanged(ref _xValue, value);
        }

        
        
        public CalculatorViewModel()
        {
            AddCommand = ReactiveCommand.Create<string>(Add);

            ClearCommand = ReactiveCommand.Create(Clear);

            BackSpaceCommand = ReactiveCommand.Create(BackSpace);

            EqualCommand = ReactiveCommand.Create(Equal);

            //HistoryCommand = ReactiveCommand.Create();

            calc = LibraryImport_x64.Constructor();

            

            _history = new ObservableCollection<string>();

            _tokens = new();
            Clear();
        }

        

        private void Equal()
        {
            var exp = TokensToString();

            
            History.Add(exp);

            var xIsCorrect = double.TryParse(_xValue, out double x);

            _tokens.Clear();

            _tokens.Add("0");

            XValue = string.Empty;

            if (!xIsCorrect && exp.Contains('X')) { ShownExpression = "Error"; }
            else
            {
                var sss = LibraryImport_x64.Test(calc, exp, x);

                if (sss.error)
                {
                    ShownExpression = "Error";
                }
                else
                {
                    ShownExpression = sss.res.ToString();
                }
            }
        }

        private void BackSpace()
        {
            _tokens.RemoveAt(_tokens.Count - 1);

            if (_tokens.Count == 0) { _tokens.Add("0"); }

            ShownExpression = TokensToString();
        }

        private void Clear()
        {
            _tokens.Clear();

            _tokens.Add("0");

            _open_brackets = 0;

            ShownExpression = "0";
        }

        private void Add(string obj)
        {
            if (IsEmptyExpression())
            {
                if (CheckEmptyExpression(obj)) _tokens.Add(obj);
            }
            else
            {
                bool check = true;
                switch (obj)
                {
                    case "(":
                    case ")":
                        check = CheckBrackets(obj);
                        break;
                    case ".e+0":
                        check = CheckExponenta();
                        break;
                }
                if (check) _tokens.Add(obj);
            }
            ShownExpression = TokensToString();
        }

        private bool CheckExponenta()
        {
            if (_tokens.Count > 0)
            {
                Regex r = new Regex(@"^\d$");

                Match m = r.Match(_tokens.Last());

                return m.Success;
            }
            return false;
        }

        private bool CheckBrackets(string obj)
        {
            switch (obj)
            {
                case "(":
                    ++_open_brackets;
                    break;
                case ")":
                    if (_open_brackets > 0) { _open_brackets--; }
                    else { return false; }
                    break;
            }
            return true;
        }

        private bool CheckEmptyExpression(string obj)
        {
            switch (obj)
            {
                case ")":
                    return false;
                case ".":
                case "*":
                case "/":
                case "^":
                case ".e+0":
                    break;
                default:
                    _tokens.Clear();
                    break;
            }
            return true;
        }

        public string TokensToString()
        {
            StringBuilder builder = new();

            foreach (var token in _tokens)
            {
                builder.Append(token);
            }
            return builder.ToString();
        }

        private bool IsEmptyExpression()
        {
            return _tokens.Count == 1 && _tokens[0] == "0";
        }
    }
}
