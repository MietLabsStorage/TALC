using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMark
{
    public class Row: Block
    {
        public Row(
            Block parent,
            Valign? valign,
            Halign? halign,
            int? textColor,
            int? bgColor,
            int? height
        )
        {
            parent = parent ?? new Block();

            Valign = valign ?? parent.Valign;
            Halign = halign ?? parent.Halign;
            TextColor = textColor ?? parent.TextColor;
            BgColor = bgColor ?? parent.BgColor;
            Height = height ?? parent.Height;
            Width = parent.Width;
        }
    }
}
