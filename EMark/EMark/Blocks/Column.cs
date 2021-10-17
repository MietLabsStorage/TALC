using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EMark
{
    class Column: Block
    {
        public Column(
            Block parent,
            XmlElement xml
            ) : base(parent, xml)
        {

        }
    }
}
