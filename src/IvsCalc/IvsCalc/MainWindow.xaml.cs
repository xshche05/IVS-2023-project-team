using System;
using IvsCalc.Classes;
using IvsCalc.Classes.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.IO;
using WinUICommunity;
using System.Collections.Generic;
using Windows.System;
using Microsoft.UI.Xaml.Documents;
using Rijndael256;
using Windows.UI.Core;

namespace IvsCalc {
    /// <summary>
    /// Main calculator window.
    /// </summary>
    public sealed partial class MainWindow : BackdropWindow {
        private bool IsOperatorClicked;
        private bool IsEqualityClicked;
        private bool IsWaitingForClosure;
        private bool IsErrorOccurred;
        private bool IsLastOperatorSimple = true;
        private bool IsLastOperatorUnary;
        private bool IsLastNumber;
        private bool IsBracketAllowed = true;
        private bool IsLastNumberClick = true;
        private bool ResetInput;
        private bool OnlyClose;
        private int BracketCounter;

        private string FinalExpression = "";
        private string LastOperator = "";
        private InputNumber _LastNumber = "0";

        private InputNumber _InputNumber = "0";

        /// <inheritdoc />
        public MainWindow() {
            InitializeComponent();

            Title = "Calculator";
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);
            SetIcon(Path.Combine(App.ApplicationFolder, "Assets/IvsCalcIcon.ico"));

            this.SetWindowSize(360, 640);
            WindowHelper.MinWindowHeight = 520;
            WindowHelper.MinWindowWidth = 270;
            this.RegisterWindowMinMax();

            Backdrop = BackdropType.Mica;
        }

        /// <summary>
        /// Updates the <see cref="ButtonC"/> text.
        /// </summary>
        void UpdateClearButton() {
            ButtonC.Content = Result.Text != "0" && !IsEqualityClicked ? "CE" : "C";
        }

        /// <summary>
        /// Updates the <see cref="Result"/> to always display an <see cref="_InputNumber"/>.
        /// </summary>
        void UpdateResultField() {
            Result.Text = _InputNumber.DisplayNumber;
        }

        /// <summary>
        /// Event handler for calculator's number buttons.
        /// </summary>
        /// <param name="sender">A button</param>
        /// <param name="e">Default arguments</param>
        private void Number_Click(object sender, RoutedEventArgs e) {
            if (!(sender is Button button)) return;
            if (button.Content == null) return;

            if (button.Content.ToString() != ".")
            {
                if (IsOperatorClicked || Result.Text == "0" || ResetInput)
                {
                    _InputNumber = "";
                    IsOperatorClicked = false;
                }
            }
            if (IsEqualityClicked || IsErrorOccurred)
            {
                Expression.Text = "";
                FinalExpression = "";
                _InputNumber = "";
            }
            IsErrorOccurred = false;
            ResetInput = false;
            string char_in = button.Content.ToString() ?? "";
            if (char_in == ".")
            {
                if (!_InputNumber.DisplayNumber.Contains("."))
                    _InputNumber += ".";
            }
            else
            {
                _InputNumber += char_in;
            }


            UpdateResultField();
            UpdateClearButton();

            IsBracketAllowed = true;
            IsLastNumberClick = true;
            IsLastNumber = true;
            IsEqualityClicked = false; // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!?????????????????????????????????????????????????????????
            _LastNumber = Result.Text;
        }

        /// <summary>
        /// Event handler for calculator's negation button.
        /// Changes the sign of <see cref="_InputNumber"/>.
        /// </summary>
        /// <param name="sender">A button</param>
        /// <param name="e">Default arguments</param>
        private void Negate_Click(object sender, RoutedEventArgs e) {
            _InputNumber.ChangeSign();

            UpdateResultField();
            UpdateClearButton();
            IsLastNumberClick = true;
        }

