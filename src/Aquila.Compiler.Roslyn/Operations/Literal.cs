using System;
using System.Globalization;
using System.IO;

namespace Aquila.Compiler.Roslyn
{
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
        
        public Literal(long literal)
        {
            _literal = literal.ToString(CultureInfo.InvariantCulture);
            _type = LiteralType.Numeric;
        }

        public Literal(uint literal)
        {
            _literal = literal.ToString();
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
}