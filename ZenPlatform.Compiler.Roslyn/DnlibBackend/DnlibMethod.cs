using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using dnlib.DotNet;

namespace ZenPlatform.Compiler.Roslyn.DnlibBackend
{
    public abstract class MemberBase
    {
        public virtual string Name { get; }

        public virtual void DumpRef(TextWriter tw)
        {
            tw.W(Name);
        }
    }

    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public abstract class SreInvokableBase : MemberBase
    {
        private ITypeDefOrRef _declaringTR;

        protected SreInvokableBase(SreTypeSystem typeSystem, dnlib.DotNet.IMethod method, MethodDef methodDef,
            ITypeDefOrRef declaringType)
        {
            System = typeSystem;
            MethodRef = method;
            MethodDef = methodDef;
            ContextResolver = new SreContextResolver(typeSystem, declaringType.Module);
            _declaringTR = declaringType;
        }

        private void UpdateReferenceInfo()
        {
        }

        protected SreContextResolver ContextResolver { get; }

        public SreTypeSystem System { get; }

        public MethodDef MethodDef { get; }

        public dnlib.DotNet.IMethod MethodRef { get; }

        public override string Name => MethodDef.Name;

        public SreType ReturnType => ContextResolver.GetType(MethodDef.ReturnType);

        public SreType DeclaringType => ContextResolver.GetType(_declaringTR);

        public SreMethod MakeGenericMethod(SreType[] typeArguments)
        {
            if (MethodRef is IMethodDefOrRef mdr)
            {
                var sig = new GenericInstMethodSig(typeArguments.Select(x => ((SreType) x).TypeRef.ToTypeSig())
                    .ToArray());

                if (sig == null)
                    throw new Exception("sig is null");

                var generic = new MethodSpecUser(mdr, sig);

                return new SreMethod(System, generic, generic.ResolveMethodDef(), _declaringTR);
            }

            throw new Exception("Can't create generic method");
        }

        public IReadOnlyList<SreType> GenericArguments
        {
            get
            {
                if ((MethodRef is MethodSpec spec))
                {
                    return spec.GenericInstMethodSig.GenericArguments.Select(x => ContextResolver.GetType(x)).ToList();
                }

                return new List<SreType>();
            }
        }

        protected ITypeDefOrRef DeclaringTypeReference => _declaringTR;

        public bool IsPublic => MethodDef.IsPublic;
        public bool IsStatic => MethodDef.IsStatic;

        public bool IsGeneric => (MethodRef is MethodSpec);

        public virtual IReadOnlyList<SreParameter> Parameters =>
            _parameters ??= CalculateParameters();

        public List<SreParameter> CalculateParameters()
        {
            void ContextGenericResolver(TypeSig sig, Parameter param)
            {
                if (param.Type is GenericInstSig gis && _declaringTR.ToTypeSig() is GenericInstSig ts)
                {
                    for (int i = 0; i < gis.GenericArguments.Count; i++)
                    {
                        if (gis.GenericArguments[i] is GenericVar gv)
                        {
                            gis.GenericArguments[i] = ts.GenericArguments[(int) gv.Number];
                        }
                    }
                }

                if (param.Type is GenericVar gv2 && _declaringTR.ToTypeSig() is GenericInstSig ts2)
                {
                    param.Type = ts2.GenericArguments[(int) gv2.Number];
                }
            }


            return MethodDef.Parameters
                .Where(x => !x.IsHiddenThisParameter)
                .Select(p =>
                {
                    ContextGenericResolver(_declaringTR.ToTypeSig(), p);

                    return new SreParameter(System, MethodDef, _declaringTR.Module, p);
                })
                .ToList();
        }


        private List<SreParameter> _parameters;

        public bool Equals(IMethod other)
        {
            throw new NotImplementedException();
        }
    }
}