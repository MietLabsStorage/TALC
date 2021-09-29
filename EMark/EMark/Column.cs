using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMark
{
    class Column: Block
    {
        public Column(
            Block parent,
            Valign? valign,
            Halign? halign,
            int? textColor,
            int? bgColor,
            int? width
        )
        {
            parent = parent ?? new Block();

            Valign = valign ?? parent.Valign;
            Halign = halign ?? parent.Halign;
            TextColor = textColor ?? parent.TextColor;
            BgColor = bgColor ?? parent.BgColor;
            Height = parent.Height;
            Width = width ?? parent.Width;
        }
    }
}
