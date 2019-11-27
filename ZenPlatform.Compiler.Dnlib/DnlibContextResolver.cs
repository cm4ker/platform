using System;
using dnlib.DotNet;
using ZenPlatform.Compiler.Contracts;
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

            throw new Exception("This reference not supported");
        }

        public IType GetType(ITypeDefOrRef tr) => _ts.Resolve(tr.ToTypeRef());

        public IType GetType(TypeSig tsig)
        {
            if (tsig.AssemblyQualifiedName.Contains("System.Private.CoreLib"))
            {
                return _ts.Resolve(new TypeRefUser(_moduleDef, tsig.Namespace, tsig.TypeName, _moduleDef));
            }
            else
            {
                return _ts.Resolve(tsig.TryGetTypeRef());
            }
        }
    }
}