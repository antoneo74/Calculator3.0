#ifndef CPP3_SMARTCALC_CALCULATOR_H
#define CPP3_SMARTCALC_CALCULATOR_H

#include <cmath>
#include <cstdint>

#include "parser.h"
#include "rpn.h"

namespace s21
{
	struct Result
	{
		bool error;
		double res;
	};

	class Calculator {
	public:
		Result data;
		Result Execute(std::string& expression, double x);

	private:
		std::list<Node> rpn_;
		std::list<Node> stack_;
		std::list<Node> tokens_;

		double result_ = 0;
		Parser parser_;
		Notation notation_;

		double CalculateAction(double first_operand, double second_operand,
			Actions action);
		double GetResult();
		bool Calculate();
		bool CalculateItem(Node item);
		void Clear();
		std::pair<double, bool> GetValueFromStack();
	};

	extern "C" __declspec(dllexport) Result Calculate(Calculator * item, const char* str, double x)
	{
		std::string expression(str);
		return item->Execute(expression, x);
	}

	extern "C" __declspec(dllexport) void* Constructor()
	{
		return (void*) new Calculator();
	}

}  // namespace s21

#endif  // CPP3_SMARTCALC_CALCULATOR_H
