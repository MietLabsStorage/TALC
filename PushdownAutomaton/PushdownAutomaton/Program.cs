using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushdownAutomaton
{
    class Program
    {
        static void Main(string[] args)
        {
            var pushdownAutomaton = new PushdownAutomaton(@"grammatic.txt");
            Console.WriteLine("Transactions:");
            pushdownAutomaton.ShowTransactions();
            pushdownAutomaton.ShowGlobals();
            //pushdownAutomaton.CheckStr("a+a*a");
            while (true)
            {
                Console.WriteLine("Write str: ");
                pushdownAutomaton.CheckStr(Console.ReadLine());
            }
            Console.ReadKey();
        }
    }
}
