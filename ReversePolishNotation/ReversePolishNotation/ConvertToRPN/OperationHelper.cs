using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversePolishNotation.ConvertToRPN
{
    public static class OperationHelper
    {
        public const string UserOperation = "log";
        private static double ExecuteUserOperaton(double numVar, double baseVar)
        {
            if (baseVar <= 0 || baseVar == 1)
            {
                throw new Exception("Invalid value in log base");
            }
            if (numVar <= 0)
            {
                throw new Exception("Invalid log value");
            }
            return Math.Log(numVar, baseVar);
        }

        public static readonly List<Operation> Brackets = new List<Operation>()
            {Operation.OpenBracket, Operation.CloseBracket};

        public static int GetPrioroty(string operation)
        {
            switch (operation)
            {
                case "(":
                    return 1;
                case ")":
                    return 1;
                case "+":
                    return 2;
                case "-":
                    return 2;
                case "*":
                    return 3;
                case "/":
                    return 3;
                case "^":
                    return 4;
                case UserOperation:
                    return 5;
                default:
                    return 0;
            }
        }

        public static string Convert(Operation operation)
        {
            switch (operation)
            {
                case Operation.OpenBracket:
                    return "(";
                case Operation.CloseBracket:
                    return ")";
                case Operation.Sum:
                    return "+";
                case Operation.Substract:
                    return "-";
                case Operation.Multiply:
                    return "*";
                case Operation.Divide:
                    return "/";
                case Operation.Pow:
                    return "^";
                case Operation.UserOperation:
                    return UserOperation;
                default:
                    return null;
            }
        }

        public static bool IsOperation(string expression) => GetPrioroty(expression) != 0;

        public static bool IsBracket(string expression) =>
            Brackets.Contains((Operation) OperationHelper.GetPrioroty(expression));

        public static double Execute(string operation, double firstNum, double secondNum)
        {
            switch (operation)
            {
                case "+":
                    return firstNum + secondNum;
                case "-":
                    return firstNum - secondNum;
                case "*":
                    return firstNum * secondNum;
                case "/":
                    if (secondNum == 0)
                    {
                        throw new Exception("Division by 0");
                    }
                    return firstNum / secondNum;
                case "^":
                    return Math.Pow(firstNum, secondNum);
                case UserOperation:
                    return ExecuteUserOperaton(firstNum, secondNum);
                default:
                    throw new Exception("Invalid operation");
            }
        }

        public static double Execute(Operation operation, double firstNum, double secondNum)
        {
            return Execute(Convert(operation), firstNum, secondNum);
        }
    }
}
