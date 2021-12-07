using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Interpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            var text = File.ReadAllText("code.txt");

            text = text.Replace("\n", " ");
            text = text.Replace("\r", " ");
            text = text.Replace("\t", " ");
            text += " ";
            while (text.Contains("  "))
            {
                text = text.Replace("  ", " ");
            }
            //var code = text.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList();
            var code = new List<string>();
            int i = 0;
            string str = "";
            while (true)
            {
                if(i >= text.Length)
                {
                    break;
                }

                if(str == "")
                {
                    str += text[i];
                    i++;
                    continue;
                }

                if(str.First() != '\"')
                {
                    while (text[i] != ' ')
                    {
                        str += text[i];
                        i++;
                    }
                    code.Add(str);
                    str = "";
                    i++;
                }
                else
                {
                    while (text[i] != '\"')
                    {
                        str += text[i];
                        i++;
                    }
                    str += text[i];
                    i++;
                    code.Add(str);
                    str = "";
                    i++;
                }
            }

            int id = 0;
            try
            {
                Interpreter.Program(code, ref id);
            }
            catch(Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
