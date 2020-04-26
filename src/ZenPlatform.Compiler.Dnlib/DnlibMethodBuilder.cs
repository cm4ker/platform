using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using ZenPlatform.Compiler.Contracts;
using IMethod = ZenPlatform.Compiler.Contracts.IMethod;
using IType = ZenPlatform.Compiler.Contracts.IType;

namespace ZenPlatform.Compiler.Dnlib
{
    public class DnlibMethodBuilder : DnlibMethodBase, IMethodBuilder
    {
        private IEmitter _generator;
        public IEmitter Generator => _generator ??= new DnlibEmitter(TypeSystem, MethodDef);

        public IParameter DefineParameter(string name, IType type, bool isOut, bool isRef)
        {
            var dtype = (DnlibType) type;

            var typeSig = dtype.TypeRef.ToTypeSig();
            MethodDef.MethodSig.Params.Add(typeSig);
            MethodDef.Parameters.UpdateParameterTypes();

            var p = MethodDef.Parameters.Last();
            p.CreateParamDef();
            p.Name = name;

            var dp = new DnlibParameter(TypeSystem, MethodDef, DeclaringTypeReference.Module, p);

            ((List<DnlibParameter>) Parameters).Add(dp);

            return dp;
        }

        public IMethodBuilder WithReturnType(IType type)
        {
            MethodDef.ReturnType = ContextResolver.GetReference(type.ToTypeRef()).ToTypeSig();

            return this;
        }

        public DnlibMethodBuilder(DnlibTypeSystem typeSystem, MethodDef method, ITypeDefOrRef declaringType) :
            base(typeSystem, method, method, declaringType)
        {
            Parameters.Any();
        }
    }
}