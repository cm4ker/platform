using System.Reflection;

namespace Aquila.Web.Razor;

public interface IAquilaAssemblyReciver
{
    public Assembly[] AdditionalAssemblies { get; }
}