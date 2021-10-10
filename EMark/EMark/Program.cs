using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMark
{
    class Program
    {
        static void Main(string[] args)
        {
             Block block = new Processor().TryParseToBlock(new Processor().ParseToList(@"C:\Users\Admin\OneDrive\Рабочий стол\7 семестр\TALC\EMark\emark.txt"));
             Console.WriteLine(block);
        }
    }
}
