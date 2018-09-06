namespace ZenPlatform.QueryBuilder.Common.Tokens
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

        public static Token SumToken = new SumToken();

        public static Token LimitToken = new LimitToken();

        public static Token InsertToken = new InsertToken();

        public static Token IntoToken = new IntoToken();

        public static Token FromToken = new FromToken();

        public static Token AscToken = new AscToken();

        public static Token DescToken = new DescToken();

        public static Token IndexToken = new IndexToken();

        public static Token OnToken = new OnToken();
    }

    public class OnToken : Token
    {
        public OnToken() : base("ON")
        {
        }
    }

    public class IndexToken : Token
    {
        public IndexToken() : base("INDEX")
        {
        }
    }

    public class AscToken : Token
    {
        public AscToken() : base("ASC")
        {
        }
    }

    public class DescToken : Token
    {
        public DescToken() : base("DESC")
        {
        }
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


    public class SumToken : Token
    {
        public SumToken() : base("SUM")
        {
        }
    }

    public class InsertToken : Token
    {
        public InsertToken() : base("INSERT")
        {
        }
    }

    public class IntoToken : Token
    {
        public IntoToken() : base("INTO")
        {
        }
    }

    public class LimitToken : Token
    {
        public LimitToken() : base("LIMIT")
        {
        }
    }

    public class FromToken : Token
    {
        public FromToken() : base("FROM")
        {
        }
    }
}