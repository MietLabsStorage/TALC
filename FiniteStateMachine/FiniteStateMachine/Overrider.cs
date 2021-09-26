using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiniteStateMachine
{
    public static class Overrider
    {
        public static string ToString<T>(this HashSet<T> hs)
        {
            string str = "";
            foreach (var s in hs)
            {
                str += s;
            }

            return str;
        }

        public static bool Equals(this HashSet<string> hs, object obj)
        {
            return !hs.Except(obj as HashSet<string>).Any();
        }
    }
}
