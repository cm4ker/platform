namespace Aquila.Compiler.Roslyn
{
    public static class ConventionsHelper
    {
        public static string GetBackingFieldName(string name) => $"{name}k__BackingField";
    }
}