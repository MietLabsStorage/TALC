using System;
using System.Collections.Generic;
using ReversePolishNotation.ConvertToRPN;
using ReversePolishNotation.SolveRPN;

namespace ReversePolishNotation
{
    class Program
    {


        static void Main(string[] args)
        {
            List<string> strList = new List<string>()
            {
                "(125.1 + 25.25 )      *( 3.3 + 10 )- 25 + log(4 log(4    ,   2)+2)",
                "1e2+2e3",
                "(1+1)/(1-1)+1",
                "4/0",
                "log(4 , 1)",
                "log(4 ,2)",
                "log(4, 0)",
                "log(4 ,-1)",
                "log(-4 ,-1)",
                "log(-4 , 2)",
                "0^1",
                "0^0",
                "1^1",
                "1^0",
                "2^-2",
                "2^(-2)",
                "2/-2",
                "2/(-1/2)",
                "2*-2",
                "2*(-2)",
                "2+-2",
                "2+(-2)",
                "2^0",
                ")2+2(",
                "(2+2))",
                "((2+2)",
                "((2+2))",
                "(-1)+1",
                "-1+1",
                "(1+1)(2+2)",
                "(1 + 2) 2 + ",
                "1 + 2 2 +",
                "(1 + 2) 2  ",
                "1 + 2 + ",
                "1 + 2 2 ",
                "1 + 2 2 +",
                "1 + 2 + ,",
                "1 + 2 + Q",
                "1 + 2 + =",
                "(1+2)-1",
                "+ 21 22",
                "+ + 2 1 3",
                " ",
                "+",
                "-21 2+1 +",
                "2(1+3)",
                "3",
                "log(8, log(log(16, 2),   log(  4,  2)  ))",
                "1e1",
                "1e-1",
                "1e - 1",
                "1e-0.1",
                "1e+1",
                "1e(-1)",
                "+1",
                "-1",
                ".5",
                "5.",
                "e2",
                "(23/38-6)*3.5-6",
                "-5+(-3)b",
                "1+f^0",
                "log(log(log(101,2),10),100)"
            };

            foreach (var str in strList)
            {                
                Console.WriteLine(str);
                List<string> newStr;
                try
                {
                    newStr = Rpn.Convert(str);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine();
                    Console.WriteLine("-----------------------------------------------------------------");
                    Console.WriteLine();
                    continue;
                }
                Console.WriteLine(Rpn.ToString(newStr));
                try
                {
                    Console.WriteLine(new SolveRpn(newStr).Answer);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine();
                    Console.WriteLine("-----------------------------------------------------------------");
                    continue;
                }                
                Console.WriteLine();
                Console.WriteLine("-----------------------------------------------------------------");
            }

            Console.WriteLine("END");
            Console.ReadLine();
        }
    }
}
