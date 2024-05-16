#include "pch.h"
#include "calculator.h"

using namespace s21;

Result Calculator::Execute(std::string& expression, double x) {
	auto res = parser_.GetResult(expression, x);
	int error = res.second;
	if (!error) {
		tokens_ = res.first;
		auto rpn = notation_.CreateRPN(tokens_);
		error = rpn.second;
		if (!error) {
			rpn_ = rpn.first;
			error = Calculate();
		}
	}
	auto result = std::make_pair(error, GetResult());
	Clear();

	data.error = result.first;
	data.res = result.second;
	return data;
}

double Calculator::CalculateAction(double first_operand, double second_operand,
	Actions action) {
	double result = 0;
	switch (action) {
	case Actions::kAdd:
		result = first_operand + second_operand;
		break;
	case Actions::kSub:
		result = first_operand - second_operand;
		break;
	case Actions::kMul:
		result = first_operand * second_operand;
		break;
	case Actions::kDiv:
		result = first_operand / second_operand;
		break;
	case Actions::kPow:
		result = pow(first_operand, second_operand);
		break;
	case Actions::kFmod:
		result = fmod(first_operand, second_operand);
		break;
	case Actions::kSin:
		result = sin(first_operand);
		break;
	case Actions::kCos:
		result = cos(first_operand);
		break;
	case Actions::kTan:
		result = tan(first_operand);
		break;
	case Actions::kAsin:
		result = asin(first_operand);
		break;
	case Actions::kAcos:
		result = acos(first_operand);
		break;
	case Actions::kAtan:
		result = atan(first_operand);
		break;
	case Actions::kLog:
		result = log(first_operand);
		break;
	case Actions::kLog10:
		result = log10(first_operand);
		break;
	case Actions::kSqrt:
		result = sqrt(first_operand);
		break;
	}
	return result;
}

bool Calculator::Calculate() {
	bool error = 0;
	while (!rpn_.empty()) {
		Node item = rpn_.front();
		switch (item.action) {
		case Actions::kNumber:
			stack_.push_front(rpn_.front());
			break;
		case Actions::kAdd:
			if (item.binary) error = CalculateItem(item);
			break;
		case Actions::kSub:
			if (item.binary)
				error = CalculateItem(item);
			else
				stack_.begin()->value *= -1;
			break;
		default:
			error = CalculateItem(item);
			break;
		}
		if (error) return error;
		rpn_.pop_front();
	}
	result_ = GetValueFromStack().first;
	return error;
}

void s21::Calculator::Clear() {
	rpn_.clear();
	stack_.clear();
	tokens_.clear();
	result_ = 0;
}

std::pair<double, bool> s21::Calculator::GetValueFromStack() {
	std::pair<double, bool> value;
	if (stack_.empty())
		value = std::make_pair(0, true);
	else {
		value = std::make_pair(stack_.begin()->value, false);
		stack_.pop_front();
	}
	return value;
}

bool s21::Calculator::CalculateItem(Node item) {
	double first_operand = 0;
	double second_operand = 0;
	bool error = 0;
	if (item.binary) second_operand = GetValueFromStack().first;

	auto value = GetValueFromStack();
	error = value.second;
	if (error) {
		return 1;
	}
	first_operand = value.first;

	double res = CalculateAction(first_operand, second_operand, item.action);
	stack_.push_front(Node{ Actions::kNumber, Priority::kLowest, false, res });
	return 0;
}

double Calculator::GetResult() { return result_; }
