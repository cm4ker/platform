using Aquila.Compiler.Roslyn.RoslynBackend;

namespace Aquila.Compiler.Roslyn
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