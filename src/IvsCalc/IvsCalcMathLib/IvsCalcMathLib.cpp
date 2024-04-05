/**
 *  \file IvsCalcMathLib.cpp
 *  \brief Implementation of mathematical parser library for Ivs Calculator
 *  \author Konstantin Romanets (xroman18(at)stud.fit.vutbr.cz)
 *  \date 22.04.2023
 */

#include "pch.h"
#include "IvsCalcMathLib.h"

/**
 * \brief Checks whether the given character (as string) is an operator
 * \param c Character as string
 * \return True if character is operator
 */
bool isOperator(const std::string& c) {
    return c == "+" || c == "-" || c == "*" || c == "/" || c == "^";
}

/**
 * \brief Checks whether the given character (as string) is left associative
 * \param c Character as string
 * \return True if character is left associative
 */
bool leftAssoc(const std::string& c) {
    return c != "^";
}

/**
 * \brief Returns the priority of the operator
 * \param c Character
 * \return Priority of the given operator
 */
int priority(char c) {
    switch (c) {
        case '^': return 3;
        case '*':
        case '/': return 2;
        case '+':
        case '-': return 1;
        default: return 0;
    }
}

/**
 * \brief Same as \see `priority(char c)` but for string
 * \param c Character as string
 * \return Priority of the given operator
 */
int priority(const std::string& c) {
    return priority(c[0]);
}

/**
 * \brief Finds `sqrt(%s)` in string and replaces it with `(%s)^(1/2)`
 * \param s Expression
 * \return Modified expression
 */
std::string ReplaceSqrt(std::string s) {
    std::string res;
    size_t pos = s.find("sqrt(");
    int Depth = 0;
    if (pos != std::string::npos) {
        pos += 5;
        for (size_t i = pos; i < s.length(); i++) {
            if (s[i] == ')' && Depth == 0)
                break;
            if (s[i] == '(') Depth++;
            if (s[i] == ')') Depth--;
            
            res += s[i];
        }

        s = s.replace(pos - 5, res.length() + 6, "(" + res + ")^(1/2)");

        if (s.find("sqrt(") != std::string::npos) {
            s = ReplaceSqrt(s);
        }
    }

    return s;
}

/**
 * \brief Finds `%s!` in string and replaces it with a sequence of numbers from 1 to `%s`
 * \param s Expression
 * \return Modified expression 
 */
std::string ReplaceFactorial(std::string s) {
    std::string num, res;

    size_t pos = s.find('!');
    if (pos != std::string::npos) {
        for (int i = (int)pos - 1; i >= 0; i--) {
            if (!isdigit(s[i])) {
                break;
            }

            num += s[i] + num;
        }

        int n = std::stoi(num);
        for (int i = 1; i <= n; i++) {
            if (i == n)
                res += std::to_string(i);
            else
                res += std::to_string(i) + "*";
        }

        s = s.replace(pos - 1, num.length() + 1, "(" + res + ")");

        if (s.find(")!") != std::string::npos) {
            throw MathException(SolveResult::InvalidExpression);
        }

        if (s.find('!') != std::string::npos) {
            s = ReplaceFactorial(s);
        }
    }

    return s;
}

/**
 * \brief Finds `--` in string and replaces it with `+`
 * \param s Expression
 * \return Modified expression 
 */
std::string ReplaceDoubleMinus(std::string s) {
    size_t pos = s.find("--");
    if (pos != std::string::npos) {
        if (isdigit(s[pos - 1]) && s[pos - 1] != '(' || s[pos - 1] == ')') {
            if (s[pos] == '-' && s[pos + 1] == '-') {
                s = s.replace(pos, 2, "+");
            }
        } else {
            s = s.replace(pos, 2, "");
        }

        if (s.find("--") != std::string::npos) {
            s = ReplaceDoubleMinus(s);
        }
    }

    return s;
}

/**
 * \brief Finds `+-` or `-+` in string and replaces it with `-`
 * \param s Expression
 * \return Modified expression 
 */
std::string ReplacePlusMinus(std::string s) {
    size_t pos = s.find("+-");
    if (pos != std::string::npos) {
        s = s.replace(pos, 2, "-");
        if (s.find("+-") != std::string::npos) {
            s = ReplacePlusMinus(s);
        }
    } else {
        pos = s.find("-+");
        if (pos != std::string::npos) {
            s = s.replace(pos, 2, "-");
            if (s.find("-+") != std::string::npos) {
                s = ReplacePlusMinus(s);
            }
        }
    }

    return s;
}

/**
 * \brief Finds next token in expression
 * \param expr Expression
 * \param pos Position to search for
 * \return A next token from `pos`
 */
std::string nextToken(std::string& expr, int& pos) {
    while (pos < expr.length() && expr[pos] == ' ')
        pos++;

    if (pos == expr.length())
        return "";

    std::string b;

    while (
        pos < expr.length()
        && expr[pos] == ' '
        && expr[pos] != '('
        && expr[pos] != ')'
        || isdigit(expr[pos])
        || (expr[pos] == '.')
        || pos != 0 && (
            !isdigit(expr[pos - 1])
            && expr[pos - 1] != ')'
            && expr[pos] == '-'
            && isdigit(expr[pos + 1])
            )
        )
        b += expr[pos++];

    if (!b.empty())
        return b;

    return { expr[pos++] };
}

