using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorParser
{
    public class TransactionArg
    {
        public string S { get; set; }
        public string P { get; set; }
        public string H { get; set; }

        public override string ToString()
        {
            return $"{S}, {P}, {H}";
        }
    }

    public class TransactionVal
    {
        public string S { get; set; }
        public List<string> H { get; set; }

        public override string ToString()
        {
            return $"{S}, {H}";
        }
    }

    public class Parser
    {
        private List<KeyValuePair<TransactionArg, List<TransactionVal>>> _transactions;
        private HashSet<string> _globalP;
        private HashSet<string> _globalZ;

        private Dictionary<string, HashSet<string>> _firsts;
        private Dictionary<string, HashSet<string>> _follows;
        private List<KeyValuePair<(string A, string a), KeyValuePair<TransactionArg, List<string>>>> _table;

        const char Eof = '$';


        public Parser(string filename)
        {
            _transactions = new List<KeyValuePair<TransactionArg, List<TransactionVal>>>();
            _globalP = new HashSet<string>();
            _globalZ = new HashSet<string>();
            _firsts = new Dictionary<string, HashSet<string>>();
            _follows = new Dictionary<string, HashSet<string>>();
            _table = new List<KeyValuePair<(string A, string a), KeyValuePair<TransactionArg, List<string>>>>();

            var lines = File.ReadAllLines(filename);

            for (int i = lines.Length - 1; i > 0; i--)
            {
                if (lines[i][0] != '<')
                {
                    if (lines[i - 1].Last() == ':')
                    {
                        lines[i - 1] += Eof;
                    }
                    lines[i - 1] += lines[i];
                }
            }

            lines = lines.Where(_ => _[0] == '<').ToArray();

            foreach (var line in lines)
            {
                try
                {
                    _transactions.Add(ParsedTransition(line));
                }
                catch
                {

                }
            }

            /*foreach (var sym in _globalP)
            {
                _transactions.Add(new KeyValuePair<TransactionArg, List<TransactionVal>>(new TransactionArg { S = "s0", P = sym.ToString(), H = sym }, new List<TransactionVal>() { new TransactionVal { S = "s0", H = new List<string> { Eof.ToString() } } }));
            }*/

             //firsts
            foreach (var A in _globalZ)
            {
                _firsts.Add(A, new HashSet<string>());
            }
            bool changed = true;
            while (changed)
            {
                changed = false;
                foreach (var transA in _transactions)
                {
                    foreach (var trans in transA.Value)
                    {

                        var firsts = First(trans.H);
                        var len = _firsts[transA.Key.H].Count();
                        for (int i = 0; i < firsts.Count; i++)
                        {
                            _firsts[transA.Key.H].Add(firsts[i]);
                        }
                        if (_firsts[transA.Key.H].Count() != len)
                        {
                            changed = true;
                        }

                    }

                }
            }

            //follow
            foreach (var A in _globalZ)
            {
                _follows.Add(A, new HashSet<string>());
            }
            _follows["<program>"].Add(Eof.ToString());
            changed = true;
            while (changed)
            {
                changed = false;
                foreach (var transA in _transactions)
                {
                    foreach (var trans in transA.Value)
                    {
                        for (int i = 0; i < trans.H.Count(); i++)
                        {
                            if (IsNONTerminal(trans.H[i]))
                            {
                                var gamma = new List<string>();
                                for(int j = i+1; j < trans.H.Count(); j++)
                                {
                                    gamma.Add(trans.H[j]);
                                }
                                /*if(gamma.Count() == 0)
                                {
                                    gamma.Add(Eof.ToString());
                                }*/
                                if (gamma.Count() != 0)
                                {
                                    var fst = First(gamma);
                                    var removed = fst.Remove(Eof.ToString());
                                    var flwlen = _follows[trans.H[i]].Count();
                                    for (int k = 0; k < fst.Count; k++)
                                    {
                                        _follows[trans.H[i]].Add(fst[k]);
                                    }
                                    if (removed)
                                    {
                                        var flw = _follows[transA.Key.H];
                                        foreach (var fw in flw)
                                        {
                                            _follows[trans.H[i]].Add(fw);
                                        }
                                    }
                                    if (_follows[trans.H[i]].Count() != flwlen)
                                    {
                                        changed = true;
                                    }
                                }
                                else
                                {
                                    var flwlen = _follows[trans.H[i]].Count();
                                    var flw = _follows[transA.Key.H];
                                    foreach (var fw in flw)
                                    {
                                        _follows[trans.H[i]].Add(fw);
                                    }
                                    if (_follows[trans.H[i]].Count() != flwlen)
                                    {
                                        changed = true;
                                    }
                                }
                            }
                        }                      

                    }

                }
            }

            //table
            foreach (var transA in _transactions)
            {
                foreach (var trans in transA.Value)
                {
                    var first = First(trans.H);
                    foreach(var fst in first)
                    {
                        if (IsTerminal(fst))
                        {
                            _table.Add(
    new KeyValuePair<(string A, string a), KeyValuePair<TransactionArg, List<string>>>(
        (transA.Key.H, fst),
        new KeyValuePair<TransactionArg, List<string>>(transA.Key, trans.H)
    ));
                        }
                    }
                    var follow = _follows[transA.Key.H];
                    if (first.Contains(Eof.ToString()) && trans.H.First() != Eof.ToString())
                    {
                        
                        foreach (var flw in follow)
                        {
                            if (IsTerminal(flw))
                            {
                                _table.Add(
    new KeyValuePair<(string A, string a), KeyValuePair<TransactionArg, List<string>>>(
        (transA.Key.H, flw),
        new KeyValuePair<TransactionArg, List<string>>(transA.Key, trans.H)
    ));
                            }
                        }                        
                    }
                    if (first.Contains(Eof.ToString()))
                    {
                        _table.Add(
                            new KeyValuePair<(string A, string a), KeyValuePair<TransactionArg, List<string>>>(
                                (transA.Key.H, Eof.ToString()),
                                new KeyValuePair<TransactionArg, List<string>>(transA.Key, trans.H)
                            ));
                    }

                }
            }

        }

        public List<string> First(List<string> alef)
        {
            if (IsTerminal(alef[0]))
            {
                return new List<string>() { alef[0] };
            }

            var lst = new List<string>();
            if (alef[0] == Eof.ToString())
            {
                lst.Add(Eof.ToString());
            }
            int ind = 0;
            var fst = _firsts.ContainsKey(alef[ind]) ? _firsts[alef[ind]] : new HashSet<string>() { Eof.ToString() };
            lst.AddRange(fst);
            while (fst.Contains(Eof.ToString()))
            {
                ind++;
                if (ind < alef.Count())
                {
                    fst = _firsts.ContainsKey(alef[ind]) ? _firsts[alef[ind]] : new HashSet<string>() { Eof.ToString() };
                    lst.AddRange(fst);
                }
                else
                {
                    break;
                }
            }
            return lst;
        }

        private KeyValuePair<TransactionArg, List<TransactionVal>> ParsedTransition(string transition)
        {
            if (transition.Length < 3)
            {
                throw new Exception();
            }

            var terms = transition.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            TransactionArg transactionArg = new TransactionArg { S = "s0", P = Eof.ToString(), H = terms[0] };
            _globalZ.Add(transactionArg.H);

            var elems = terms[1].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            List<TransactionVal> transactionVals = new List<TransactionVal>();
            foreach (var elem in elems)
            {
                int i = 0;
                string word = "";
                List<string> Hs = new List<string>();
                while (true)
                {
                    if (i >= elem.Length)
                    {
                        break;
                    }

                    if (elem[i] == Eof)
                    {
                        i++;
                        Hs.Add(Eof.ToString());
                        continue;
                    }

                    if(elem[i] == ' ')
                    {
                        Hs.Add(" ");
                        i++;
                    }

                    if(elem[i] == '<')
                    {
                        while(elem[i] != '>')
                        {
                            word += elem[i];
                            i++;
                        }
                        word += elem[i];
                        i++;

                        Hs.Add(word);
                        _globalP.Add(word);
                        word = "";
                        continue;
                    }

                    if (elem[i] == '\'')
                    {
                        word += elem[i];
                        i++;
                        while (elem[i] != '\'')
                        {
                            word += elem[i];
                            i++;
                        }
                        word += elem[i];
                        i++;

                        Hs.Add(word);
                        _globalP.Add(word);
                        word = "";
                        continue;
                    }
                }
                transactionVals.Add(new TransactionVal { S = "s0", H = Hs });

            }

            return new KeyValuePair<TransactionArg, List<TransactionVal>>(transactionArg, transactionVals);
        }

        public static bool IsTerminal(string val) => val[0] == '\'';

        public static bool IsNONTerminal(string val) => val[0] == '<';

        public static string WithoutBracks(string str)
        {
            if(str == Eof.ToString())
            {
                return Eof.ToString();
            }
            str = str.Remove(0, 1);
            return str.Remove(str.Length - 1, 1);
        }

        public void Run(string code)
        {
            Stack<string> stack = new Stack<string>();
            stack.Push(Eof.ToString());
            stack.Push(_globalZ.First());

            code = code.Replace("\n", " ");
            code = code.Replace("\r", " ");
            code = code.Replace("\t", " ");
            while(code.Contains("  "))
            {
                code = code.Replace("  ", " ");
            }

            List<string> codeSymbols = new List<string>();
            int j = 0;
            string str = "";
            while (true)
            {

                do
                {
                    str += code[j];
                    j++;
                } while (code[j] != ' ');
                str += code[j];
                j++;
                if (this._globalP.Contains($"\'{str}\'"))
                {
                    codeSymbols.Add(str);
                    str = "";
                }
                else
                {
                    foreach (var sym in str)
                    {
                        codeSymbols.Add(sym.ToString());
                    }
                    codeSymbols.RemoveAt(codeSymbols.Count - 1);
                    str = "";
                }

                if (j == code.Length)
                {
                    break;
                }
            }

            int i = 0;
            do
            {
                if (stack.Peek() == Eof.ToString() || IsTerminal(stack.Peek()))
                {
                    if (WithoutBracks(stack.Peek()) == codeSymbols[i])
                    {
                        stack.Pop();
                        i++;
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    var M = _table.FirstOrDefault(_ => _.Key.A == stack.Peek() && WithoutBracks(_.Key.a) == codeSymbols[i]);
                    if (M.Value.Value == null)
                    {
                        M = _table.FirstOrDefault(_ => _.Key.A == stack.Peek() && WithoutBracks(_.Key.a) == Eof.ToString());
                    }
                    
                    if (M.Value.Value != null)
                    {
                        stack.Pop();
                        for (int k = M.Value.Value.Count - 1; k >= 0; k--)
                        {
                            if(M.Value.Value[k] != Eof.ToString())
                            {
                                stack.Push(M.Value.Value[k]);
                            }
                        }

                    }
                    else
                    {
                        throw new Exception();
                    }
                }
            } while (stack.Peek() != Eof.ToString());
        }
    }
}
