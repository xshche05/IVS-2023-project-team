//
// Created by Spagetik on 14.04.2023.
//

#include <iostream>
#include <vector>
#include <string>
#include <chrono>
#include "../IvsCalc/IvsCalcMathLib/IvsCalcMathLib.h"


using namespace std;

int main()
{
	vector<int> numbers;
	double result;
	string line;
	while (getline(cin, line))
	{
		istringstream iss(line);
		string word;
		while (iss >> word)
		{
			numbers.push_back(stoi(word));
		}
	}

	string expr;
	double x_val;

	for (size_t i = 0; i < numbers.size(); i++)
	{
		expr += to_string(numbers[i]);
		if (i != numbers.size() - 1)
		{
			expr += "+";
		}
	}

	expr = "1/" + to_string(numbers.size()) + "*" + "(" + expr + ")";
    

	ParseExpressionToFloat(expr.c_str(), &x_val);

	expr = "";

	for (size_t i = 0; i < numbers.size(); i++)
	{
		expr += to_string(numbers[i]) + "^2";
		if (i != numbers.size() - 1)
		{
			expr += "+";
		}
	}

	expr = expr + "-" + to_string(numbers.size()) + "*" + to_string(x_val) + "^2";

	expr = "1/" + to_string(numbers.size() - 1) + "*" + "(" + expr + ")";

	expr = "sqrt(" + expr + ")";

	ParseExpressionToFloat(expr.c_str(), &result);

	cout << std::setprecision(25) << result << endl;

	return 0;
}