/**
 * \brief Converts an infix notation expression to a reverse polish notation
 * \param input An infix notation expression
 * \throws MathException when any of the errors occur
 * \return A reverse polish notation expression
 */
std::string CreateRPN(std::string input) {
    std::vector<std::string> S, O;
    std::string tok;

    input.erase(remove(input.begin(), input.end(), ' '), input.end());
    //    input = std::regex_replace(input, std::regex("--"), "+");
    //    input = std::regex_replace(input, std::regex("--\\d+"), "");
    try {
        input = ReplacePlusMinus(input);
        input = ReplaceDoubleMinus(input);
        input = ReplaceSqrt(input);
        input = ReplaceFactorial(input);
    } catch(std::exception& e) {
        throw MathException(SolveResult::InvalidExpression);
    }


    int pos = 0;

    while (!(tok = nextToken(input, pos)).empty()) {
        if (tok == "(")
            S.push_back(tok);
        else if (tok == ")") {
            while (!S.empty() && S[S.size() - 1] != "(") {
                O.push_back(S[S.size() - 1]);
                S.pop_back();
            }
            if (S.empty())
                throw MathException(SolveResult::UnpairedBrackets);
            S.pop_back();
        } else if (isOperator(tok)) {
            while (!S.empty() && isOperator(S[S.size() - 1])
                && ((leftAssoc(tok) && priority(tok) <= priority(S[S.size() - 1]))
                || (!leftAssoc(tok) && priority(tok) <  priority(S[S.size() - 1])))) {
                O.push_back(S[S.size() - 1]);
                S.pop_back();
            }
            S.push_back(tok);
        } else
            O.push_back(tok);
    }

    while (!S.empty()) {
        if (!isOperator(S[S.size() - 1]))
            throw MathException(SolveResult::UnpairedBrackets);
        O.push_back(S[S.size() - 1]);
        S.pop_back();
    }

    if (O.empty())
        throw MathException(SolveResult::InvalidExpression);

    std::string output;

    for (int j = 0; j < O.size(); j++) {
        if (j != 0) output += " ";
        output += O[j];
    }

    return output;
}

/**
 * \brief Checks whether a number is NaN or infinite
 * \param result A number to check
 * \throws MathException of either DoubleOverflow or NotANumber type
 */
void CheckIsNanOrInfinite(double &result) {
    if (std::isinf(result))
        throw MathException(SolveResult::DoubleOverflow);
    if (std::isnan(result))
        throw MathException(SolveResult::NotANumber);
}

/**
 * \brief Calculates a result of a reverse polish notation expression
 * \param rpn A reverse polish notation expression
 * \throws MathException when any of the errors occur
 * \return A calculated result
 */
double ProcessRPN(const std::string& rpn) {
    std::stack<double> stack;
    std::vector<std::string> rpnList;
    std::istringstream iss(rpn);
    std::string token;
    const std::string cOperators = "^+-*/";

    while (std::getline(iss, token, ' ')) {
        if (!token.empty()) {
            rpnList.push_back(token);
        }
    }

    stack.push(std::stod(rpnList[0]));
    for (int i = 1; i < rpnList.size(); i++) {
        if (rpnList[i].empty()) {
            continue;
        }
        if (cOperators.find(rpnList[i]) != std::string::npos) {
            double op1 = stack.top();
            stack.pop();

            if (stack.empty() && rpnList[rpnList.size() - 1] == "-") return -op1;
                
            double op2 = stack.top();
            stack.pop();
            double res = 0;
            switch (rpnList[i][0]) {
                case '+':
                    stack.push(op2 + op1);
                break;
                case '-':
                    stack.push(op2 - op1);
                break;
                case '*':
                    res = op2 * op1;
                    CheckIsNanOrInfinite(res);
                    stack.push(res);
                    break;
                case '/':
                    if (op1 == 0)
                        throw MathException(SolveResult::DivisionByZero);

                    res = op2 / op1;
                    CheckIsNanOrInfinite(res);
                    stack.push(res);
                    break;
                case '^':
                    if (op2 == 0 && op1 < 0)
                        throw MathException(SolveResult::DivisionByZero);

                    res = pow(op2, op1);
                    CheckIsNanOrInfinite(res);
                    stack.push(res);
                    break;
            }
        } else {
            stack.push(std::stod(rpnList[i]));
        }
    }

    if (std::isnan(stack.top()))
        throw MathException(SolveResult::NotANumber);
    
    return stack.top();
}

/**
 * \brief Parses an expression and solves it to double
 * \param str Input string to parse
 * \param result Output result
 * \return SolveResult code 
 */
int ParseExpressionToFloat(const char* str, double* result) {
    try {
        std::string RevPolNotation = CreateRPN(std::string(str));
        *result = ProcessRPN(RevPolNotation);
        return SolveResult::Success;
    } catch (MathException& e) {
        *result = std::numeric_limits<double>::quiet_NaN();
        return e.code;
    } catch (std::exception& e) {
        *result = std::numeric_limits<double>::quiet_NaN();
        return SolveResult::InvalidExpression;
    }
}
