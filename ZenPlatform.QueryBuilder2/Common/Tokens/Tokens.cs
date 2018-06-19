using ZenPlatform.QueryBuilder2.Common;

namespace ZenPlatform.QueryBuilder2.DDL.CreateTable
{
    public static class Tokens
    {
        public static Token SpaceToken = new SpaceToken();
        public static Token CreateToken = new CreateToken();
        public static Token DropToken = new DropToken();
        public static Token AlterToken = new AlterToken();
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


    public class DropToken : Token
    {
        public DropToken() : base("DROP")
        {
        }
    }

    public class AlterToken : Token
    {
        public AlterToken() : base("ALTER")
        {
        }
    }
}