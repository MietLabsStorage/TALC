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

        const char Eof = '$';


        public Parser(string filename)
        {
            _transactions = new List<KeyValuePair<TransactionArg, List<TransactionVal>>>();
            _globalP = new HashSet<string>();
            _globalZ = new HashSet<string>();
            _firsts = new Dictionary<string, HashSet<string>>();
            _follows = new Dictionary<string, HashSet<string>>();

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

            foreach (var sym in _globalP)
            {
                _transactions.Add(new KeyValuePair<TransactionArg, List<TransactionVal>>(new TransactionArg { S = "s0", P = sym.ToString(), H = sym }, new List<TransactionVal>() { new TransactionVal { S = "s0", H = new List<string> { Eof.ToString() } } }));
            }

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
                                if(gamma.Count() == 0)
                                {
                                    gamma.Add(Eof.ToString());
                                }
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
                                if(_follows[trans.H[i]].Count() != flwlen)
                                {
                                    changed = true;
                                }
                            }
                        }                      

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
                var termElems = elem.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var spaces = termElems.Count() - 1;
                for(int i = 0; i < spaces; i++)
                {
                    termElems.Insert(i * 2 + 1, " ");
                }


                if (termElems.Count() == 1 && termElems[0].Contains("><"))
                {
                    termElems = termElems[0].Split(new string[] { "><" }, StringSplitOptions.None).ToList();

                    termElems[0] += ">";
                    termElems[1] = "<" + termElems[1];
                }

                transactionVals.Add(new TransactionVal { S = "s0", H = termElems });                

                /*var syms = transactionVals.Last().H.Split();
                foreach (var sym in syms)
                {
                    foreach (var s in sym)
                    {
                        _globalZ.Add(s);
                        if (!"ABCDEFGHIJKLMNOPQRSTUVWXYZ".Contains(s))
                        {
                            _globalP.Add(s);
                        }
                    }
                }*/
            }

            return new KeyValuePair<TransactionArg, List<TransactionVal>>(transactionArg, transactionVals);
        }

        public static bool IsTerminal(string val) => val[0] == '\'';

        public static bool IsNONTerminal(string val) => val[0] == '<';
    }
}
