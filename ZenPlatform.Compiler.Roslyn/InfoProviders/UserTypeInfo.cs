namespace ZenPlatform.Compiler.Roslyn.InfoProviders
{
    public class UserTypeInfo
    {
        public string Name { get; set; }

        public string FullName { get; set; }

        public string Namespace { get; set; }

        public bool IsPublic { get; set; }

        public bool IsNestedAssembly { get; set; }
    }
}