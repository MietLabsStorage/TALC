using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EMark
{
    class Program
    {
        static void Main(string[] args)
        {
            new Processor(@"emark.txt").Render();
            Console.ReadLine();
        }
    }
}