        /// <summary>
        /// Event handler for calculator's equality button.
        /// </summary>
        /// <param name="sender">A button</param>
        /// <param name="e">Default arguments</param>
        private void Eq_Click(object sender, RoutedEventArgs e) {
            if (FinalExpression == "") return;
            if (!IsLastNumber && !IsLastNumberClick) return;
 //           if (BracketCounter != 0)
 //           {
 //              if (IsLastNumberClick)
 //               {
 //                   Expression.Text += $"{_InputNumber.DisplayNumber}";
 //                   FinalExpression += $"{_InputNumber.ExprNumber}";
 //               }
 //           }
            if (!IsErrorOccurred) {
                if (IsEqualityClicked && LastOperator != ")") {
                    if (IsLastOperatorSimple)
                    {
                        Expression.Text = $"{_InputNumber.DisplayNumber} {LastOperator} {_LastNumber.DisplayNumber}";
                        FinalExpression = $"{_InputNumber.ExprNumber} {LastOperator} {_LastNumber.ExprNumber}"; 
                    }
                    else
                    {
                        if (LastOperator == "^ 2")
                        {
                            Expression.Text = $"{_InputNumber.DisplayNumber}^2";
                            FinalExpression = $"{_InputNumber.ExprNumber}^2";
                        }

                        if (LastOperator == "sqrt")
                        {
                            Expression.Text = $"sqrt({_InputNumber.DisplayNumber})";
                            FinalExpression = $"(sqrt({_InputNumber.ExprNumber}))";
                        }

                        if (LastOperator == "nqrt")
                        {
                            Expression.Text = $"({_InputNumber.DisplayNumber})^(1/{_LastNumber.DisplayNumber})";
                            FinalExpression = $"(({_InputNumber.ExprNumber})^(1/{_LastNumber.ExprNumber}))";
                        }

                        if (LastOperator == "!")
                        {
                            Expression.Text = $"{_InputNumber.DisplayNumber}!";
                            FinalExpression = $"{_InputNumber.ExprNumber}!";
                        }
                    }
                } else {
                    if (LastOperator != "^ 2" && LastOperator != "sqrt" && LastOperator != "!" && LastOperator != ")")
                    {
                        Expression.Text += _InputNumber.DisplayNumber;
                        FinalExpression += _InputNumber.ExprNumber;
                    }
                    for (int i = 0; i < BracketCounter; i++)
                    {
                        Expression.Text += ")";
                        FinalExpression += ")";
                        LastOperator = ")";
                    }
                }
                if (IsWaitingForClosure)
                {
                    IsWaitingForClosure = false;
                    Expression.Text += ")";
                    FinalExpression += "))";
                }
            } else {
                Clear_Click(sender, e);
                return;
            }

            //Expression.Text += _InputNumber.DisplayNumber;
            //FinalExpression += _InputNumber.ExprNumber;

            IsEqualityClicked = true;

            SolveResult code = MathLib.Solve(FinalExpression, out double result);

            string strRes = result.ToString("0." + new string('#', 48)).Replace(',', '.');
            _InputNumber = strRes;

            IsErrorOccurred = code != SolveResult.Success;

            switch (code) {
                case SolveResult.Success:
                    Result.Text = strRes;
                    //Expression.Text = strRes;
                    FinalExpression = strRes;
                    break;
                case SolveResult.DivisionByZero:
                    Result.Text = "Division by zero";
                    break;
                case SolveResult.DoubleOverflow:
                    Result.Text = "Double overflow";
                    break;
                case SolveResult.InvalidExpression:
                    Result.Text = "Invalid Expression";
                    break;
                case SolveResult.NotANumber:
                    Result.Text = "Not a number";
                    break;
                case SolveResult.UnpairedBrackets:
                    Result.Text = "Unpaired brackets";
                    break;
                case SolveResult.UnknownError:
                    Result.Text = "Unknown error";
                    break;
            }
            IsLastOperatorUnary = false;
            IsLastNumber = true;
            IsLastNumberClick = true;
            BracketCounter = 0;
            UpdateClearButton();
        }

