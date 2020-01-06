namespace ZenPlatform.Compiler.Contracts
{
    public static class TypeSystemExtensions
    {
        public static ICustomAttributeBuilder CreateAttribute<T>(this IAssemblyBuilder ab, params IType[] args)
        {
            var type = ab.TypeSystem.FindType<T>();
            return ab.CreateAttribute(type, args);
        }
    }
}