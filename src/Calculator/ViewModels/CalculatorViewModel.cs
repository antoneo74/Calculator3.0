using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using Calculator3.Models.Calculator;
using System.Text.Json.Serialization;

namespace Calculator3.ViewModels
{
    public class CalculatorViewModel : ViewModelBase
    {
        private bool _expIsActive = false;

        private bool _openActive = false;

        private bool _operationActive = false;

        private bool _operandActive = false;

        private List<string> _tokens;

        private string _expression = "0";

        private string _xValue = string.Empty;

        private int _open_brackets = 0;

        private int _selectedIndex = 0;

        private readonly IntPtr _calc;

        private ObservableCollection<string> _history;

        [JsonIgnore]
        public ReactiveCommand<string, Unit> AddCommand { get; }

        [JsonIgnore]
        public ReactiveCommand<Unit, Unit> ClearCommand { get; }

        [JsonIgnore]
        public ReactiveCommand<Unit, Unit> BackSpaceCommand { get; }

        [JsonIgnore]
        public ReactiveCommand<Unit, Unit> EqualCommand { get; }

        public CalculatorViewModel()
        {
            AddCommand = ReactiveCommand.Create<string>(Add);

            ClearCommand = ReactiveCommand.Create(Clear);

            BackSpaceCommand = ReactiveCommand.Create(BackSpace);

            EqualCommand = ReactiveCommand.Create(Equal);

            _calc = LibraryImport.Constructor();

            _history = new ObservableCollection<string>();

            _tokens = new List<string>();
        }

        [JsonInclude]
        public ObservableCollection<string> History
        {
            get => _history;
            set => this.RaiseAndSetIfChanged(ref _history, value);
        }

        [JsonIgnore]
        public int SelectedIndex
        {
            get => _selectedIndex;
            set => this.RaiseAndSetIfChanged(ref _selectedIndex, value);
        }

        [JsonIgnore]
        public string ShownExpression
        {
            get => _expression;
            set => this.RaiseAndSetIfChanged(ref _expression, value);
        }

        [JsonIgnore]
        public string XValue
        {
            get => _xValue;
            set => this.RaiseAndSetIfChanged(ref _xValue, value);
        }

        #region Functions processing button presses

        /// <summary>
        /// Remove last symbol/function
        /// </summary>
        private void BackSpace()
        {
            if (!_tokens.IsEmpty())
            {
                if (_tokens[^1].IsNumber())
                {
                    RemoveNumberDigit();
                    if (!_tokens.IsEmpty() && _tokens[^1].Contains('e'))
                    {
                        _tokens[^1] = "e+0";
                    }
                }
                else
                {
                    if (_tokens[^1].Contains('(')) --_open_brackets;

                    _tokens.RemoveAt(_tokens.Count - 1);
                }

                if (_tokens.Count == 1 && _tokens[^1] == "0") _tokens.Clear();
            }
            InitFlags();

            ShownExpression = _tokens.IsEmpty() ? "0" : _tokens.ListToString();
        }

        /// <summary>
        /// Method to reset calculator
        /// /// Triggered when the button "C" is pressed
        /// </summary>
        private void Clear()
        {
            Reset();

            ShownExpression = "0";
        }

        /// <summary>
        /// Method that calls the calculation of the entered expression. 
        /// Triggered when the button "=" is pressed
        /// </summary>
        private void Equal()
        {
            AddMissingBrackets();

            var exp = _tokens.ListToString();

            if (exp != string.Empty)
            {
                History.Insert(0, exp);

                var xIsCorrect = double.TryParse(_xValue, out double x);

                if (!xIsCorrect && exp.Contains('X')) { ShownExpression = "Error"; }
                else
                {
                    var result = LibraryImport.Calculate(_calc, exp, x);

                    ShownExpression = result.error ? "Error" : Math.Round(result.res, 12).ToString();
                }
                Reset();
            }
        }

