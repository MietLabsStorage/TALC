using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMark
{
    public class Processor
    {
        public readonly List<char> spaceSyms = new List<char>() {'\n', '\t', '\r', ' '};

        public List<string> ParseToList(string filename)
        {
            void TryPush(ref List<string> refStack, ref List<char> refEelement)
            {
                if (!refEelement.All(_ => spaceSyms.Contains(_)))
                {
                    refStack.Add(new string(refEelement.ToArray()));
                }
                refEelement.Clear();
            }

            List<string> deque = new List<string>();

            string text = File.ReadAllText(filename);
            List<char> element = new List<char>();

            foreach (var sym in text)
            {
                var currentSym = sym;
                if (spaceSyms.Contains(sym))
                {
                    currentSym = ' ';
                }

                if (element.Count == 0)
                {
                    element.Add(currentSym);
                }
                else
                {
                    if (currentSym == '<')
                    {
                        TryPush(ref deque, ref element);
                        element.Add(currentSym);
                        continue;
                    }

                    element.Add(currentSym);

                    if (currentSym == '>')
                    {
                        TryPush(ref deque, ref element);
                    }
                }
            }

            return deque;
        }

        public Block TryParseToBlock(List<string> deque)
        {
            if(!(Regexps.BlockBegin.IsMatch(deque.First()) && Regexps.BlockEnd.IsMatch(deque.Last())))
            {
                throw new EMarkException($"{deque.First()} {deque.Last()}");
            }
            Block textBlock = new Block(null);
            return new Block();
        }

    }
}
