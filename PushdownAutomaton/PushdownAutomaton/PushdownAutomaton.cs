using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushdownAutomaton
{
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

        public void CheckStr(string str)
        {
            _history = new List<string>();
            _stack = new List<char>();
            _stack.Add(Eof);
            _stack.Add(_globalZ.First(_ => !_globalP.Contains(_)));

            var record = new StringBuilder($"{"s0"}, {str}, ");
            foreach(var sym in _stack)
            {
                record.Append(sym);
            }
            _history.Add(record.ToString());

            try
            {
                string newStr = new string(str.ToArray());
                List<char> newStack = new List<char>(_stack);
                List<string> newHistory = new List<string>(_history);
                _history = Step(ref newStr, ref newStack, ref newHistory);

                Console.WriteLine("-----SUCCES------");
                Console.WriteLine("history:");
                foreach (var rec in _history)
                {
                    Console.WriteLine(rec);
                }

            }

            catch (Exception e)
            {
                Console.WriteLine(e);

                Console.WriteLine("BAD STRING");
            }
        }

        private List<string> Step(ref string str, ref List<char> stack, ref List<string> history)
        {
            if(string.IsNullOrEmpty(str) && stack.Count == 1 && stack.Last() == Eof)
            {
                return history;
            }

            if (string.IsNullOrEmpty(str) || (stack.Count == 1 && stack.Last() == Eof))
            {
                throw new Exception();
            }

            if (IsNonTerminal(stack.Last()))
            {
                var tmpStack = new List<char>(stack);
                var transaction = _transactions.First(_ => _.Key.H == tmpStack.Last());
                foreach(var val in transaction.Value)
                {
                    stack.RemoveAt(stack.Count - 1);
                    string newStr = new string(str.ToArray());
                    List<char> newStack = new List<char>(stack);
                    foreach(var sym in val.H)
                    {
                        newStack.Add(sym);
                    }
                    List<string> newHistory = new List<string>(history);
                    try
                    {
                        Step(ref newStr, ref newStack, ref newHistory);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
            else
            {
                if (str.First() == stack.Last())
                {
                    str = str.Remove(0, 1);
                    stack.RemoveAt(stack.Count - 1);
                    string newStr = new string(str.ToArray());
                    List<char> newStack = new List<char>(stack);
                    List<string> newHistory = new List<string>(history);
                    try
                    {
                        Step(ref newStr, ref newStack, ref newHistory);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                else
                {
                    throw new Exception();
                }
            }

            return history;
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
