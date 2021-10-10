using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMark
{
    public abstract class Block
    {
        public Valign? Valign { get; protected set; }
        public Halign? Halign { get; protected set; }
        public int? TextColor { get; protected set; }
        public int? BgColor { get; protected set; }
        public int? Height { get; protected set; }
        public int? Width { get; protected set; }
        public LinkedList<Block> Children { get; protected set; }
        public Block Parent { get; protected set; }

        public Block()
        {
            Valign = EMark.Valign.Top;
            Halign = EMark.Halign.Left;
            TextColor = 15;
            BgColor = 0;
            Height = 24;
            Width = 80;
            Children = new LinkedList<Block>();
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
            Children = new LinkedList<Block>();
            Valign = valign;
            Halign = halign;
            TextColor = textColor;
            BgColor = bgColor;
            Height = height;
            Width = width;
        }
    }
}
