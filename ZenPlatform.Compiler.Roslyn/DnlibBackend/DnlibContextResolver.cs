using System;
using dnlib.DotNet;

namespace ZenPlatform.Compiler.Roslyn.DnlibBackend
{
    public class SreContextResolver
    {
        private readonly SreTypeSystem _ts;
        private readonly ModuleDef _moduleDef;

        public SreContextResolver(SreTypeSystem ts, ModuleDef moduleDef)
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

        public SreType GetType(ITypeDefOrRef tr)
        {
            if (tr is TypeRef)
                return _ts.Resolve(tr);
            if (tr is TypeDefUser)
                return _ts.Resolve(tr);
            if (tr is TypeSpec)
                return _ts.Resolve(tr);
            else
                return _ts.Resolve(tr.ToTypeRef());
        }

        public MethodSig ResolveMethodSig(MethodSig msig, SreType[] genericArguments)
        {
            if (!msig.RetType.ContainsGenericParameter)
            {
                SreType dt = (SreType) GetType(msig.RetType);

                if (dt != null)
                    msig.RetType = dt.TypeRef.ToTypeSig();
            }

            for (int i = 0; i < msig.Params.Count; i++)
            {
                if (!msig.Params[i].ContainsGenericParameter)
                {
                    SreType dt = (SreType) GetType(msig.Params[i]);

                    if (dt != null)
                        msig.Params[i] = dt.TypeRef.ToTypeSig();
                }
            }

            return msig;
        }

        public SreType GetType(TypeSig tsig)
        {
            if (tsig.AssemblyQualifiedName.Contains("System.Private.CoreLib"))
            {
                if (tsig.IsGenericInstanceType)
                    return _ts.FindAssembly(_ts.GetSystemBindings().MSCORLIB)
                        .FindType(tsig.ToTypeDefOrRef().ScopeType.FullName);
                else
                    return _ts.FindAssembly(_ts.GetSystemBindings().MSCORLIB).FindType(tsig.FullName);
            }
            else
            {
                var result = _ts.Resolve(tsig.ToTypeDefOrRef());

                return result;
            }
        }
    }
}