#include "pch.h"
#include "parser.h"
using namespace s21;

std::pair<std::list<Node>, bool> Parser::GetResult(const std::string expresion,
	double x_value) {
	Initialization(expresion, x_value);
	return std::make_pair(tokens_, Parse());
}

void s21::Parser::Initialization(const std::string expresion, double x_value) {
	expresion_ = expresion;
	x_value_ = x_value;
	index_ = 0;
	if (!tokens_.empty()) {
		tokens_.clear();
	}
}

bool Parser::Parse() {
	int error = 0;
	int action = 1;
	int operand = 0;

	while (index_ < expresion_.size()) {
		if (expresion_[index_] == 'X') {
			CheckAction(action, operand);
			tokens_.push_back(
				Node{ Actions::kNumber, Priority::kLowest, false, x_value_ });
			action = 0;
			operand = 1;
			++index_;
		}
		else if (expresion_[index_] == '.' || std::isdigit(expresion_[index_])) {
			CheckAction(action, operand);
			error = CreateNumber();
			operand = 1;
			action = 0;
		}
		else if (expresion_[index_] == ')') {
			error = ParseAction(action, operand);
			operand = 1;
			action = 0;
		}
		else if (expresion_[index_] == ' ') {
			++index_;
		}
		else {
			error = ParseAction(action, operand);
			action = 1;
			operand = 0;
		}
		if (error) return error;
	}
	return error;
}

void Parser::CheckAction(int action, int operand) {
	if (operand && !action) {
		tokens_.push_back(Node{ Actions::kMul, Priority::kMedium, true, 0 });
	}
}

bool Parser::CreateNumber() {
	try {
		size_t end = index_;
		std::string str = expresion_.substr(index_);
		double number = std::stod(str, &end);
		tokens_.push_back(Node{ Actions::kNumber, Priority::kLowest, false, number });
		index_ = index_ + end;
		return 0;
	}
	catch (const std::exception&) {
		return 1;
	}
}

bool Parser::ParseAction(int action_is_active, int operand) {
	int error = 0;
	if (expresion_.substr(index_, 3) == "sin") {
		CheckAction(action_is_active, operand);
		tokens_.push_back(Node{ Actions::kSin, Priority::kHigh, false, 0 });
		index_ += 3;
	}
	else if (expresion_.substr(index_, 3) == "cos") {
		CheckAction(action_is_active, operand);
		tokens_.push_back(Node{ Actions::kCos, Priority::kHigh, false, 0 });
		index_ += 3;
	}
	else if (expresion_.substr(index_, 3) == "tan") {
		CheckAction(action_is_active, operand);
		tokens_.push_back(Node{ Actions::kTan, Priority::kHigh, false, 0 });
		index_ += 3;
	}
	else if (expresion_.substr(index_, 3) == "mod") {
		tokens_.push_back(Node{ Actions::kFmod, Priority::kMedium, true, 0 });
		index_ += 3;
	}
	else if (expresion_.substr(index_, 2) == "ln") {
		CheckAction(action_is_active, operand);
		tokens_.push_back(Node{ Actions::kLog, Priority::kHigh, false, 0 });
		index_ += 2;
	}
	else if (expresion_.substr(index_, 3) == "log") {
		CheckAction(action_is_active, operand);
		tokens_.push_back(Node{ Actions::kLog10, Priority::kHigh, false, 0 });
		index_ += 3;
	}
	else if (expresion_.substr(index_, 4) == "asin") {
		CheckAction(action_is_active, operand);
		tokens_.push_back(Node{ Actions::kAsin, Priority::kHigh, false, 0 });
		index_ += 4;
	}
	else if (expresion_.substr(index_, 4) == "acos") {
		CheckAction(action_is_active, operand);
		tokens_.push_back(Node{ Actions::kAcos, Priority::kHigh, false, 0 });
		index_ += 4;
	}
	else if (expresion_.substr(index_, 4) == "atan") {
		CheckAction(action_is_active, operand);
		tokens_.push_back(Node{ Actions::kAtan, Priority::kHigh, false, 0 });
		index_ += 4;
	}
	else if (expresion_.substr(index_, 4) == "sqrt") {
		CheckAction(action_is_active, operand);
		tokens_.push_back(Node{ Actions::kSqrt, Priority::kHigh, false, 0 });
		index_ += 4;
	}
	else if (expresion_[index_] == '(') {
		CheckAction(action_is_active, operand);
		tokens_.push_back(
			Node{ Actions::kOpeningBracket, Priority::kLowest, false, 0 });
		++index_;
	}
	else if (expresion_[index_] == ')') {
		tokens_.push_back(
			Node{ Actions::kClosingBracket, Priority::kLowest, false, 0 });
		++index_;
	}
	else if (expresion_[index_] == '+' || expresion_[index_] == '-') {
		Actions action = expresion_[index_] == '-' ? Actions::kSub : Actions::kAdd;
		if (action_is_active) {
			tokens_.push_back(Node{ action, Priority::kHighest, false, 0 });
		}
		else {
			tokens_.push_back(Node{ action, Priority::kLow, true, 0 });
		}
		++index_;
	}
	else if (expresion_[index_] == '*' || expresion_[index_] == '/') {
		Actions action = expresion_[index_] == '*' ? Actions::kMul : Actions::kDiv;
		tokens_.push_back(Node{ action, Priority::kMedium, true, 0 });
		++index_;
	}
	else if (expresion_[index_] == '^') {
		tokens_.push_back(Node{ Actions::kPow, Priority::kHigh, true, 0 });
		++index_;
	}
	else {
		error = 1;
	}
	return error;
}
