using System.Data;
using System.Runtime.InteropServices.WindowsRuntime;
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

        public static Token ColumnToken = new ColumnToken();

        public static Token LeftBracketToken = new LeftBracketToken();
        public static Token RightBracketToken = new RightBracketToken();

        public static Token CommaToken = new CommaToken();
        public static Token TopToken = new TopToken();

        public static Token NewLineToken = new NewLineToken();
        public static Token TabToken = new TabToken();

        public static Token AddToken = new AddToken();

        public static Token NullToken = new NullToken();

        public static Token NotToken = new NotToken();

        public static Token SchemaSeparator = new SchemaSeparatorToken();

        public static Token UniqueToken = new UniqueToken();

        public static Token DatabaseToken = new DatabaseToken();
    }


    public class UniqueToken : Token
    {
        public UniqueToken() : base("UNIQUE")
        {
        }
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

    public class ColumnToken : Token
    {
        public ColumnToken() : base("COLUMN")
        {
        }
    }

    public class AddToken : Token
    {
        public AddToken() : base("ADD")
        {
        }
    }

    public class NullToken : Token
    {
        public NullToken() : base("NULL")
        {
        }
    }

    public class NotToken : Token
    {
        public NotToken() : base("NOT")
        {
        }
    }

    public class DatabaseToken : Token
    {
        public DatabaseToken() : base("DATABASE")
        {
        }
    }
}