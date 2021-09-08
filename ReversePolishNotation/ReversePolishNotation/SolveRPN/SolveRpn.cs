using System;
using System.Collections.Generic;
using System.Globalization;
using ReversePolishNotation.ConvertToRPN;

namespace ReversePolishNotation.SolveRPN
{
    public class SolveRpn
    {
        public double Answer { get; }
        public List<string> Expression { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <exception cref="Exception"></exception>
        public SolveRpn(List<string> expression)
        {
            Expression = expression;
            Answer = Solve();
        }

        private double Solve()
        {
            Stack<double> stack = new Stack<double>();
            foreach (var exp in Expression)
            {
                if (OperationHelper.IsOperation(exp) && !OperationHelper.IsBracket(exp))
                {
                    if (stack.Count < 2)
                    {
                        throw new Exception("Invalid rpn-expression");
                    }
                    else
                    {
                        var secondNum = stack.Pop();
                        var firstNum = stack.Pop();
                        stack.Push(OperationHelper.Execute(exp, firstNum, secondNum));
                        continue;
                    }
                }

                if (double.TryParse(exp, NumberStyles.Any, new NumberFormatInfo(), out var num))
                {
                    stack.Push(num);
                    continue;
                }

                throw new Exception("Invalid rpn-expression");
            }

            return stack.Pop();
        }
    }
}
