namespace ZenPlatform.Compiler.Contracts
{
    public static class TypeSystemExtensions
    {
        public static ICustomAttributeBuilder CreateAttribute<T>(this IAssembly ab, params IType[] args)
        {
            var type = ab.TypeSystem.FindType<T>();
            return ab.TypeSystem.Factory.CreateAttribute(ab.TypeSystem, type, args);
        }

        public static ICustomAttributeBuilder CreateAttribute<T>(this ITypeSystem ts, params IType[] args)
        {
            var type = ts.FindType<T>();
            return ts.Factory.CreateAttribute(ts, type, args);
        }

        public static ICustomAttributeBuilder CreateAttribute<T>(this IType ts, params IType[] args)
        {
            return ts.Assembly.CreateAttribute<T>(args);
        }
    }
}