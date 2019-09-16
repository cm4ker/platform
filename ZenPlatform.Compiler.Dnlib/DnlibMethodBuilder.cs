using System;
using dnlib.DotNet;
using ZenPlatform.Compiler.Contracts;
using IMethod = ZenPlatform.Compiler.Contracts.IMethod;
using IType = ZenPlatform.Compiler.Contracts.IType;

namespace ZenPlatform.Compiler.Dnlib
{
    public class DnlibMethodBuilder : DnlibMethodBase, IMethodBuilder
    {
        public bool Equals(IMethod other)
        {
            throw new NotImplementedException();
        }

        public IMethod MakeGenericMethod(IType[] typeArguments)
        {
            throw new NotImplementedException();
        }

        public IParameter DefineParameter(string name, IType type, bool isOut, bool isRef)
        {
            var dtype = (DnlibType) type;
            
            var typeSig = dtype.TypeRef.ToTypeSig();
            MethodDef.MethodSig.Params.Add(typeSig);
            
            var a = new ParamDefUser(name);
            
            MethodDef.ParamDefs.Add(a);
            MethodDef.Parameters.UpdateParameterTypes();
            
            var p = MethodDef.Parameters[a.Sequence];

            return new DnlibParameter(TypeSystem, MethodDef, p);
        }

        public IMethodBuilder WithReturnType(IType type)
        {
            MethodDef.ReturnType = ContextResolver.GetReference(type.ToTypeRef()).ToTypeSig();
            return this;
        }

        public DnlibMethodBuilder(DnlibTypeSystem typeSystem, IMethodDefOrRef method, TypeRef declaringType) :
            base(typeSystem, method, declaringType)
        {
        }
    }
}