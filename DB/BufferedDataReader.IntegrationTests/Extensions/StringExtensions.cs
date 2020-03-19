namespace BufferedDataReaderDotNet.IntegrationTests.Extensions
{
    public static class StringExtensions
    {
        public static string QuoteName(this string name)
        {
            return $"[{name.Replace("]", "]]")}]";
        }
    }
}