namespace ZenPlatform.SyntaxGenerator
{
    public static class StringExt
    {
        public static string ToCamelCase(this string str)
        {
            var result = char.ToLower(str[0]) + str[1..];

            if (result == "else" || result == "when")
                result = '@' + result;

            return result;
        }

        public static string ToUpCase(this string str)
        {
            return char.ToUpper(str[0]) + str[1..];
        }
    }
}