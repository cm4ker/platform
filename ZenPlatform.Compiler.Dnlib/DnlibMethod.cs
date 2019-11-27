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

        public DnlibMethodBase(DnlibTypeSystem typeSystem, IMethodDefOrRef method, ITypeDefOrRef declaringType)
        {
            TypeSystem = typeSystem;
            MethofRef = method;
            MethodDef = method.ResolveMethodDef() ?? throw new ArgumentNullException();
            ContextResolver = new DnlibContextResolver(typeSystem, method.Module);
            _declaringTR = declaringType;
        }

        private void UpdateReferenceInfo()
        {
        }

        protected DnlibContextResolver ContextResolver { get; }

        public DnlibTypeSystem TypeSystem { get; }

        public MethodDef MethodDef { get; }

        public IMethodDefOrRef MethofRef { get; }

        public string Name => MethodDef.Name;

        public IType ReturnType => ContextResolver.GetType(MethodDef.ReturnType);
        public IType DeclaringType => ContextResolver.GetType(_declaringTR);

        public IMethod MakeGenericMethod(IType[] typeArguments)
        {
            throw new NotImplementedException();
        }

        protected ITypeDefOrRef DeclaringTypeReference => _declaringTR;

        public bool IsPublic => MethodDef.IsPublic;
        public bool IsStatic => MethodDef.IsStatic;

        public IReadOnlyList<IParameter> Parameters => 
            _parameters ??= MethodDef.Parameters
            .Where(x => !x.IsHiddenThisParameter)
            .Select(p => new DnlibParameter(TypeSystem, MethodDef, p))
            .ToList();


        private IEmitter _generator;
        private List<DnlibParameter> _parameters;
        
        public IEmitter Generator => _generator ??= new DnlibEmitter(TypeSystem, MethodDef);

        public bool Equals(IMethod other)
        {
            throw new NotImplementedException();
        }
    }


    public class DnlibMethod : DnlibMethodBase
    {
        public MethodDef MethodDef { get; }

        public DnlibMethod(DnlibTypeSystem ts, MethodDef methodDef, ITypeDefOrRef declaringType) : base(ts, methodDef,
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
            return ((DnlibType) type).TypeRef.ToTypeSig().TryGetTypeRef();
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