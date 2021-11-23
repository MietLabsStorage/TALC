using System;
using System.IO;

namespace ErrorParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new Parser("grammatic.txt");
            parser.Run(File.ReadAllText("code.txt"));
        }
    }
}
