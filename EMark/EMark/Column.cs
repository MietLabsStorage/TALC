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
            int width,
            Valign? valign = null,
            Halign? halign = null,
            int? textColor = null,
            int? bgColor = null
        ): base(parent, valign: valign, halign: halign, textColor: textColor, bgColor: bgColor, height: null, width: width)
        {

        }
    }
}
