using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EMark
{
    public abstract class Block
    {
        public LinkedList<Block> Children { get; protected set; }
        public Block Parent { get; protected set; }
        public XmlElement Xml { get; protected set; }

        public bool IsLast { get; set; }

        public Valign? Valign { get; protected set; }
        public Halign? Halign { get; protected set; }
        public int? TextColor { get; protected set; }
        public int? BgColor { get; protected set; }
        public int? Height { get; protected set; }
        public int? Width { get; protected set; }


        public int LastWidth => Width - Children.Sum(_ => _.Width) ?? 0;
        public int LastHeight => Height - Children.Sum(_ => _.Height) ?? 0;

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
            XmlElement xml
            ) : base()
        {
            Children = new LinkedList<Block>();
            Parent = parent;
            Xml = xml;

            Valign = XmlElementHelper.Valign(xml) ?? parent?.Valign;
            Halign = XmlElementHelper.Halign(xml) ?? parent?.Halign;
            TextColor = XmlElementHelper.TextColor(xml) ?? parent?.TextColor;
            BgColor = XmlElementHelper.BgColor(xml) ?? parent?.BgColor;

            var height = XmlElementHelper.Height(xml);
            var width = XmlElementHelper.Width(xml);

            switch (this)
            {
                case EBlock block:
                    Height = (height < parent?.LastHeight ? height : parent?.LastHeight) ?? parent?.Height;
                    Width = (width < parent?.LastWidth ? width : parent?.LastWidth) ?? parent?.Width;
                    break;

                case Column block:
                    Height = parent?.Height;
                    Width = (width < parent?.LastWidth ? width : parent?.LastWidth) ?? parent?.Width;
                    break;

                case Row block:
                    Height = (height < parent?.LastHeight ? height : parent?.LastHeight) ?? parent?.Height;
                    Width = parent?.Width;
                    break;

                case Content block:
                    Height = parent?.Height;
                    Width = parent?.Width;
                    break;
            }

            if (xml != null)
            {
                foreach (var child in xml.ChildNodes)
                {
                    if(Children.Count != 0)
                    {
                        Children.Last().IsLast = false;
                    }

                    if (child is XmlElement element)
                    {
                        switch (element.Name)
                        {
                            case "block":
                                Children.AddLast(new EBlock(this, element));
                                break;
                            case "row":
                                Children.AddLast(new Row(this, element));
                                break;
                            case "column":
                                Children.AddLast(new Column(this, element));
                                break;
                        }
                    }

                    if (child is XmlText text)
                    {                        
                        Children.AddLast(new Content(this, text));
                    }

                    Children.Last().IsLast = true;
                }
            }
        }

        public virtual PixelText[][] GetText()
        {
            PixelText[][] text = new PixelText[Height ?? 0][];

            List<PixelText[][]> childTexts = new List<PixelText[][]>();
            foreach (var child in Children)
            {
                childTexts.Add(child.GetText());
            }

            for (int i = 0; i < text.Length; i++)
            {
                text[i] = new PixelText[Width ?? 0];
                var textRow = new List<PixelText>();
                foreach (var childRow in childTexts)
                {
                    textRow.AddRange(childRow[i]);
                }
                for (int j = 0; j < textRow.Count; j++)
                {
                    text[i][j] = textRow[j];
                }
            }
            return text;

        }

        public void Align()
        {
            foreach(var child in Children)
            {
                if (child.IsLast)
                {
                    child.Height = child.Height+this.LastHeight;
                    child.Width = child.Width+this.LastWidth;
                }
                child.Align();
            }
        }
    }

    public static class XmlElementHelper
    {
        public static Valign? Valign(XmlElement xmlElement) => ValignConverter.Convert(xmlElement?.GetAttribute("valign"));

        public static Halign? Halign(XmlElement xmlElement) => HalignConverter.Convert(xmlElement?.GetAttribute("halign"));

        public static int? TextColor(XmlElement xmlElement)
        {
            bool e = int.TryParse(xmlElement?.GetAttribute("textcolor"), out int result);
            return e ? (int?)result : null;
        }

        public static int? BgColor(XmlElement xmlElement)
        {
            bool e = int.TryParse(xmlElement?.GetAttribute("bgcolor"), out int result);
            return e ? (int?)result : null;
        }

        public static int? Height(XmlElement xmlElement)
        {
            bool e = int.TryParse(xmlElement?.GetAttribute("height"), out int result);
            return e ? (int?)result : null;
        }

        public static int? Width(XmlElement xmlElement)
        {
            bool e = int.TryParse(xmlElement?.GetAttribute("width"), out int result);
            return e ? (int?)result : null;
        }
    }
}
