//  Contains a description of the parser class and its methods.

#ifndef CPP3_SMARTCALC_PARSER_PARSER_H
#define CPP3_SMARTCALC_PARSER_PARSER_H

#include <iostream>
#include <list>
#include <string>

#include "../Model/actions.h"
#include "../Model/node.h"
#include "../Model/priority.h"

namespace s21 {
/**
 * @brief the class is responsible for generating list of tokens from input
 * string
 */
class Parser {
 public:
  /**
   * @brief main method creating list of tokens
   * @param input string and the value of the variable "X"
   * @return pair of list of tokens and error value
   */
  std::pair<std::list<Node>, bool> GetResult(const std::string expresion,
                                             double x_value);

 private:
  /**
   * @brief field of input string
   */
  std::string expresion_;
  /**
   * @brief field of tokens list
   */
  std::list<Node> tokens_;
  /**
   * @brief field of variable "X"
   */
  double x_value_ = 0;
  /**
   * @brief field of index
   */
  size_t index_ = 0;

  /**
   * @brief assigning default values to class fields
   */
  void Initialization(const std::string expresion, double x_value);
  /**
   * @brief Parsing start function
   */
  bool Parse();
  /**
   * @brief Parsing operators
   */
  bool ParseAction(int action_is_active, int operand);
  /**
   * @brief Parsing numbers
   */
  bool CreateNumber();
  /**
   * @brief missing multiplication check function
   */
  void CheckAction(int action, int operand);
};
}  // namespace s21
#endif  // CPP3_SMARTCALC_PARSER_PARSER_H
