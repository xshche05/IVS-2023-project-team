namespace IvsCalc.Classes; 

/// <summary>
/// Class to represent numbers in the Calculator
/// </summary>
public struct InputNumber {
    private string _source;

    private string _number;
    private bool _sign;

    /// <summary>
    /// Number to display in the Calculator
    /// </summary>
    public string DisplayNumber => _sign ? $"-{_number}" : $"{_number}";
    
    /// <summary>
    /// Number to add to expression
    /// </summary>
    public string ExprNumber => _sign ? $"(0 - {_number})" : $"{_number}";

    /// <summary>
    /// Input number constructor
    /// </summary>
    /// <param name="number">Number to assign</param>
    /// <param name="sign">Number's sign</param>
    public InputNumber(string number, bool sign) : this() {
        _number = number;
        _sign = sign;

        _source = DisplayNumber;
    }

    /// <summary>
    /// Chages the sign of the number
    /// </summary>
    public void ChangeSign() => _sign = !_sign;

    /// <summary>
    /// Operator to convert int to InputNumber
    /// </summary>
    /// <param name="num">Number to convert</param>
    /// <returns>An instance of <see cref="InputNumber"/></returns>
    public static implicit operator InputNumber(int num) {
        bool sign = num < 0;
        return new InputNumber($"{num}".Replace("-", ""), sign);
    }

    /// <summary>
    /// Operator to convert string to InputNumber
    /// </summary>
    /// <param name="str">Number to convert</param>
    /// <returns>An instance of <see cref="InputNumber"/></returns>
    public static implicit operator InputNumber(string str) {
        bool sign = str.Contains('-');
        return new InputNumber(str.Replace("-", ""), sign);
    }

    /// <summary>
    /// Operator to add two InputNumbers together
    /// </summary>
    /// <param name="left">Left side of addition</param>
    /// <param name="right">Right side of addition</param>
    /// <returns>An instance of <see cref="InputNumber"/></returns>
    public static InputNumber operator +(InputNumber left, InputNumber right) {
        return new InputNumber(left._number + right._number, left._sign || right._sign);
    }
}