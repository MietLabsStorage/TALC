using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMark
{
    public enum Valign
    {
        Top,
        Center,
        Bottom
    }

    public static class ValignConverter
    {
        public static Valign? Convert(string valign)
        {
            switch (valign)
            {
                case "top":
                    return Valign.Top;
                case "center":
                    return Valign.Center;
                case "bottom":
                    return Valign.Bottom;
                default:
                    return null;
            }
        }
    }
}
