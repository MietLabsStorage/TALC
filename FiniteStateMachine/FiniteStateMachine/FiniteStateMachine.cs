using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiniteStateMachine
{
    public class FiniteStateMachine
    {
        private readonly HashSet<string> _startState = new HashSet<string>() {"q0"};
        private readonly string _endState = "f0";
        private FiniteStateDictionary _stateTable;

        public bool NeedDetermine { get; private set; } = false;

        public FiniteStateMachine()
        {
            _stateTable = new FiniteStateDictionary();
            var stateTable = File.ReadAllLines("StateTable.txt");
            Console.WriteLine($"Read {stateTable.Length} rows");

            foreach (var transition in stateTable)
            {
                var parsedTransition = ParsedTransition(transition);
                var key = new HashSet<string>() {parsedTransition.Key.Key};
                if (!_stateTable.ContainsKey(key))
                {
                    _stateTable.Add(key, new List<KeyValuePair<char, HashSet<string>>>());
                }

                _stateTable.ValueAdd(key, new KeyValuePair<char, HashSet<string>>(parsedTransition.Key.Value, new HashSet<string>(){ parsedTransition.Value }));
            }

            _stateTable.SynchronizedDict();
            foreach (var obj in _stateTable)
            {
                var transitions = (KeyValuePair<HashSet<string>, List<KeyValuePair<char, HashSet<string>>>>)obj;
                foreach (var transition in transitions.Value)
                {
                    if (transition.Value.Count != 1)
                    {
                        if (!_stateTable.ContainsKey(transition.Value))
                        {
                            Determine();
                            NeedDetermine = true;
                            break;
                        }
                    }
                }
            }

            if (NeedDetermine)
            {
                Console.WriteLine("Not determined");
                Console.WriteLine("New state table:");
                foreach (var obj in _stateTable)
                {
                    var transitions = (KeyValuePair<HashSet<string>, List<KeyValuePair<char, HashSet<string>>>>)obj;
                    foreach (var transition in transitions.Value)
                    {
                        Console.WriteLine($"{"{"}{transitions.Key.ToString<string>()}{"}"} {transition.Key} -> {"{"}{transition.Value.ToString<string>()}{"}"}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Determined");
            }
            Console.WriteLine("------------------------------------------------");
        }

        public bool Run(string text)
        {
            HashSet<string> currentState = _startState;
            foreach (var currentSym in text)
            {
                var key = currentState;
                if (key != null && _stateTable.ContainsKey(key))
                {
                    currentState = _stateTable.NewState(key, currentSym);
                    //_ = currentState ?? throw new Exception($"Find symbol ({currentSym}) or state is not contained in state table");
                }
                else
                {
                    return false;
                }
            }

            return currentState?.Contains(_endState) ?? false;
        }

        private void Determine()
        {
            var stateTable = new FiniteStateDictionary(_stateTable);
            foreach (var obj in _stateTable)
            {
                var transitions = (KeyValuePair<HashSet<string>, List<KeyValuePair<char, HashSet<string>>>>)obj;

                foreach (var transition in transitions.Value)
                {
                    if (transition.Value.Count != 1 && !_stateTable.ContainsKey(transition.Value))
                    {
                        var newKey = transition.Value;
                        stateTable.Add(newKey, new List<KeyValuePair<char, HashSet<string>>>());
                        foreach (var oldState in newKey)
                        {
                            var hsState = new HashSet<string>() { oldState };
                            if (_stateTable.ContainsKey(hsState))
                            {
                                var keyPair = _stateTable.Value(hsState);
                                foreach (var newState in keyPair)
                                {
                                    stateTable.ValueAdd(newKey, newState);
                                }
                            }
                        }
                    }
                }
            }

            _stateTable = new FiniteStateDictionary(stateTable);

            _stateTable.SynchronizedDict();
            foreach (var obj in _stateTable)
            {
                var transitions = (KeyValuePair<HashSet<string>, List<KeyValuePair<char, HashSet<string>>>>)obj;
                foreach (var transition in transitions.Value)
                {
                    if (transition.Value.Count != 1)
                    {
                        if (!_stateTable.ContainsKey(transition.Value))
                        {
                            Determine();
                            break;
                        }
                    }
                }
            }
        }

        private KeyValuePair<KeyValuePair<string, char>, string> ParsedTransition(string transition)
        {
            string oldState = transition.Substring(0, transition.IndexOf(','));
            char sym = transition.Substring(transition.IndexOf(',') + 1, 1)[0];
            string newState = transition.Substring(transition.LastIndexOf('=') + 1);

            while (newState.Contains(' '))
            {
                newState = newState
                    .Remove(newState.IndexOf(' '), 1);
            }
            while (newState.Contains('\t'))
            {
                newState = newState
                    .Remove(newState.IndexOf('\t'), 1);
            }

            return new KeyValuePair<KeyValuePair<string, char>, string>(new KeyValuePair<string, char>(oldState, sym),
                newState);
        }
    }
}
