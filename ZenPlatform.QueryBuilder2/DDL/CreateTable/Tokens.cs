using ZenPlatform.QueryBuilder2.Common;

namespace ZenPlatform.QueryBuilder2.DDL.CreateTable
{
    public static class Tokens
    {
        public static Token SpaceToken = new SpaceToken();
        public static Token CreateToken = new CreateToken();
        public static Token SelectToken = new SelectToken();
        public static Token TableToken = new TableToken();

        public static Token LeftBracketToken = new LeftBracketToken();
        public static Token RightBracketToken = new RightBracketToken();

        public static Token CommaToken = new CommaToken();
        public static Token TopToken = new TopToken();

        public static Token NewLineToken = new NewLineToken();
        public static Token TabToken = new TabToken();

        public static Token SchemaSeparator = new SchemaSeparatorToken();
    }

    public class TopToken : Token
    {
        public TopToken() : base("TOP")
        {
        }
    }

    public class NewLineToken : Token
    {
        public NewLineToken() : base("\n")
        {
        }
    }

    public class TabToken : Token
    {
        public TabToken() : base("    ")
        {
        }
    }

    public class SchemaSeparatorToken : Token
    {
        public SchemaSeparatorToken() : base(".")
        {
        }
    }
}