using System.Collections.Generic;
using System.IO;

namespace ZenPlatform.Compiler.Roslyn
{
    // public class RAssemblyBuilder : TmpSreBackendAssembly
    // {
    //     private List<RModuleBuilder> _modules = new List<RModuleBuilder>();
    //
    //     public RAssemblyBuilder()
    //     {
    //         TypeSystem = new RTypeSystem();
    //     }
    //
    //     public RModuleBuilder DefineModule()
    //     {
    //         var mb = new RModuleBuilder(this);
    //
    //         _modules.Add(mb);
    //         return mb;
    //     }
    //
    //     public RTypeSystem TypeSystem { get; }
    //
    //     public void Dump(string directory)
    //     {
    //         foreach (var module in _modules)
    //         {
    //             // module.Dump();
    //         }
    //     }
    //
    //     public void Dump(TextWriter textWriter)
    //     {
    //         foreach (var module in _modules)
    //         {
    //             module.Dump(textWriter);
    //         }
    //     }
    //
    //     public void Compile(string outPath)
    //     {
    //     }
    // }
}