        /// <summary>
        /// Event handler for calculator's operator buttons.
        /// </summary>
        /// <param name="sender">A button</param>
        /// <param name="e">Default arguments</param>
        private void Operator_Click(object sender, RoutedEventArgs e) {
            if (IsErrorOccurred) Clear_Click(sender, e);

            if (!(sender is Button button)) return;
            if (button.Content == null) return;
            
            // Another nullability warning?
            string op = button.Content.ToString() ?? "";
            
            bool _OnlyClose = false;
            ResetInput = true;
            bool _IsEqualityClicked = IsEqualityClicked;
            IsEqualityClicked = false;
            IsOperatorClicked = true;
            IsLastOperatorSimple = true;
            IsLastNumberClick = false;

            if (_IsEqualityClicked)
            {
                Expression.Text = "";
                FinalExpression = Expression.Text;
            }


            if (op == "^ 2") {
                if (IsLastOperatorUnary && LastOperator != ")" && LastOperator != "^ 2" && LastOperator != "sqrt") return;
                if (IsWaitingForClosure) return;
                if (LastOperator == ")" || LastOperator == "^ 2" || LastOperator == "sqrt")
                {
                    Expression.Text += $"^2";
                    FinalExpression += $"^2";
                }
                else
                {
                    Expression.Text += $"{_InputNumber.DisplayNumber}^2";
                    FinalExpression += $"{_InputNumber.ExprNumber}^2";
                }
                IsLastOperatorSimple = false;
                IsLastOperatorUnary = true;
                IsLastNumber = true;
                IsBracketAllowed = true;
                _OnlyClose = true;
            }

            else if (op == "sqrt") {
                if (IsLastOperatorUnary) return;
                if (IsWaitingForClosure) return;
                // if (_InputNumber.DisplayNumber.Contains('-')) return;
                Expression.Text += $"sqrt({_InputNumber.DisplayNumber})";
                FinalExpression += $"(sqrt({_InputNumber.ExprNumber}))";
                IsLastOperatorSimple = false;
                IsLastOperatorUnary = true;
                IsLastNumber = true;
                IsBracketAllowed = true;
                _OnlyClose = true;
            }

            else if (op == "nqrt") {
                if (IsLastOperatorUnary) return;
                if (IsWaitingForClosure) return;
                // if (_InputNumber.DisplayNumber.Contains('-')) return;
                Expression.Text += $"({_InputNumber.DisplayNumber})^(1/";
                FinalExpression += $"(({_InputNumber.ExprNumber})^(1/";
                IsWaitingForClosure = true;
                IsLastOperatorSimple = false;
                IsLastOperatorUnary = false;
                IsLastNumber = false;
                // IsBracketAllowed = false;
                _OnlyClose = true;
            }
            else if (op == "!")
            {
                if (IsWaitingForClosure) return;
                if (_InputNumber.DisplayNumber.Contains('.')) return;
                // if (_InputNumber.DisplayNumber.Contains('-')) return;
                //if (IsLastOperatorUnary && LastOperator != ")") return;
                if (IsLastOperatorUnary || LastOperator == ")") return;
                //if (LastOperator != ")")
                //{
                Expression.Text += $"{_InputNumber.DisplayNumber}!";
                FinalExpression += $"{_InputNumber.ExprNumber}!";
                //}
                //else
                //{
                //   Expression.Text += $"!";
                //   FinalExpression += $"!";
                //}
                IsLastOperatorSimple = false;
                IsLastOperatorUnary = true;
                IsLastNumber = true;
                IsBracketAllowed = true;
                _OnlyClose = true;
            }
            else
            {
                if (LastOperator != "^ 2" && LastOperator != "sqrt" && LastOperator != "!" && (LastOperator != ")" || _IsEqualityClicked || LastOperator == "("))
                {
                    Expression.Text += $"{_InputNumber.DisplayNumber}";
                    FinalExpression += $"{_InputNumber.ExprNumber}";
                }
                else if (_IsEqualityClicked)
                {
                    Expression.Text += _InputNumber.DisplayNumber;
                    FinalExpression += _InputNumber.ExprNumber;
                }

                if (IsWaitingForClosure)
                {
                    IsWaitingForClosure = false;
                    Expression.Text += ")";
                    FinalExpression += "))";
                    IsLastNumber = true;
                    IsBracketAllowed = true;
                    _OnlyClose = true;
                }

                Expression.Text += $" {op} ";
                FinalExpression += $" {op} ";
                IsLastOperatorUnary = false;
                IsLastNumber = false;
                IsBracketAllowed = true;
            }
            OnlyClose = _OnlyClose;
            UpdateClearButton();
            LastOperator = op;
        }

        /// <summary>
        /// Event handler for calculator's Clear button.
        /// </summary>
        /// <param name="sender">A button</param>
        /// <param name="e">Default arguments</param>
        private void Clear_Click(object sender, RoutedEventArgs e) {
            if (ButtonC.Content == null) return;
            if (ButtonC.Content.ToString() == "CE") {
                Result.Text = "0";
                IsLastNumberClick = true;
                _InputNumber = Result.Text;
                ButtonC.Content = "C";
            } else {
                Result.Text = "0";
                _InputNumber = Result.Text;
                Expression.Text = "";
                FinalExpression = "";
                _LastNumber = "0";
                IsOperatorClicked = false;
                IsEqualityClicked = false;
                IsWaitingForClosure = false;
                IsErrorOccurred = false;
                IsLastOperatorSimple = true;
                IsLastOperatorUnary = false;
                IsLastNumberClick = true;
                IsLastNumber = false;
                OnlyClose = true;
                BracketCounter = 0;
                LastOperator = "";
            }
        }
        
        /// <summary>
        /// Event handler for calculator's Delete button.
        /// </summary>
        /// <param name="sender">A button</param>
        /// <param name="e">Default arguments</param>
        private void Delete_Click(object sender, RoutedEventArgs e) {
            if (IsErrorOccurred) Clear_Click(sender, e);

            Result.Text = Result.Text.Remove(Result.Text.Length - 1, 1);
            _InputNumber = Result.Text;
            if (IsEqualityClicked)
                Expression.Text = "";

            IsEqualityClicked = false;
            IsLastNumberClick = true;

            if (Result.Text == "") {
                Result.Text = "0";
                _InputNumber = Result.Text;
            }
        }
        
