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
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(@"C:\Users\Admin\OneDrive\Рабочий стол\7 семестр\TALC\EMark\emark.txt");
            Block block = new EBlock(new EBlock(), xDoc.DocumentElement);
            block.Align();
            var text = block.GetText();
            Console.WriteLine(block);
        }
    }
}
