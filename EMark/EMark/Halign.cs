using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMark
{
    public enum Halign
    {
        Left,
        Center,
        Right
    }

    public static class HalignConverter
    {
        public static Halign? Convert(string halign)
        {
            switch (halign)
            {
                case "left":
                    return Halign.Left;
                case "center":
                    return Halign.Center;
                case "right":
                    return Halign.Right;
                default:
                    return null;
            }
        }
    }
}
