using System;
using dnlib.DotNet;
using ZenPlatform.Compiler.Contracts;
using IField = dnlib.DotNet.IField;
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

            if (moduleDef is null)
            {
                throw new ArgumentNullException(nameof(moduleDef));
            }

            _moduleDef = moduleDef ?? throw new ArgumentNullException(nameof(moduleDef));
        }

        public ITypeDefOrRef GetReference(ITypeDefOrRef type)
        {
            if (type is TypeRef tr)
            {
                return _moduleDef.Import(tr);
            }
            else if (type is TypeSpec ts)
            {
                return _moduleDef.Import(ts);
            }
            else if (type is TypeDefUser td)
                return td;


            throw new Exception("This reference not supported");
        }

        public IType GetType(ITypeDefOrRef tr) => _ts.Resolve(tr.ToTypeRef());

        public IType GetType(TypeSig tsig)
        {
            if (tsig.AssemblyQualifiedName.Contains("System.Private.CoreLib"))
            {
                return _ts.FindAssembly(_ts.GetSystemBindings().MSCORLIB).FindType(tsig.FullName);
            }
            else
            {
                var result = _ts.Resolve(tsig.ToTypeDefOrRef());
                if (result is UnresolvedDnlibType)
                {
                    result = _ts.Resolve(tsig.ToTypeDefOrRef());
                    throw new Exception($"Can't resolve type {tsig}");
                }

                return result;
            }
        }
    }
}