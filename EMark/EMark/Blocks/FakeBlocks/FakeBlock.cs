using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMark.Blocks.FakeBlocks
{
    public class FakeBlock: Block
    {
        public FakeBlock(string parameters)
        {
            Valign = ValignConverter.Convert(Regexps.Valign.Match(parameters).Value.Split('=').Last());
            Halign = HalignConverter.Convert(Regexps.Halign.Match(parameters).Value.Split('=').Last());

            Int32.TryParse(Regexps.TextColor.Match(parameters).Value, out int textColor);
            TextColor = textColor;

            Int32.TryParse(Regexps.BgColor.Match(parameters).Value, out int bgColor);
            BgColor = bgColor;

            Int32.TryParse(Regexps.Height.Match(parameters).Value, out int height);
            Height = height;

            Int32.TryParse(Regexps.Width.Match(parameters).Value, out int width);
            Width = width;
        }
    }
}
