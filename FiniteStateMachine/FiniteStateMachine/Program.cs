using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiniteStateMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            FiniteStateMachine machine = null;
            try
            {
                machine = new FiniteStateMachine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            while (true)
            {
                Console.WriteLine("Write line:");
                var text = Console.ReadLine();
                try
                {
                    Console.WriteLine($"{text}: {machine?.Run(text)}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{text}: {e.Message}");
                }
                Console.WriteLine();
            }
        }
    }
}
