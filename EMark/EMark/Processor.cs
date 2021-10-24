using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EMark
{
    public class Processor
    {
        private XmlDocument _xDoc;
        private Block _block;

        public Processor(string filename)
        {
            _xDoc = new XmlDocument();
            _xDoc.Load(filename);
            _block = new EBlock(null, _xDoc.DocumentElement);
        }

        public void Render()
        {
            var text = _block.GetText();
            for (int i = 0; i < text.Length; i++)
            {
                for (int j = 0; j < text[i].Length; j++)
                {
                    Console.ForegroundColor = (ConsoleColor)text[i][j].TextColor;
                    Console.BackgroundColor = (ConsoleColor)text[i][j].BgColor;
                    Console.Write(text[i][j].Sym);
                }
                Console.WriteLine();
            }
        }        
    }
}
