using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushdownAutomaton
{
    public class Process
    {
        public string Str { get; set; }
        public List<char> Stack { get; set; }
        public List<string> History { get; set; }
        public int Num { get; set; }
    }

    public class TransactionArg
    {
        public string S { get; set; }
        public string P { get; set; }
        public char H { get; set; }

        public override string ToString()
        {
            return $"{S}, {P}, {H}";
        }
    }

    public class TransactionVal
    {
        public string S { get; set; }
        public string H { get; set; }

        public override string ToString()
        {
            return $"{S}, {H}";
        }
    }

    public class PushdownAutomaton
    {
        private HashSet<char> _globalP;
        private HashSet<char> _globalZ;
        private HashSet<string> _globalS;
        private HashSet<string> _globalF;
        private List<KeyValuePair<TransactionArg, List<TransactionVal>>> _transactions;

        private List<string> _history;
        private List<char> _stack;

        const char Eof = ' ';

        public PushdownAutomaton(string filename)
        {
            _transactions = new List<KeyValuePair<TransactionArg, List<TransactionVal>>>();
            _globalP = new HashSet<char>();
            _globalZ = new HashSet<char>();
            _globalS = new HashSet<string>() { "s0" };
            _globalF = new HashSet<string>() { "s0" };

            var lines = File.ReadAllLines(filename);
            foreach(var line in lines)
            {
                try
                {
                    _transactions.Add(ParsedTransition(line));
                }
                catch
                {

                }
            }

            foreach(var sym in _globalP)
            {
                _transactions.Add(new KeyValuePair<TransactionArg, List<TransactionVal>>(new TransactionArg { S = "s0", P = sym.ToString(), H = sym }, new List<TransactionVal>() { new TransactionVal { S = "s0", H = Eof.ToString() } }));
            }

            _transactions.Add(new KeyValuePair<TransactionArg, List<TransactionVal>>(new TransactionArg { S = "s0", P = Eof.ToString(), H = Eof }, new List<TransactionVal>() { new TransactionVal { S = "s0", H = Eof.ToString() } }));
        }

        public void ShowGlobals()
        {
            Console.Write($"P: ");
            foreach(var elem in _globalP)
            {
                Console.Write($"{elem} ");
            }
            Console.WriteLine();

            Console.Write($"Z: ");
            foreach (var elem in _globalZ)
            {
                Console.Write($"{elem} ");
            }
            Console.WriteLine();

            Console.Write($"S: ");
            foreach (var elem in _globalS)
            {
                Console.Write($"{elem} ");
            }
            Console.WriteLine();

            Console.Write($"F: ");
            foreach (var elem in _globalF)
            {
                Console.Write($"{elem} ");
            }

            Console.WriteLine();
        }

        public void CheckStr(string str)
        {
            _history = new List<string>();
            _stack = new List<char>();
            _stack.Add(Eof);
            _stack.Add(_globalZ.First(_ => !_globalP.Contains(_)));

            try
            {
                string newStr = new string(str.ToArray());
                List<char> newStack = new List<char>(_stack);
                List<string> newHistory = new List<string>(_history);

                var processes = new List<Process>(){ new Process { Str = newStr, History = newHistory, Stack = newStack, Num = 1 }};

                int count = 0;
                while (true)
                {
                    _history = Step(ref processes);
                    if(_history != null)
                    {
                        break;
                    }
                    
                    Console.WriteLine($"Processed {processes.Count} branches...  {System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024} MB memory, {count}  cicles");                    

                    if(processes.Count == 0)
                    {
                        Console.WriteLine("BAD STRING");
                        return;
                    }

                    if(count > 100000 || System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024 > 500)
                    {
                        Console.WriteLine("PROBABLY SMTH BAD");
                        return;
                    }

                    count++;
                }

                Console.WriteLine("-----SUCCES------");
                Console.WriteLine("history:");
                foreach (var rec in _history)
                {
                    Console.WriteLine(rec);
                }

            }

            catch (Exception e)
            {
                Console.WriteLine("BAD STRING");
            }
        }

        private List<string> Step(ref List<Process> processes)
        {
            var newProcesses = new List<Process>(processes);

            foreach (var process in processes)
            {
                var record = new StringBuilder($"{"s0"}, {process.Str}, ");
                foreach (var sym in process.Stack)
                {
                    record.Append(sym);
                }
                process.History.Add(record.ToString());

                if (string.IsNullOrEmpty(process.Str) && process.Stack.Count == 1 && process.Stack.Last() == Eof)
                {
                    return process.History;
                }

                if (string.IsNullOrEmpty(process.Str) || (process.Stack.Count == 1 && process.Stack.Last() == Eof))
                {
                    newProcesses.Remove(processes.First(_ => _.Num == process.Num));
                }

                if (IsNonTerminal(process.Stack.Last()))
                {
                    var tmpStack = new List<char>(process.Stack);
                    var transaction = _transactions.First(_ => _.Key.H == tmpStack.Last());
                    newProcesses.Remove(process);
                    foreach (var val in transaction.Value)
                    {                       
                        string newStr = new string(process.Str.ToArray());
                        List<char> newStack = new List<char>(process.Stack);
                        newStack.RemoveAt(process.Stack.Count - 1);
                        foreach (var sym in val.H)
                        {
                            newStack.Add(sym);
                        }
                        List<string> newHistory = new List<string>(process.History);

                        newProcesses.Add(new Process { Str = newStr, History = newHistory, Stack = newStack, Num = processes.Last().Num++ });
                    }
                }
                else
                {
                    if (process.Str.First() == process.Stack.Last())
                    {
                        process.Str = process.Str.Remove(0, 1);
                        process.Stack.RemoveAt(process.Stack.Count - 1);
                        string newStr = new string(process.Str.ToArray());
                        List<char> newStack = new List<char>(process.Stack);
                        List<string> newHistory = new List<string>(process.History);
                    }
                    else
                    {
                        newProcesses.Remove(processes.First(_ => _.Num == process.Num));
                    }
                }
            }

            processes = new List<Process>(newProcesses);
            return null;
        }

        public void ShowTransactions()
        {
            foreach(var transaction in _transactions)
            {
                Console.Write($"{transaction.Key} ->");
                foreach(var val in transaction.Value)
                {
                    Console.Write($"({val}); ");
                }
                Console.WriteLine();
            }
        }

        private KeyValuePair<TransactionArg, List<TransactionVal>> ParsedTransition(string transition)
        {
            if(transition.Length < 3)
            {
                throw new Exception();
            }

            TransactionArg transactionArg = new TransactionArg { S = "s0", P = Eof.ToString(), H = transition[0] };
            _globalZ.Add(transactionArg.H);

            transition = transition.Remove(0, 2);
            while (transition.Contains(' '))
            {
                transition = transition
                    .Remove(transition.IndexOf(' '), 1);
            }
            while (transition.Contains('\t'))
            {
                transition = transition
                    .Remove(transition.IndexOf('\t'), 1);
            }


            var elems = transition.Split( new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            List<TransactionVal> transactionVals = new List<TransactionVal>();
            foreach(var elem in elems)
            {
                transactionVals.Add(new TransactionVal { S = "s0", H = new string(elem.Reverse().ToArray()) });

                var syms = transactionVals.Last().H.Split();
                foreach(var sym in syms)
                {
                    foreach(var s in sym)
                    {
                        _globalZ.Add(s);
                        if (!"ABCDEFGHIJKLMNOPQRSTUVWXYZ".Contains(s))
                        {
                            _globalP.Add(s);
                        }
                    }
                }
            }

            return new KeyValuePair<TransactionArg, List<TransactionVal>>(transactionArg, transactionVals);
        }

        public bool IsTerminal(char s) => !"ABCDEFGHIJKLMNOPQRSTUVWXYZ".Contains(s);

        public bool IsNonTerminal(char s) => "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Contains(s);
    }
}
