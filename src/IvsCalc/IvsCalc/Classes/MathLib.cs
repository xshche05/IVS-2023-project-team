using System;
using System.Runtime.InteropServices;

namespace IvsCalc.Classes;
    
public enum SolveResult {
    Success = 0,
    InvalidExpression = 1,
    UnpairedBrackets = 2,
    DivisionByZero = 3,
    NotANumber = 4,
    DoubleOverflow = 5,

    UnknownError = -1
}

/// <summary>
/// Class wrapper for IvsCalcMathLib.dll
/// </summary>
public static class MathLib {
    /// <summary>
    /// String representation of constant PI
    /// </summary>
    public static readonly string Pi  = Math.PI.ToString("#." + new string('#', 12)).Replace(',', '.').Remove(12);
    /// <summary>
    /// String representation of constant E
    /// </summary>
    public static readonly string Exp = Math.E.ToString("#." + new string('#', 12)).Replace(',', '.').Remove(12);
        
    /// <summary>
    /// Function import from IvsCalcMathLib_$(Configuration)_$(Platform).dll
    /// </summary>
    /// <param name="str">Math expression to solve</param>
    /// <param name="result">Out param with calculation result</param>
    /// <returns>Number indicating success or specific error</returns>
#if DEBUG
    
#if WIN32
    [DllImport("IvsCalcMathLib_Debug_x86.dll", CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport("IvsCalcMathLib_Debug_x64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
    
#else

#if WIN32
    [DllImport("IvsCalcMathLib_Release_x86.dll", CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport("IvsCalcMathLib_Release_x64.dll", CallingConvention = CallingConvention.Cdecl)]
#endif
    
#endif
    private static extern int ParseExpressionToFloat(string str, out double result);

    /// <summary>
    /// Calls the dll function to solve the expression and returns the code of solving
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static SolveResult Solve(string expression, out double result) {
        return (SolveResult)ParseExpressionToFloat(expression, out result);
    }
}