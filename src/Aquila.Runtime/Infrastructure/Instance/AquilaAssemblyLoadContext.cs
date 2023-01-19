using System.Runtime.Loader;

namespace Aquila.Core.Instance;

public class AquilaAssemblyLoadContext : AssemblyLoadContext
{
    public AquilaAssemblyLoadContext() : base(true)
    {
    }

    public bool IsUnloading { get; private set; }

    public new void Unload()
    {
        IsUnloading = true;
        base.Unload();
    }
}