using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Sre
{
    class SreMethodBuilder : SreMethodBase, IMethodBuilder
    {
        private int _parameterIndex = 1;

        public SreTypeSystem System { get; }
        public MethodBuilder MethodBuilder { get; }

        private List<Type> _parameters = new List<Type>();

        public SreMethodBuilder(SreTypeSystem system, MethodBuilder methodBuilder) : base(system,
            methodBuilder)
        {
            System = system;
            MethodBuilder = methodBuilder;
            Generator = new SreEmitter(system, new SreMethodEmitterProvider(methodBuilder));
        }

        public IEmitter Generator { get; }

        public IMethodBuilder WithParameter(IType type, bool isOut, bool isRef)
        {
            return WithParameter(null, type, isOut, isRef);
        }

        public IMethodBuilder WithParameter(string name, IType type, bool isOut, bool isRef)
        {
            _parameters.Add(System.GetType(type));
            MethodBuilder.SetParameters(_parameters.ToArray());
            MethodBuilder.DefineParameter(_parameterIndex, ParameterAttributes.None, name);
            _parameterIndex++;

            return this;
        }

        public IMethodBuilder WithReturnType(IType type)
        {
            MethodBuilder.SetReturnType(System.GetType(type));
            return this;
        }

        public void EmitClosure(IEnumerable<IType> fields)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IMethod other)
        {
            return ((SreMethodBuilder) other)?.MethodBuilder.Equals(MethodBuilder) == true;
        }

        public IType ReturnType => System.ResolveType(MethodBuilder.ReturnType);
        public IType DeclaringType => System.ResolveType(MethodBuilder.DeclaringType);
    }
}