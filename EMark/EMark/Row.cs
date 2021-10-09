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
            int? height,
            Valign? valign = null,
            Halign? halign = null,
            int? textColor = null,
            int? bgColor = null
                ): base(parent, valign: valign, halign: halign, textColor: textColor, bgColor: bgColor, height: height, width: null)
        {
        }
    }
}
