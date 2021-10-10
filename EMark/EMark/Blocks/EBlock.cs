using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMark
{
    class EBlock: Block
    {
        public EBlock() : base()
        {

        }

        public EBlock(
            Block parent,
            Valign? valign = null,
            Halign? halign = null,
            int? textColor = null,
            int? bgColor = null,
            int? height = null,
            int? width = null
        ) : base(parent, valign, halign, textColor, bgColor, height, width)
        {

        }


    }
}
