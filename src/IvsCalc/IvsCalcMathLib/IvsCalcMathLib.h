#pragma once

#include <iostream>
#include <stack>
#include <string>
#include <cmath>
#include <regex>
#include <utility>
#include <iomanip>
#include <sstream>

#ifdef _WIN32
#define EXPORT extern "C" __declspec(dllexport)
#else
#define EXPORT extern "C"
#endif

/**
 * \brief An enum to represent the result of the calculation
 */
enum SolveResult {
    Success = 0,
    InvalidExpression = 1,
    UnpairedBrackets = 2,
    DivisionByZero = 3,
    NotANumber = 4,
    DoubleOverflow = 5,

    UnknownError = -1,
};

/**
 * \brief A class to represent the exception thrown by the calculation
 */
class MathException : public std::exception {
public:
    /**
     * \brief The result of the calculation
     */
    SolveResult code;
    /**
     * \brief Explicit constructor
     * \param _code The result of the calculation
     */
    explicit MathException(SolveResult _code) : code(_code) {}
};

EXPORT int ParseExpressionToFloat(const char* str, double *result);