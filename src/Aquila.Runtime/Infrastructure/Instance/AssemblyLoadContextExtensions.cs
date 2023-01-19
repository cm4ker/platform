using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Aquila.Core.Instance;

public static class AssemblyLoadContextExtensions
{
    public static Assembly LoadFromByteArray(this AssemblyLoadContext context, byte[] bytes)
    {
        using var ms = new MemoryStream(bytes);
        return context.LoadFromStream(ms);
    }
}