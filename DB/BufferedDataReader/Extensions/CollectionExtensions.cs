namespace BufferedDataReaderDotNet.Extensions
{
    internal static class CollectionExtensions
    {
        public static int IndexOf<T>(this System.Collections.Generic.IReadOnlyList<T> self, T elementToFind)
        {
            int i = 0;
            foreach (T element in self)
            {
                if (Equals(element, elementToFind))
                    return i;
                i++;
            }

            return -1;
        }
    }
}