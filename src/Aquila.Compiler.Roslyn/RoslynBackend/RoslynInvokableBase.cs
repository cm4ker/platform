using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using dnlib.DotNet;

namespace Aquila.Compiler.Roslyn.RoslynBackend
{
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public abstract class RoslynInvokableBase : RoslynMemberBase
    {
        private ITypeDefOrRef _declaringTR;
        private List<RoslynParameter> _parameters;
        private List<RoslynCustomAttribute> _customAttributes;

        protected RoslynInvokableBase(RoslynTypeSystem typeSystem, dnlib.DotNet.IMethod method, MethodDef methodDef,
            ITypeDefOrRef declaringType)
        {
            System = typeSystem;
            MethodRef = method;
            MethodDef = methodDef;
            ContextResolver = new RoslynContextResolver(typeSystem, declaringType.Module);
            _declaringTR = declaringType;
        }

        protected RoslynContextResolver ContextResolver { get; }

        public RoslynTypeSystem System { get; }

        public MethodDef MethodDef { get; }

        public IMethod MethodRef { get; }

        public override string Name => MethodDef.Name;

        public RoslynType ReturnType => ContextResolver.GetType(MethodDef.ReturnType);

        public RoslynType DeclaringType => ContextResolver.GetType(_declaringTR);

        public RoslynMethod MakeGenericMethod(params RoslynType[] typeArguments)
        {
            if (MethodRef is IMethodDefOrRef mdr)
            {
                var sig = new GenericInstMethodSig(typeArguments.Select(x => ((RoslynType) x).TypeRef.ToTypeSig())
                    .ToArray());

                var generic = new MethodSpecUser(mdr, sig);

                return new RoslynMethod(System, generic, generic.ResolveMethodDef(), _declaringTR);
            }

            throw new Exception("Can't create generic method");
        }

        public IReadOnlyList<RoslynType> GenericArguments
        {
            get
            {
                if ((MethodRef is MethodSpec spec))
                {
                    return spec.GenericInstMethodSig.GenericArguments.Select(x => ContextResolver.GetType(x)).ToList();
                }

                return new List<RoslynType>();
            }
        }

        protected ITypeDefOrRef DeclaringTypeReference => _declaringTR;

        public bool IsPublic => MethodDef.IsPublic;
        public bool IsStatic => MethodDef.IsStatic;

        public bool IsGeneric => (MethodRef is MethodSpec);

        public virtual IReadOnlyList<RoslynParameter> Parameters =>
            _parameters ??= CalculateParameters();

        public List<RoslynParameter> CalculateParameters()
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

                    return new RoslynParameter(System, MethodDef, _declaringTR.Module, p);
                })
                .ToList();
        }


        public IReadOnlyList<RoslynCustomAttribute> CustomAttributes =>
            _customAttributes ??= new List<RoslynCustomAttribute>();


        public bool Equals(IMethod other)
        {
            throw new NotImplementedException();
        }
    }
}