        /// <summary>
        /// method that adds the value of an object to the resulting expression
        /// </summary>
        /// <param name="obj">the value associated with the pressed button</param>
        private void Add(string obj)
        {
            if (obj.IsNumber())
            {
                AddDigit(obj);
            }
            else
            {
                switch (obj)
                {
                    case "e+0":
                        AddExponenta();
                        break;
                    case "(":
                        AddOpeningBracket();
                        break;
                    case ")":
                        AddClosingBracket();
                        break;
                    case ".":
                        AddDot();
                        break;
                    case "-":
                        AddMinus();
                        break;
                    case "+":
                        AddPlus();
                        break;
                    case "X":
                        AddX();
                        break;
                    case "*":
                    case "/":
                    case "^":
                    case "mod":
                        AddBinaryOperation(obj);
                        break;
                    case "sqrt":
                    case "ln":
                    case "log":
                    case "sin":
                    case "asin":
                    case "cos":
                    case "acos":
                    case "tan":
                    case "atan":
                        AddFunction(obj);
                        break;
                }
            }
            InitFlags();
            ShownExpression = _tokens.IsEmpty() ? "0" : _tokens.ListToString();
        }

        /// <summary>
        /// Displaying the selected history item
        /// </summary>
        public void SelectHistoryItem()
        {
            if (History.Count != 0)
            {
                _tokens.Clear();

                _tokens.Add(History[SelectedIndex]);

                ShownExpression = History[SelectedIndex];
            }
        }

        /// <summary>
        /// Open documentation web page
        /// </summary>
        public static void Documentation()
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string projectPath = appDirectory.Substring(0, appDirectory.IndexOf("bin"));

            var path = Path.Combine(projectPath, "documentation", "index.html");

            var process = new Process();

            process.StartInfo = new ProcessStartInfo(path)
            {
                UseShellExecute = true
            };
            process.Start();
        }

        /// <summary>
        /// Clear displaing history
        /// </summary>
        public void ClearHistory()
        {
            History.Clear();
        }
        #endregion

        #region functions that implement the rules for displaying input data

        /// <summary>
        /// Actions if the "." button is pressed
        /// </summary>
        private void AddDot()
        {
            if (_tokens.IsEmpty() || _tokens[^1].Contains('(') || _operationActive)
            {
                _tokens.Add("0.");
            }
            else if (_expIsActive || (!_tokens[^1].IsNumber() && !_operationActive)
                || (_tokens.Count > 1 && _tokens[^2].Contains('e')) || _tokens[^1].Contains('.'))
            {
                _tokens.Add("*");
                _tokens.Add("0.");
            }
            else
            {
                _tokens[^1] += ".";
            }

        }

        /// <summary>
        /// Actions if the any function button (sqrt, ln etc.) is pressed
        /// </summary>
        private void AddFunction(string obj)
        {
            if (_operandActive)
            {
                _tokens.Add("*");
            }
            _tokens.Add(obj + "(");

            ++_open_brackets;
        }

        /// <summary>
        /// Actions if the any binary operation button (*, /, ^, mod) is pressed
        /// </summary>
        private void AddBinaryOperation(string obj)
        {
            if (_tokens.IsEmpty())
            {
                _tokens.Add("0");
                _tokens.Add(obj);
            }
            else
            {
                if (!_operationActive && _operandActive)
                {
                    _tokens.Add(obj);
                }
            }
        }

        /// <summary>
        /// Actions if the button with digit is pressed
        /// </summary>
        private void AddDigit(string obj)
        {
            if (_expIsActive)
            {
                _tokens[^1] = _tokens[^1].Remove(_tokens[^1].Length - 1);

                _tokens.Add(obj);
            }
            else if (!_tokens.IsEmpty() && _tokens[^1].IsNumber())
            {
                if (_tokens[^1] == "0") { _tokens[^1] = obj; }

                else { _tokens[^1] += obj; }
            }
            else if (_operandActive)
            {
                _tokens.Add("*");

                _tokens.Add(obj);
            }
            else
            {
                _tokens.Add(obj);
            }
        }

        /// <summary>
        /// If last token is number remove last digit of the number
        /// </summary>
        private void RemoveNumberDigit()
        {
            _tokens[^1] = _tokens[^1].Remove(_tokens[^1].Length - 1);

            if (_tokens[^1] == string.Empty)
            {
                _tokens.Remove(_tokens[^1]);
            }
        }

        /// <summary>
        /// Actions if the "X" button is pressed
        /// </summary>
        private void AddX()
        {
            if (!_operationActive && !_tokens.IsEmpty() && !_openActive)
            {
                _tokens.Add("*");
            }
            _tokens.Add("X");
        }

