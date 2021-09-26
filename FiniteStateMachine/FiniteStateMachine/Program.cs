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
            /*var machine = new FiniteStateMachine();
            List<string> texts = new List<string>()
            {
                "a0", "a0000", "a", "b000", "b0", "ab", "ba", "0"
            };

            foreach (var text in texts)
            {
                try
                {
                    Console.WriteLine($"{text}: {machine.Run(text)}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{text}: {e.Message}");
                }
            }*/
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
                var text = Console.ReadLine();
                try
                {
                    Console.WriteLine($"{text}: {machine?.Run(text)}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{text}: {e.Message}");
                }
            }
        }
    }
}
