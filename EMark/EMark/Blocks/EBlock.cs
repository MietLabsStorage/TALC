using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EMark
{
    class EBlock: Block
    {
        public EBlock() : base()
        {

        }

        public EBlock(
            Block parent,
            XmlElement xml
            ) : base(parent, xml)
        {

        }


    }
}
