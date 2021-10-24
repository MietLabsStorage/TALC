using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EMark
{
    public class Content: Block
    {
        public string Text { get; private set; }

        public Content(
            Block parent,
            XmlText xml
            ): base(parent, null)
        {
            Text = xml.Value;
        }

        public override PixelText[][] GetText()
        {
            var text = new PixelText[Height ?? 0][];
            int k = 0;
            for(int i = 0; i < text.Length; i++)
            {
                text[i] = new PixelText[Width ?? 0];
                for (int j = 0; j < text[i].Length; j++)
                {
                    if(Text.Length > k)
                    {
                        text[i][j] = new PixelText { Sym = Text[k], BgColor = this.BgColor ?? 0, TextColor = this.TextColor ?? 0 };
                    }
                    else
                    {
                        text[i][j] = new PixelText { Sym = ' ', BgColor = this.BgColor ?? 0, TextColor = this.TextColor ?? 0 };
                    }
                    k++;
                }
            }
            return text;
        }
    }
}
