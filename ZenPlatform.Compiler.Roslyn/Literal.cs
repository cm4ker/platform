using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace ZenPlatform.Compiler.Roslyn
{
    public class ReplaceString
    {
        static readonly IDictionary<string, string> m_replaceDict
            = new Dictionary<string, string>();

        const string ms_regexEscapes = @"[\a\b\f\n\r\t\v\\""]";

        public static string StringLiteral(string i_string)
        {
            return Regex.Replace(i_string, ms_regexEscapes, match);
        }

        public static string CharLiteral(char c)
        {
            return c == '\'' ? @"'\''" : string.Format("'{0}'", c);
        }

        private static string match(Match m)
        {
            string match = m.ToString();
            if (m_replaceDict.ContainsKey(match))
            {
                return m_replaceDict[match];
            }

            throw new NotSupportedException();
        }

        static ReplaceString()
        {
            m_replaceDict.Add("\a", @"\a");
            m_replaceDict.Add("\b", @"\b");
            m_replaceDict.Add("\f", @"\f");
            m_replaceDict.Add("\n", @"\n");
            m_replaceDict.Add("\r", @"\r");
            m_replaceDict.Add("\t", @"\t");
            m_replaceDict.Add("\v", @"\v");

            m_replaceDict.Add("\\", @"\\");
            m_replaceDict.Add("\0", @"\0");

            //The SO parser gets fooled by the verbatim version 
            //of the string to replace - @"\"""
            //so use the 'regular' version
            m_replaceDict.Add("\"", "\\\"");
        }
    }

    public class Literal : Expression
    {
        private readonly string _literal;
        private readonly LiteralType _type;


        public Literal(int literal)
        {
            _literal = literal.ToString();
            _type = LiteralType.Numeric;
        }

        public Literal(double literal)
        {
            _literal = literal.ToString(CultureInfo.InvariantCulture);
            _type = LiteralType.Numeric;
        }

        public Literal(decimal literal)
        {
            _literal = literal.ToString(CultureInfo.InvariantCulture);
            _type = LiteralType.Numeric;
        }

        public Literal(string literal)
        {
            _literal = ReplaceString.StringLiteral(literal);
            _type = LiteralType.String;
        }


        public Literal(char literal)
        {
            _literal = ReplaceString.CharLiteral(literal);
            _type = LiteralType.String;
        }

        public override void Dump(TextWriter tw)
        {
            switch (_type)
            {
                case LiteralType.String:
                    tw.W("\"").W(_literal).W("\"");
                    break;
                case LiteralType.Char:
                    tw.W("'").W(_literal).W("'");
                    break;
                case LiteralType.Numeric:
                    tw.W(_literal);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public class IndexerAccess : Expression
    {
        private readonly Expression _array;
        private readonly Expression _indexExpression;

        public IndexerAccess(Expression indexExpression, Expression array)
        {
            _array = array;
            _indexExpression = indexExpression;
        }

        public override void Dump(TextWriter tw)
        {
            _array.Dump(tw);
            using (tw.SquareBrace())
                _indexExpression.Dump(tw);
        }
    }
}