        /// <summary>
        /// Actions if the "+" button is pressed
        /// </summary>
        private void AddPlus()
        {
            if (_operationActive)
            {
                AddOpeningBracket();
            }
            _tokens.Add("+");
        }

        /// <summary>
        /// Actions if the "-" button is pressed
        /// </summary>
        private void AddMinus()
        {
            if (_expIsActive)
            {
                _tokens[^1] = _tokens[^1] == "e+0" ? "e-0" : "e+0";
            }
            else
            {
                if (_operationActive)
                {
                    AddOpeningBracket();
                }
                _tokens.Add("-");
            }
        }

        /// <summary>
        /// Actions if the ")" button is pressed
        /// </summary>
        private void AddClosingBracket()
        {
            if (!_openActive && _operandActive && _open_brackets != 0)
            {
                --_open_brackets;
                _tokens.Add(")");
            }
        }

        /// <summary>
        /// Actions if the "(" button is pressed
        /// </summary>
        private void AddOpeningBracket()
        {
            ++_open_brackets;
            if (!_operationActive && !_tokens.IsEmpty() && !_tokens[^1].Contains('(')) _tokens.Add("*");
            _tokens.Add("(");
        }

        /// <summary>
        /// Actions if the exponenta button is pressed
        /// </summary>
        private void AddExponenta()
        {
            if (_tokens.IsEmpty())
            {
                _tokens.Add("0");
            }
            if (_tokens[^1].IsNumber())
            {
                if (_tokens.Count == 1)
                {
                    _tokens.Add("e+0");
                }
                else
                {
                    if (!_tokens[^2].Contains('e'))
                    {
                        _tokens.Add("e+0");
                    }
                    else
                    {
                        _tokens.Add("*");
                        _tokens.Add("0");
                        _tokens.Add("e+0");
                    }
                }
            }
        }
        #endregion

        #region Helped functions
        /// <summary>
        /// Method adding missing closing brackets
        /// </summary>
        private void AddMissingBrackets()
        {
            if (!_openActive && _operandActive)
            {
                for (int i = 0; i < _open_brackets; --_open_brackets)
                {
                    _tokens.Add(")");
                }
            }
        }

        /// <summary>
        /// Reset all flags to default
        /// </summary>
        private void ResetAllFlags()
        {
            _expIsActive = false;

            _openActive = false;

            _operationActive = false;

            _operandActive = false;
        }

        /// <summary>
        /// Setting flags for the current expression
        /// </summary>
        private void InitFlags()
        {
            ResetAllFlags();

            if (!_tokens.IsEmpty())
            {
                if (_tokens[^1].IsNumber())
                {
                    _operandActive = true;
                }
                else
                {
                    switch (_tokens[^1])
                    {
                        case "X":
                        case ")":
                        case ".":
                            _operandActive = true;
                            break;
                        case "e+0":
                        case "e-0":
                            _expIsActive = true;
                            _operandActive = true;
                            break;
                        case "+":
                        case "-":
                        case "^":
                        case "/":
                        case "*":
                        case "mod":
                            _operationActive = true;
                            break;
                        default:
                            _openActive = true;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Reset to default settings
        /// </summary>
        private void Reset()
        {
            _tokens.Clear();

            _open_brackets = 0;

            XValue = string.Empty;

            InitFlags();
        }

        #endregion
    }

    #region static class with extension methods
    public static class Extensions
    {
        /// <summary>
        /// Extension method for list of strings
        /// Make result stringfrom list items
        /// </summary>
        public static string ListToString(this List<string> tokens)
        {
            StringBuilder builder = new();

            tokens.ForEach(tokens => builder.Append(tokens));

            return builder.ToString();
        }

        /// <summary>
        /// checking list for emptiness
        /// </summary>
        /// <returns>true if list empty</returns>
        public static bool IsEmpty(this List<string> tokens)
        {
            return tokens.Count == 0;
        }

        /// <summary>
        /// Extension method for checking whether argument is number
        /// </summary>
        /// <param name="str">checking argument</param>
        /// <returns>true if argument is number</returns>
        public static bool IsNumber(this string str)
        {
            return double.TryParse(str, out var _);
        }
    }
    #endregion
}
