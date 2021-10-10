using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EMark
{
    public static class Regexps
    {
        public static Regex BlockBegin => new Regex("^<block.*>$");
        public static Regex BlockEnd => new Regex("^</block>$");
        public static Regex ColumnBegin => new Regex("^<column.*>$");
        public static Regex ColumnEnd => new Regex("^</column>$");
        public static Regex RowBegin => new Regex("^<row.*>$");
        public static Regex RowEnd => new Regex("^</row>$");


        public static Regex Valign => new Regex("valign=(top|center|bottom)");
        public static Regex Halign => new Regex("^halign=(left|center|right)");
        public static Regex TextColor => new Regex("^textcolor=[0-9]{1,2}$");
        public static Regex BgColor => new Regex("^bgcolor=[0-9]{1,2}$");
        public static Regex Height => new Regex("^height=[0-9]{1,2}$");
        public static Regex Width => new Regex("^width=[0-9]{1,2}$");
    }
}
