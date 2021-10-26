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

        public Valign? Valign { get; protected set; }
        public Halign? Halign { get; protected set; }
        public int? TextColor { get; protected set; }
        public int? BgColor { get; protected set; }
        public int? Height { get; protected set; }
        public int? Width { get; protected set; }

        public int Rows { get; protected set; }
        public int Columns { get; protected set; }
        public int LastWidth => Width - Children.Sum(_ => _.Width) ?? 0;
        public int LastHeight => Height - Children.Sum(_ => _.Height) ?? 0;
        public int Childs => Rows != 0 ? Rows : Columns != 0 ? Columns : 1;

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
            ) : this()
        {
            Children = new LinkedList<Block>();
            Parent = parent;
            Xml = xml;

            Valign = XmlElementHelper.Valign(xml) ?? parent?.Valign ?? EMark.Valign.Top;
            Halign = XmlElementHelper.Halign(xml) ?? parent?.Halign ?? EMark.Halign.Left;
            TextColor = XmlElementHelper.TextColor(xml) ?? parent?.TextColor ?? 15;
            BgColor = XmlElementHelper.BgColor(xml) ?? parent?.BgColor ?? 0;
            Rows = XmlElementHelper.Rows(xml);
            Columns = XmlElementHelper.Columns(xml);

            var height = XmlElementHelper.Height(xml) ?? (parent == null ? Height : null);
            var width = XmlElementHelper.Width(xml) ?? (parent == null ? Width : null);

            switch (this)
            {
                case EBlock _:
                    Height = parent?.Height ?? height;
                    Width = parent?.Width ?? width;
                    break;

                case Column _:
                    Height = parent?.Height ?? height; 
                    Width = (parent.Children.Count == parent.Childs - 1) ? parent.LastWidth : width ?? throw new EMarkException("need width");
                    break;

                case Row _:
                    Height = (parent.Children.Count == parent.Childs - 1) ? parent.LastHeight : height ?? throw new EMarkException("need height");
                    Width = parent?.Width ?? width;
                    break;

                case Content _:
                    Height = parent?.Height ?? height;
                    Width = parent?.Width ?? width;
                    break;
            }

            if (xml != null)
            {
                if(xml.ChildNodes.Count != Childs)
                {
                    throw new EMarkException("Errors children count");
                }
                foreach (var child in xml.ChildNodes)
                {
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
                }
            }

            if (parent != null && !(Children.Count != 0 && Children?.Last() is Content) && !(this is Content))
            {
                if (Columns <= 0 && Rows <= 0 && !(Children?.Last() is EBlock)) 
                {
                    throw new EMarkException("Need rows or columns");
                }

                if (Columns > 0 && Rows > 0)
                {
                    throw new EMarkException("Need only rows or only columns");
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

            List<PixelText[]> childRows = new List<PixelText[]>();
            foreach (var childRow in childTexts)
            {
                foreach (var row in childRow)
                {
                    childRows.Add(row);
                }
            }

            for (int i = 0; i < text.Length; i++)
            {
                text[i] = new PixelText[Width ?? 0];
                var textRow = new List<PixelText>();
                if (Rows > 0)
                {
                    textRow.AddRange(childRows[i]);   
                }
                else
                {
                    foreach (var childRow in childTexts)
                    {
                        textRow.AddRange(childRow[i]);
                    }
                }                             

                for (int j = 0; j < textRow.Count; j++)
                {
                    text[i][j] = textRow[j];
                }
            }
            return text;
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

        public static int Rows(XmlElement xmlElement)
        {
            bool e = int.TryParse(xmlElement?.GetAttribute("rows"), out int result);
            return e ? result : 0;
        }

        public static int Columns(XmlElement xmlElement)
        {
            bool e = int.TryParse(xmlElement?.GetAttribute("columns"), out int result);
            return e ? result : 0;
        }
    }
}
