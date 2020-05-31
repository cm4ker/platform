namespace Aquila.Compiler.Aqua
{
    public class BackendTypeProvider
    {
        private readonly RoslynTypeSystem _ts;

        public BackendTypeProvider(RoslynTypeSystem ts)
        {
            _ts = ts;
        }

        public object FindType(string @namespace, string name)
        {
            return FindType($"{@namespace}.{name}");
        }

        public RoslynType FindType(string fullName)
        {
            return _ts.FindType(fullName);
        }
    }
}