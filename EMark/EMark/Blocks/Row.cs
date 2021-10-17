using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EMark
{
    public class Row: Block
    {
        public Row(
            Block parent,
            XmlElement xml
            ) : base(parent, xml)
        {

        }
    }
}
