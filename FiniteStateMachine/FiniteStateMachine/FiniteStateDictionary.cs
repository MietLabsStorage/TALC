using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiniteStateMachine
{
    public class FiniteStateDictionary: IEnumerable
    {
        private Dictionary<HashSet<string>, List<KeyValuePair<char, HashSet<string>>>> _dictionary;

        public FiniteStateDictionary()
        {
            _dictionary = new Dictionary<HashSet<string>, List<KeyValuePair<char, HashSet<string>>>>();
        }

        public FiniteStateDictionary(FiniteStateDictionary dictionary)
        {
            _dictionary = new Dictionary<HashSet<string>, List<KeyValuePair<char, HashSet<string>>>>(dictionary._dictionary);
        }

        public void Add(HashSet<string> key, List<KeyValuePair<char, HashSet<string>>> value)
        {
            _dictionary.Add(key, value);
        }

        public bool ContainsKey(HashSet<string> key)
        {
            return _dictionary.Keys.Any(_ => IsKeysEquals(_, key));
        }

        public IEnumerator GetEnumerator()
        {
            foreach (var val in _dictionary)
            {
                yield return val;
            }
        }

        public List<KeyValuePair<char, HashSet<string>>> Value(HashSet<string> key) =>
            _dictionary.FirstOrDefault(_ => IsKeysEquals(_.Key, key)).Value;

        public void ValueAdd(HashSet<string> key, KeyValuePair<char, HashSet<string>> value) =>
            _dictionary.FirstOrDefault(_ => IsKeysEquals(_.Key, key)).Value?.Add(value);

        public void ValueClear(HashSet<string> key)
        {
            _dictionary.FirstOrDefault(_ => IsKeysEquals(_.Key, key)).Value?.Clear();
        }

        public HashSet<string> NewState(HashSet<string> key, char sym) =>
            _dictionary.FirstOrDefault(_ => IsKeysEquals(_.Key, key)).Value.FirstOrDefault(_ => _.Key == sym).Value;

        public static bool IsKeysEquals(HashSet<string> hs1, HashSet<string> hs2)
        {
            return hs1.Count() >= hs2.Count() ? !hs1.Except(hs2).Any() : !hs2.Except(hs1).Any();
        }

        public void SynchronizedDict()
        {
            Dictionary<HashSet<string>, List<KeyValuePair<char, HashSet<string>>>> newDict =
                new Dictionary<HashSet<string>, List<KeyValuePair<char, HashSet<string>>>>();
            foreach (var transition in _dictionary)
            {
                var newValues = new Dictionary<char, HashSet<string>>();
                foreach (var keyPair in transition.Value)
                {
                    if (!newValues.ContainsKey(keyPair.Key))
                    {
                        newValues.Add(keyPair.Key, new HashSet<string>());
                    }

                    foreach (var val in keyPair.Value)
                    {
                        newValues[keyPair.Key].Add(val);
                    }
                }
                newDict.Add(transition.Key, newValues.ToList());
            }

            _dictionary = new Dictionary<HashSet<string>, List<KeyValuePair<char, HashSet<string>>>>(newDict);
        }
    }
}
