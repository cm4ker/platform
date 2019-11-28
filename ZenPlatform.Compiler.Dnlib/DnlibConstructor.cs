using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ZenPlatform.Compiler.Contracts;
using IType = ZenPlatform.Compiler.Contracts.IType;

namespace ZenPlatform.Compiler.Dnlib
{
    public class DnlibConstructor : DnlibMethodBase, IConstructor
    {
        public DnlibConstructor(DnlibTypeSystem ts, IMethodDefOrRef method, MethodDef methodDef, ITypeDefOrRef declType)
            : base(ts, method, methodDef,
                declType)
        {
        }

        public bool Equals(IConstructor other)
        {
            throw new NotImplementedException();
        }
    }

    public class DnlibConstructorBuilder : DnlibConstructor, IConstructorBuilder
    {
        private readonly MethodDefUser _methodDef;

        public DnlibConstructorBuilder(DnlibTypeSystem ts, MethodDefUser methodDef, ITypeDefOrRef declType) : base(ts,
            methodDef, methodDef, declType)
        {
            _methodDef = methodDef;
            _methodDef.Body = new CilBody() {KeepOldMaxStack = true};
        }

        private IEmitter _generator;
        public IEmitter Generator => _generator ??= new DnlibEmitter(TypeSystem, MethodDef);


        public IParameter DefineParameter(IType type)
        {
            var dtype = (DnlibType) type;
            
            var typeSig = dtype.TypeRef.ToTypeSig();
            MethodDef.MethodSig.Params.Add(typeSig);
            
            MethodDef.Parameters.UpdateParameterTypes();

            var p = MethodDef.Parameters.Last();
            p.CreateParamDef();
             
            var dp = new DnlibParameter(TypeSystem, MethodDef, DeclaringTypeReference.Module, p);
            
            ((List<DnlibParameter>)Parameters).Add(dp);
            
            return dp;
        }
    }
}