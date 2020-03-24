using System;
using System.Reflection;
using System.Runtime.Loader;

namespace ZenPlatform.Compiler.Tests
{
    class HostAssemblyLoadContext : AssemblyLoadContext
    {
        // Resolver of the locations of the assemblies that are dependencies of the

        // main plugin assembly.

        //   private AssemblyDependencyResolver _resolver;


        public HostAssemblyLoadContext(string pluginPath) : base(isCollectible: true)

        {
            // _resolver = new AssemblyDependencyResolver(pluginPath);
        }


        // The Load method override causes all the dependencies present in the plugin's binary directory to get loaded

        // into the HostAssemblyLoadContext together with the plugin assembly itself.

        // NOTE: The Interface assembly must not be present in the plugin's binary directory, otherwise we would

        // end up with the assembly being loaded twice. Once in the default context and once in the HostAssemblyLoadContext.

        // The types present on the host and plugin side would then not match even though they would have the same names.

        protected override Assembly Load(AssemblyName name)

        {
            return null;
        }
    }
}