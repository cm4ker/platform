using System.Collections.Generic;
using dnlib.DotNet;
using IModule = Aquila.Compiler.Contracts.IModule;
using IType = Aquila.Compiler.Contracts.IType;

namespace Aquila.Compiler.Dnlib
{
    public class DnlibModule : IModule
    {
        private readonly DnlibTypeSystem _ts;
        private readonly ModuleDef _dnlibModule;
        private DnlibContextResolver _cr;

        public DnlibModule(DnlibTypeSystem ts, ModuleDef dnlibModule)
        {
            _ts = ts;
            _dnlibModule = dnlibModule;
            _cr = new DnlibContextResolver(_ts, dnlibModule);
        }

        public bool Equals(IModule other)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IType> GetTypes()
        {
            foreach (var type in _dnlibModule.Types)
            {
                if (type.FullName != "<Module>")
                    yield return _cr.GetType(type);
            }

            foreach (var type in _dnlibModule.ExportedTypes)
            {
                var resolved = type.Resolve();
                if (resolved != null)
                    yield return _cr.GetType(resolved);
            }
        }
    }
}