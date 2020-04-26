using System.IO;

namespace ZenPlatform.Compiler.Roslyn
{
    public static class Ext
    {
        public static TextWriter Space(this TextWriter tw)
        {
            return tw.W(" ");
        }
        
        public static TextWriter Dot(this TextWriter tw)
        {
            return tw.W(".");
        }

        public static TextWriter Comma(this TextWriter tw)
        {
            return tw.W(";");
        }


        public static TextWriter W(this TextWriter tw, string s)
        {
            tw.Write(s);
            return tw;
        }

        public static Pair Parenthesis(this TextWriter tw)
        {
            return new Pair(tw, "(", ")");
        }

        public static Pair SquareBrace(this TextWriter tw)
        {
            return new Pair(tw, "[", "]");
        }

        public static Pair CurlyBrace(this TextWriter tw, bool newLine = true)
        {
            return new Pair(tw, "{", "}", newLine);
        }

        public static Pair AngleBrace(this TextWriter tw)
        {
            return new Pair(tw, "<", ">");
        }
    }
}