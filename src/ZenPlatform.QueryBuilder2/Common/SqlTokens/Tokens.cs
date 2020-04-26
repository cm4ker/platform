namespace ZenPlatform.QueryBuilder.Common.SqlTokens
{
    public static class Tokens
    {
        public static class Comparators
        {
            /// <summary>
            /// Больше
            /// </summary>
            public static ComparerToken Gt = new GreatThenToken();

            /// <summary>
            /// Меньше
            /// </summary>
            public static ComparerToken Lt = new LessThenToken();

            /// <summary>
            /// Больше или равно
            /// </summary>
            public static ComparerToken GoE = new GreatOrEqualsThenToken();

            /// <summary>
            /// Меньше или равно
            /// </summary>
            public static ComparerToken LoE = new LessOrEqualsThenToken();

            /// <summary>
            /// Равно
            /// </summary>
            public static ComparerToken E = new EqualsToken();

            /// <summary>
            /// Не равно
            /// </summary>
            public static ComparerToken Ne = new NotEqualsToken();

        }

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


        public static Token AndToken = new AndToken();
        public static Token IsToken = new IsToken();
        public static Token LikeToken = new LikeToken();
    }

    public class LikeToken : Token
    {
        public LikeToken() : base("LIKE")
        {
        }
    }

    public class IsToken : Token
    {
        public IsToken() : base("IS")
        {
        }
    }
    public class AndToken : Token
    {
        public AndToken() : base("AND")
        {
        }
    }

    public abstract class ComparerToken : Token
    {
        protected ComparerToken(string name) : base(name)
        {
        }
    }

    public class GreatThenToken : ComparerToken
    {
        internal GreatThenToken() : base(">")
        {
        }
    }

    public class LessThenToken : ComparerToken
    {
        internal LessThenToken() : base("<")
        {
        }
    }

    public class GreatOrEqualsThenToken : ComparerToken
    {
        internal GreatOrEqualsThenToken() : base("<=")
        {
        }
    }

    public class LessOrEqualsThenToken : ComparerToken
    {
        internal LessOrEqualsThenToken() : base(">=")
        {
        }
    }

    public class NotEqualsToken : ComparerToken
    {
        internal NotEqualsToken() : base("<>")
        {
        }
    }

    public class EqualsToken : ComparerToken
    {
        internal EqualsToken() : base("=")
        {
        }
    }

    public class OnToken : Token
    {
        internal OnToken() : base("ON")
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