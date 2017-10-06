namespace QueryCompiler
{
    public static class SQLTokens
    {
        public static string INSERT = "INSERT";
        public static string VALUES = "VALUES";
        public static string INTO = "INTO";


        public static string DELETE = "DELETE";

        public static string UPDATE = "UPDATE";
        public static string SET = "SET";

        public static string SELECT = "SELECT";
        public static string FROM = "FROM";
        public static string WHERE = "WHERE";
        public static string GROUP_BY = "GROUP BY";
        public static string HAVING = "HAVING";
        public static string ORDER_BY = "ORDER BY";

        public static string AS = "AS";

        public static string JOIN = "JOIN";
        public static string INNER = "INNER";
        public static string LEFT = "LEFT";
        public static string RIGHT = "RIGHT";
        public static string FULL = "FULL";
        public static string CROSS = "CROSS";

        public static string ON = "ON";
        public static string AND = "AND";
        public static string OR = "OR";
        public static string IS = "IS";
        public static string NULL = "NULL";


        public static string OUTER = "OUTER";
        public static string APPLY = "APPLY";

        public static string SUM = "SUM";

        public static string IDENTITY_INSERT = "IDENTITY_INSERT";
        public static string OFF = "OFF";

        public static string IN = "IN";

        public static string TOP = "TOP";
        public static string CASE = "CASE";
        public static string WHEN = "WHEN";
        public static string THEN = "THEN";
        public static string ELSE = "ELSE";
        public static string END = "END";
    }

    public static class SQLVariables
    {
        public static string CONTEXT_INFO = "CONTEXT_INFO";
    }
}