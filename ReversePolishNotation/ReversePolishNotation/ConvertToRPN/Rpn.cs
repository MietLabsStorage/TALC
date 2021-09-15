using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ReversePolishNotation.ConvertToRPN
{
    public static class Rpn
    {
        public static string ToString(List<string> expression) => string.Join(" ", expression);

        public static string Separator => ",";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public static List<string> Convert(string str)
        {
            CheckStringExpressionByBrackets(str);
            var expressions = PrepareString(str);
            CheckListExpressionByFirstItem(expressions);
            CheckListExpressionsByOperationsAndNumsSequence(expressions);
            RemoveSeparators(ref expressions);

            List<string> rpnStr = new List<string>();
            Stack<string> stack = new Stack<string>();

            foreach (var expression in expressions)
            {
                if (!OperationHelper.IsOperation(expression))
                {
                    rpnStr.Add(expression);
                    continue;
                }

                if (stack.Count == 0)
                {
                    stack.Push(expression);
                    continue;
                }

                if (expression == OperationHelper.Convert(Operation.OpenBracket))
                {
                    stack.Push(expression);
                    continue;
                }

                if (expression == OperationHelper.Convert(Operation.CloseBracket))
                {
                    while (stack.Peek() != OperationHelper.Convert(Operation.OpenBracket))
                    {
                        rpnStr.Add(stack.Pop());
                        if (stack.Count == 0)
                        {
                            break;
                        }
                    }
                    stack.Pop();
                    if(stack.Count!=0 && stack.Peek() == OperationHelper.Convert(Operation.UserOperation))
                    {
                        rpnStr.Add(stack.Pop());
                    }
                    continue;
                }

                while (!OperationHelper.IsBracket(stack.Peek()) 
                       && OperationHelper.GetPrioroty(stack.Peek()) >= OperationHelper.GetPrioroty(expression))
                {
                    rpnStr.Add(stack.Pop());
                    if (stack.Count == 0)
                    {
                        break;
                    }
                }
                stack.Push(expression);
            }

            while (stack.Count != 0)
            {
                rpnStr.Add(stack.Pop());
            }

            CheckExpressionByIrrelevantSymbols(expressions);

            return rpnStr;
        }

        private static List<string> PrepareString(string str)
        {
            var syms = str.ToCharArray();
            StringBuilder stringBuilder = new StringBuilder();

            foreach (var sym in syms)
            {
                if (OperationHelper.IsOperation(sym.ToString()))
                {
                    if ((sym.ToString() == OperationHelper.Convert(Operation.Substract)
                         || sym.ToString() == OperationHelper.Convert(Operation.Sum))
                         && stringBuilder.Length > 1
                         && stringBuilder[stringBuilder.Length - 1].ToString() == "e")
                    {
                        stringBuilder.Append($"{sym}");
                        continue;
                    }

                    // (-1 -> (0-1 
                    // 2 -1 -> 2 0-1
                    if (sym.ToString() == OperationHelper.Convert(Operation.Substract)
                        && stringBuilder.Length > 1
                        && (stringBuilder[stringBuilder.Length - 2].ToString() == OperationHelper.Convert(Operation.OpenBracket) 
                            /*|| !char.IsDigit(stringBuilder[stringBuilder.Length - 2])*/
                            ))
                    {
                        stringBuilder.Append($" 0 {OperationHelper.Convert(Operation.Substract)} ");
                        continue;
                    }

                    // 1*-1 -> exception
                    if (sym.ToString() == OperationHelper.Convert(Operation.Substract)
                        && stringBuilder.Length > 1
                        && OperationHelper.IsOperation(stringBuilder[stringBuilder.Length - 2].ToString())
                        && !OperationHelper.IsBracket(stringBuilder[stringBuilder.Length - 2].ToString()))
                    {
                        throw new Exception("Bracketless negotive number");
                    }

                    // )(  -> )*(
                    // 2( -> 2*(
                    if
                        (sym.ToString() == OperationHelper.Convert(Operation.OpenBracket) &&
                         (( stringBuilder.Length > 1 
                            && stringBuilder[stringBuilder.Length - 2].ToString() == OperationHelper.Convert(Operation.CloseBracket))
                          || (stringBuilder.Length > 0 
                              && double.TryParse(stringBuilder[stringBuilder.Length - 1].ToString(), NumberStyles.Any, new NumberFormatInfo(), out var n))))
                    {
                        stringBuilder.Append($" * {OperationHelper.Convert(Operation.OpenBracket)} ");
                        continue;
                    }

                    // -1 -> 0-1
                    // ,-1 -> ,0-1
                    if (((sym.ToString() == OperationHelper.Convert(Operation.Substract)
                          || sym.ToString() == OperationHelper.Convert(Operation.Sum))
                         && stringBuilder.Length == 0) 
                        || (stringBuilder.Length > 1 
                            && stringBuilder[stringBuilder.Length - 2].ToString() == Separator))
                    {
                        stringBuilder.Append($" 0 {sym} ");
                        continue;
                    }

                    stringBuilder.Append($" {sym} ");
                }
                else
                {
                    if (sym == char.Parse(Separator))
                    {
                        stringBuilder.Append($" {Separator} ");
                        continue;
                    }

                    stringBuilder.Append(sym);
                }
            }

            List<string> expressions = stringBuilder
                .ToString()
                .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(_ => _.ToString())
                .ToList();

            return expressions;
        }

        private static void CheckExpressionByIrrelevantSymbols(List<string> expressions)
        {
            for (int i = 1; i < expressions.Count - 1; i++)
            {
                if (expressions[i - 1] == OperationHelper.Convert(Operation.CloseBracket)
                    && expressions[i + 1] == OperationHelper.Convert(Operation.CloseBracket)
                    && expressions[i] == OperationHelper.Convert(Operation.Multiply))
                {
                    expressions[i] = Separator;
                }
            }

            var ops = expressions
                .Where(_ => OperationHelper.IsOperation(_.ToString()) && !OperationHelper.IsBracket(_.ToString()))
                .ToList();

            var nums = expressions
                .Where(_ => !OperationHelper.IsOperation(_.ToString()) && _ != Separator)
                .ToList();

            foreach (var num in nums)
            {
                if (!double.TryParse(num, NumberStyles.Any, new NumberFormatInfo(), out var n))
                {
                    throw new Exception("Has non-number and non-operation");
                }
            }

            if (ops.Count() != nums.Count() - 1)
            {
                throw new Exception("Invalid count of nums and operations");
            }
            
        }

        private static void CheckStringExpressionByBrackets(string expression)
        {
            Stack<Operation> stack = new Stack<Operation>();
            foreach (var exp in expression)
            {
                if (OperationHelper.Convert(Operation.OpenBracket) == exp.ToString())
                {
                    stack.Push(Operation.OpenBracket);
                    continue;
                }

                if (OperationHelper.Convert(Operation.CloseBracket) == exp.ToString())
                {
                    if (stack.Count() != 0)
                    {
                        stack.Pop();
                        continue;
                    }

                    throw new Exception("Invalid count of brackets");
                }
            }

            if (stack.Count() != 0)
            {
                throw new Exception("Invalid count of brackets");
            }
        }

        private static void CheckListExpressionByFirstItem(List<string> expressions)
        {
            if (expressions.Count == 0)
            {
                throw new Exception("Empty expression");
            }
            if (!(double.TryParse(expressions[0], NumberStyles.Any, new NumberFormatInfo(), out var num) 
                || OperationHelper.Convert(Operation.OpenBracket) == expressions[0]
                || OperationHelper.Convert(Operation.UserOperation) == expressions[0]))
            {
                throw new Exception("First num not number, not open bracket and not user operation");
            }
        }

        private static void CheckListExpressionsByOperationsAndNumsSequence(List<string> expressions)
        {
            for(int i = 1; i < expressions.Count; i++)
            {
                if (double.TryParse(expressions[i], NumberStyles.Any, new NumberFormatInfo(), out var n)
                      && (double.TryParse(expressions[i - 1], NumberStyles.Any, new NumberFormatInfo(), out var o)
                      || expressions[i - 1] == OperationHelper.Convert(Operation.CloseBracket)))
                {
                    throw new Exception("Not arithmetic expression");
                }
            }
        }

        private static void RemoveSeparators(ref List<string> expressions)
        {
            expressions.RemoveAll(_ => _ == Separator);
        }
    }
}
