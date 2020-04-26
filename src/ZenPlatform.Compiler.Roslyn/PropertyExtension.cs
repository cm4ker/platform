using ZenPlatform.Compiler.Roslyn.RoslynBackend;

namespace ZenPlatform.Compiler.Roslyn
{
    public static class PropertyExtension
    {
        public static RoslynCustomAttribute FindCustomAttribute<T>(this RoslynProperty property)
        {
            var type = property.PropertyType.Assembly.TypeSystem.Resolve<T>();
            return property.FindCustomAttribute(type);
        }
    }
}