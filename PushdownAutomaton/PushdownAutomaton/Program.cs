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
            var pushdownAutomaton = new PushdownAutomaton(@"C:\Users\Admin\OneDrive\Рабочий стол\7 семестр\ТАЯК\Laba3\test3.txt");
            pushdownAutomaton.ShowTransactions();
            pushdownAutomaton.CheckStr("a+a*a");
            Console.ReadKey();
        }
    }
}
