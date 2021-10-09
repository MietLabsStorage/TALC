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
    }
}
