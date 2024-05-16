//  Contains a description of the reverse polish notation class and its methods.

#ifndef CPP3_SMARTCALC_PARSER_RPN_H
#define CPP3_SMARTCALC_PARSER_RPN_H

#include "parser.h"

namespace s21 {
/**
 * @brief the class is responsible for generating
 * the reverse polish notation
 */
class Notation {
 public:
  /**
   * @brief main method creating rpn
   * @param list of tokens getting from parser
   * @return pair of list of tokens written to rpn and error value
   */
  std::pair<std::list<Node>, bool> CreateRPN(std::list<Node> tokens);

 private:
  /**
   * @brief the resulting list written in reverse polish notation
   */
  std::list<Node> rpn_;
  /**
   * @brief auxiliary list for generating reverse polish notation
   */
  std::list<Node> temporary_stack_;
  /**
   * @brief list of tokens received from the parser
   */
  std::list<Node> tokens_;

  /**
   * @brief function for processing the kClosingBracket token
   */
  bool ClosingBracketItem();
  /**
   * @brief function for processing kAdd, kSub, kMul, kDiv, kPow,
   * kFmod and kOpeningBracket tokens
   */
  void DefaultItem();
  /**
   * @brief clearing all class lists
   */
  void Clear();
};
}  // namespace s21
#endif  // CPP3_SMARTCALC_PARSER_RPN_H
