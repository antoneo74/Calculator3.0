#include "rpn.h"

namespace s21 {

/**
 * @brief Generating reverse polish notation based on algorithm Dijkstra
 */
std::pair<std::list<Node>, bool> Notation::CreateRPN(std::list<Node> tokens) {
  Clear();
  tokens_ = tokens;

  while (!tokens_.empty()) {
    auto token = tokens_.begin()->action;

    switch (token) {
      case kNumber:
        rpn_.push_back(tokens_.front());
        break;
      case kSin:
      case kCos:
      case kTan:
      case kAsin:
      case kAcos:
      case kAtan:
      case kLog:
      case kLog10:
      case kSqrt:
      case kOpeningBracket:
        temporary_stack_.push_front(tokens_.front());
        break;
      case s21::kClosingBracket:
        if (ClosingBracketItem()) {
          Clear();
          return std::make_pair(rpn_, true);
        }
        break;
      default:
        DefaultItem();
        break;
    }
    tokens_.pop_front();
  }

  while (!temporary_stack_.empty()) {
    if (temporary_stack_.begin()->action == kOpeningBracket) {
      Clear();
      return std::make_pair(rpn_, true);
    }
    rpn_.push_back(temporary_stack_.front());
    temporary_stack_.pop_front();
  }
  return std::make_pair(rpn_, false);
}

/**
 * @brief When finding a closing bracket, push all operators up to the opening
 * bracket from the temporary_stack_, and remove the opening bracket from the
 * stack.
 */
bool Notation::ClosingBracketItem() {
  while (!temporary_stack_.empty() &&
         temporary_stack_.begin()->action != kOpeningBracket) {
    rpn_.push_back(temporary_stack_.front());
    temporary_stack_.pop_front();
  }
  if (temporary_stack_.empty())
    return 1;
  else
    temporary_stack_.pop_front();
  return 0;
}

/**
 * @brief push into the output rpn_ list from the temporary_stack_ list all
 * operators that have a priority higher than the one under consideration;
 */
void Notation::DefaultItem() {
  while (!temporary_stack_.empty() &&
         tokens_.begin()->priority <= temporary_stack_.begin()->priority) {
    rpn_.push_back(temporary_stack_.front());
    temporary_stack_.pop_front();
  }
  temporary_stack_.push_front(tokens_.front());
}

/**
 * @brief clearing all class lists
 */
void Notation::Clear() {
  tokens_.clear();
  rpn_.clear();
  temporary_stack_.clear();
}
}  // namespace s21
