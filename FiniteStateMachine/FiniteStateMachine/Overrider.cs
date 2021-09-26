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
            StringBuilder str = new StringBuilder(" ");
            foreach (var s in hs)
            {
                str.Append($"{s} ");
            }

            return str.ToString();
        }
    }
}
