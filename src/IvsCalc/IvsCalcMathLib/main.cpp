/**
 *  \file main.cpp
 *  \brief File for various tests and fast prototyping
 *  \author Konstantin Romanets (xroman18(at)stud.fit.vutbr.cz
 *  \date 22.04.2023
 */

#include "IvsCalcMathLib.h"
#include <chrono>

using namespace std::chrono;

#define MEASURE_FUNC(F) \
    auto start = high_resolution_clock::now(); \
    F;              \
    auto stop = high_resolution_clock::now();  \
    auto duration = duration_cast<microseconds>(stop - start) \

#define MEASURE(F) \
    auto start = high_resolution_clock::now(); \
    F              \
    auto stop = high_resolution_clock::now();  \
    auto duration = duration_cast<microseconds>(stop - start) \


//bool isOperator(const std::string& c) {
//    return c == "+" || c == "-" || c == "*" || c == "/" || c == "^";
//}

int main() {

    double res;

    int code = ParseExpressionToFloat("3nqrt(8)", &res);

    std::cout << code << " :: " << res << std::endl;

    return 0;
}