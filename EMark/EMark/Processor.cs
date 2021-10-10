using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMark.Blocks.FakeBlocks;

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

            List<string> parsedList = new List<string>();

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
                        TryPush(ref parsedList, ref element);
                        element.Add(currentSym);
                        continue;
                    }

                    element.Add(currentSym);

                    if (currentSym == '>')
                    {
                        TryPush(ref parsedList, ref element);
                    }
                }
            }

            return parsedList;
        }

        public Block TryParseToBlock(List<string> parsedList)
        {
            /*if(!(Regexps.BlockBegin.IsMatch(parsedList.First()) && Regexps.BlockEnd.IsMatch(parsedList.Last())))
            {
                throw new EMarkException($"{parsedList.First()} {parsedList.Last()}");
            }*/

            Stack<Block> stack = new Stack<Block>();

            List<string> copyList = new List<string>(parsedList);
            foreach (var element in parsedList)
            {
                copyList.RemoveAt(0);

                if (Regexps.BlockBegin.IsMatch(element))
                {
                    stack.Push(new FakeBlock(element));
                    continue;
                }

                if (Regexps.ColumnBegin.IsMatch(element))
                {
                    stack.Push(null);
                    continue; 
                }

                if (Regexps.RowBegin.IsMatch(element))
                {
                    stack.Push(null);
                    continue;
                }
                

                if (Regexps.BlockEnd.IsMatch(element))
                {
                    var block = new EBlock();
                    while (stack.Peek() != null)
                    {
                        block.Children.AddFirst(stack.Pop());
                    }
                    stack.Pop();

                    stack.Push(block);
                    continue;
                }

                if (Regexps.ColumnEnd.IsMatch(element))
                {
                    var block = new Column(null, 1);
                    while (stack.Peek() != null)
                    {
                        block.Children.AddFirst(stack.Pop());
                    }
                    stack.Pop();

                    stack.Push(block);
                    continue;
                }

                if (Regexps.RowEnd.IsMatch(element))
                {
                    var block = new Row(null, 1);
                    while (stack.Peek() != null)
                    {
                        block.Children.AddFirst(stack.Pop());
                    }
                    stack.Pop();

                    stack.Push(block);
                    continue;
                }

                stack.Push(new Content(element));
            }

            return stack.Count != 0 ? stack.Pop() : null;
        }
    }
}
