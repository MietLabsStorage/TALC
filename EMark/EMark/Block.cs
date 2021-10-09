using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMark
{
    public class Block
    {
        public Valign Valign { get; protected set; }
        public Halign Halign { get; protected set; }
        public int TextColor { get; protected set; }
        public int BgColor { get; protected set; }
        public int Height { get; protected set; }
        public int Width { get; protected set; }
        public List<Block> Children { get; protected set; }
        public Block Parent { get; protected set; }

        public Block()
        {
            Valign = Valign.Top;
            Halign = Halign.Left;
            TextColor = 15;
            BgColor = 0;
            Height = 24;
            Width = 80;
            Children = new List<Block>();
            Parent = null;
        }

        public Block(
            Block parent,
            Valign? valign = null,
            Halign? halign = null,
            int? textColor = null,
            int? bgColor = null,
            int? height = null,
            int? width = null
            )
        {
            Parent = parent;
            Valign = valign ?? parent.Valign;
            Halign = halign ?? parent.Halign;
            TextColor = textColor ?? parent.TextColor;
            BgColor = bgColor ?? parent.BgColor;
            Height = height ?? parent.Height;
            Width = width ?? parent.Width;
        }
    }
}
