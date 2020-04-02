using System;
using dnlib.DotNet;
using dnlib.DotNet.Resources;
using ZenPlatform.Compiler.Contracts;
using IField = dnlib.DotNet.IField;
using IMethod = dnlib.DotNet.IMethod;
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

        public IMethod GetReference(IMethod method)
        {
            return _moduleDef.Import(method);
        }

        public IType GetType(ITypeDefOrRef tr)
        {
            if (tr is TypeDefUser)
                return _ts.Resolve(tr);
            else
                return _ts.Resolve(tr.ToTypeRef());
        }

        public MethodSig ResolveMethodSig(MethodSig msig, IType[] genericArguments)
        {
            if (!msig.RetType.ContainsGenericParameter)
            {
                DnlibType dt = (DnlibType) GetType(msig.RetType);

                if (dt != null)
                    msig.RetType = dt.TypeRef.ToTypeSig();
            }

            for (int i = 0; i < msig.Params.Count; i++)
            {
                if (!msig.Params[i].ContainsGenericParameter)
                {
                    DnlibType dt = (DnlibType) GetType(msig.Params[i]);

                    if (dt != null)
                        msig.Params[i] = dt.TypeRef.ToTypeSig();
                }
            }

            return msig;
        }

        public IType GetType(TypeSig tsig)
        {
            if (tsig.AssemblyQualifiedName.Contains("System.Private.CoreLib"))
            {
                if (tsig.IsGenericInstanceType)
                    return _ts.FindAssembly(_ts.GetSystemBindings().MSCORLIB).FindType(tsig.ToTypeDefOrRef().ScopeType.FullName);
                else
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