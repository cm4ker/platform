namespace ZenPlatform.SyntaxGenerator
{
    public static class StringExt
    {
        public static string ToCamelCase(this string str)
        {
            return char.ToLower(str[0]) + str[1..];
            
        }

        public static string ToUpCase(this string str)
        {
            return char.ToUpper(str[0]) + str[1..];
        }
    }
}