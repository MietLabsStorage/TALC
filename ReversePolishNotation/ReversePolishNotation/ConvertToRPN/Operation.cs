using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversePolishNotation.ConvertToRPN
{
    public enum Operation
    {
        OpenBracket,
        CloseBracket,
        Sum,
        Substract,
        Multiply,
        Divide,
        Pow,
        UserOperation
    }
}
