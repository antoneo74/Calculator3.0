#ifndef CPP3_SMARTCALC_MODEL_NODE_H
#define CPP3_SMARTCALC_MODEL_NODE_H

#include "actions.h"
#include "priority.h"

namespace s21 {
	class Node {
	public:
		Actions action;
		Priority priority;
		bool binary;
		double value;

		Node(Actions act, Priority pr, bool bi, double val)
			: action(act), priority(pr), binary(bi), value(val) {}
		Node(const Node& other)
			: action(other.action),
			priority(other.priority),
			binary(other.binary),
			value(other.value) {}
	};
}  // namespace s21
#endif  // CPP3_SMARTCALC_MODEL_NODE_H
