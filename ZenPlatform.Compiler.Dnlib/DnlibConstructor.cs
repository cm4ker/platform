using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ZenPlatform.Compiler.Contracts;
using IType = ZenPlatform.Compiler.Contracts.IType;

namespace ZenPlatform.Compiler.Dnlib
{
    public class DnlibConstructorBase : IConstructor
    {
        public DnlibTypeSystem TypeSystem { get; }
        public MethodDef MethodDef { get; }

        public DnlibConstructorBase(DnlibTypeSystem typeSystem, MethodDef methodDef)
        {
            TypeSystem = typeSystem;
            MethodDef = methodDef;
        }

        public bool Equals(IConstructor other)
        {
            throw new NotImplementedException();
        }

        public bool IsPublic { get; }
        public bool IsStatic { get; }
        public IReadOnlyList<IParameter> Parameters { get; }
    }


    public class DnlibConstructor : DnlibConstructorBase
    {
        public DnlibConstructor(MethodDef methodDef) : base(null, methodDef)
        {
        }
    }

    public class DnlibConstructorBuilder : DnlibConstructorBase, IConstructorBuilder
    {
        private readonly MethodDefUser _methodDef;

        public DnlibConstructorBuilder(MethodDefUser methodDef) : base(null, methodDef)
        {
            _methodDef = methodDef;
            _methodDef.Body = new CilBody();
            Generator = new DnlibEmitter(TypeSystem, methodDef);
        }

        public IEmitter Generator { get; }

        public IParameter DefineParameter(IType type)
        {
            _methodDef.MethodSig.Params.Add(new ClassSig(((DnlibType) type).TypeRef));
            _methodDef.Parameters.UpdateParameterTypes();
            return new DnlibParameter(TypeSystem, _methodDef, _methodDef.Module, _methodDef.Parameters.Last());
        }
    }
}