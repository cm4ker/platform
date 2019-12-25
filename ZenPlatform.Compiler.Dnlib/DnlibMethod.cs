using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using dnlib.DotNet;
using ZenPlatform.Compiler.Contracts;
using IMethod = ZenPlatform.Compiler.Contracts.IMethod;
using IType = ZenPlatform.Compiler.Contracts.IType;

namespace ZenPlatform.Compiler.Dnlib
{
    public abstract class DnlibMethodBase : IMethod
    {
        private ITypeDefOrRef _declaringTR;

        public DnlibMethodBase(DnlibTypeSystem typeSystem, dnlib.DotNet.IMethod method, MethodDef methodDef,
            ITypeDefOrRef declaringType)
        {
            TypeSystem = typeSystem;
            MethodRef = method;
            MethodDef = methodDef;
            ContextResolver = new DnlibContextResolver(typeSystem, declaringType.Module);
            _declaringTR = declaringType;
        }

        private void UpdateReferenceInfo()
        {
        }

        protected DnlibContextResolver ContextResolver { get; }

        public DnlibTypeSystem TypeSystem { get; }

        public MethodDef MethodDef { get; }

        public dnlib.DotNet.IMethod MethodRef { get; }

        public string Name => MethodDef.Name;

        public IType ReturnType => ContextResolver.GetType(MethodDef.ReturnType);
        public IType DeclaringType => ContextResolver.GetType(_declaringTR);

        public IMethod MakeGenericMethod(IType[] typeArguments)
        {
            if (MethodRef is IMethodDefOrRef mdr)
            {
                var sig = new GenericInstMethodSig(typeArguments.Select(x => ((DnlibType) x).TypeRef.ToTypeSig())
                    .ToArray());

                var generic = new MethodSpecUser(mdr, sig);

                return new DnlibMethod(TypeSystem, generic, generic.ResolveMethodDef(), _declaringTR);
            }

            throw new Exception("Can't create generic method");
        }

        protected ITypeDefOrRef DeclaringTypeReference => _declaringTR;

        public bool IsPublic => MethodDef.IsPublic;
        public bool IsStatic => MethodDef.IsStatic;

        public IReadOnlyList<IParameter> Parameters =>
            _parameters ??= MethodDef.Parameters
                .Where(x => !x.IsHiddenThisParameter)
                .Select(p => new DnlibParameter(TypeSystem, MethodDef, _declaringTR.Module, p))
                .ToList();

        private List<DnlibParameter> _parameters;

        public bool Equals(IMethod other)
        {
            throw new NotImplementedException();
        }
    }


    public class DnlibMethod : DnlibMethodBase
    {
        public MethodDef MethodDef { get; }

        public DnlibMethod(DnlibTypeSystem ts, dnlib.DotNet.IMethod method, MethodDef methodDef,
            ITypeDefOrRef declaringType) : base(ts, method, methodDef,
            declaringType)
        {
            MethodDef = methodDef;
        }

        public bool Equals(IMethod other)
        {
            throw new NotImplementedException();
        }

        public IMethod MakeGenericMethod(IType[] typeArguments)
        {
            throw new NotImplementedException();
        }
    }

    internal static class DnlibExtensions
    {
        public static ITypeDefOrRef ToTypeRef(this IType type)
        {
            return ((DnlibType) type).TypeRef;
        }

        public static ITypeDefOrRef ToTypeRef(this TypeDef type)
        {
            return new TypeRefUser(type.Module, type.Namespace, type.Name);
        }

        public static ITypeDefOrRef ToTypeRef(this ITypeDefOrRef type)
        {
            return new TypeRefUser(type.Module, type.Namespace, type.Name);
        }
    }
}