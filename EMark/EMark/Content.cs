using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMark
{
    class Content: Block
    {
        public string Text { get; private set; }

        public Content(
            string text
        ) : base(null)
        {
            Text = text;
        }
    }
}
