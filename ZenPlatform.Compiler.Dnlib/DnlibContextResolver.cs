using System;
using dnlib.DotNet;
using IType = ZenPlatform.Compiler.Contracts.IType;

namespace ZenPlatform.Compiler.Dnlib
{
    public class DnlibContextResolver
    {
        private readonly DnlibTypeSystem _ts;
        private readonly ModuleDef _moduleDef;

        public DnlibContextResolver(DnlibTypeSystem ts, ModuleDef moduleDef)
        {
            _ts = ts;
            _moduleDef = moduleDef ?? throw new ArgumentNullException(nameof(moduleDef));
        }

        public TypeRef GetReference(TypeRef type)
        {
            return _moduleDef.Import(type);
        }

        public IType GetType(ITypeDefOrRef tr) => _ts.Resolve(tr.ToTypeRef());

        public IType GetType(TypeSig tsig) => _ts.Resolve(tsig.TryGetTypeRef());
    }
}