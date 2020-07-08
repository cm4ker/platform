using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.Public;

namespace Aquila.Component1
{
    public class Component1 : IAnalysesComponent
    {
        public AquilaCompilation PopulateAssemblyReference(AquilaCompilation compilation)
        {
            // for this component no more runtimes needed

            return compilation;
        }

        public AquilaCompilation PopulateTypes(AquilaCompilation compilation)
        {
            var tm = compilation.ComponentTypeManager;
        }

        public AquilaCompilation PopulateMembers(AquilaCompilation compilation)
        {
            throw new System.NotImplementedException();
        }
    }
}