        /// <summary>
        /// Event handler for calculator's Brackets' buttons.
        /// </summary>
        /// <param name="sender">A button</param>
        /// <param name="e">Default arguments</param>
        private void Bracket_Click(object sender, RoutedEventArgs e)
        {
            if (IsErrorOccurred) Clear_Click(sender, e);

            if (!(sender is Button button)) return;
            if (button.Content == null) return;

            string content = button.Content.ToString() ?? "";

            if (content == "(" && IsBracketAllowed)
            {
                if (IsEqualityClicked)
                {
                    Expression.Text = Result.Text;
                    FinalExpression = Expression.Text;
                    IsLastNumber = true;
                    IsLastNumberClick = true;
                }
                BracketCounter += 1;
                if (IsWaitingForClosure)
                {
                    Expression.Text += $"{_InputNumber.DisplayNumber})";
                    FinalExpression += $"{_InputNumber.ExprNumber}))";
                }
                if (IsLastNumber && LastOperator != ")")
                {
                    if (IsLastNumberClick && !IsEqualityClicked && !IsWaitingForClosure)
                    {
                        Expression.Text += $"{_InputNumber.DisplayNumber} * (";
                        FinalExpression += $"{_InputNumber.ExprNumber} * (";
                    }
                    else
                    {
                        Expression.Text += $" * (";
                        FinalExpression += $" * (";
                    }
                }
                else if (IsLastNumber && LastOperator == ")" || IsWaitingForClosure)
                {
                    Expression.Text += $" * (";
                    FinalExpression += $" * (";
                }
                else
                {
                    Expression.Text += "(";
                    FinalExpression += "(";
                }
                IsLastNumberClick = false;
                IsLastNumber = false;
                ResetInput = true;
                IsWaitingForClosure = false;
                IsLastOperatorUnary = false;
                LastOperator = "(";
                OnlyClose = false;
            }
            if (content == ")" && BracketCounter != 0)
            {
                if (LastOperator == ")") OnlyClose = true;
                BracketCounter -= 1;
                LastOperator = ")";
                //if (IsLastNumber)
                //{
                //    Expression.Text += ")";
                //    FinalExpression += ")";
                //}
                //else
                //{
                if (IsWaitingForClosure)
                {
                    Expression.Text += $"{_InputNumber.DisplayNumber})";
                    FinalExpression += $"{_InputNumber.ExprNumber}))"; ;
                    IsWaitingForClosure = false;
                }
                if (OnlyClose)
                {
                    Expression.Text += ")";
                    FinalExpression += ")";
                    OnlyClose = false;
                }
                else
                {
                    Expression.Text += $"{_InputNumber.DisplayNumber})";
                    FinalExpression += $"{_InputNumber.ExprNumber})";
                }
                IsLastNumber = true;
                IsLastOperatorUnary = true;
                //}
            }
            IsEqualityClicked = false;
        }
        
        /// <summary>
        /// Event handler for calculator's const buttons.
        /// </summary>
        /// <param name="sender">A button</param>
        /// <param name="e">Default arguments</param>
        private void Const_Click(object sender, RoutedEventArgs e)
        {
            if (IsErrorOccurred) Clear_Click(sender, e);

            if (!(sender is Button button)) return;
            if (button.Content == null) return;

            string content = button.Content.ToString() ?? "";

            if (IsEqualityClicked || IsErrorOccurred)
            {
                Expression.Text = "";
                FinalExpression = "";
                _InputNumber = "";
            }
            if (content == "pi")
            {
                _InputNumber = MathLib.Pi;
            }
            else if (content == "e")
            {
                _InputNumber = MathLib.Exp;
            }
            IsErrorOccurred = false;
            ResetInput = true;

            UpdateResultField();
            UpdateClearButton();

            IsBracketAllowed = true;
            IsLastNumberClick = true;
            IsLastNumber = true;
            IsEqualityClicked = false;
            _LastNumber = Result.Text;
        }
        
        /// <summary>
        /// Global keyboard hook for Enter key.
        /// </summary>
        /// <param name="sender">A button</param>
        /// <param name="e">Default arguments</param>
        private void GlobalPreviewKeyDown(object sender, KeyRoutedEventArgs e) {
            if (e.Key == VirtualKey.Enter) {
                Eq_Click(sender, e);
                e.Handled = true;
            }
        }
    }
}
