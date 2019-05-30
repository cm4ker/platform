using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Sre
{
    class SreMethodBuilder : SreMethodBase, IMethodBuilder, ISreMethod
    {
        private int _parameterIndex = 1;

        public SreTypeSystem System { get; }

        public MethodInfo Method => _methodBuilder;

        private List<Type> _parameters = new List<Type>();
        private readonly MethodBuilder _methodBuilder;

        public SreMethodBuilder(SreTypeSystem system, MethodBuilder methodBuilder) : base(system,
            methodBuilder)
        {
            System = system;
            _methodBuilder = methodBuilder;
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
            _methodBuilder.SetParameters(_parameters.ToArray());
            _methodBuilder.DefineParameter(_parameterIndex, ParameterAttributes.None, name);
            _parameterIndex++;

            return this;
        }

        public IMethodBuilder WithReturnType(IType type)
        {
            _methodBuilder.SetReturnType(System.GetType(type));
            return this;
        }

        public void EmitClosure(IEnumerable<IType> fields)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IMethod other)
        {
            return ((SreMethodBuilder) other)?._methodBuilder.Equals(_methodBuilder) == true;
        }

        public IType ReturnType => System.ResolveType(_methodBuilder.ReturnType);
        public IType DeclaringType => System.ResolveType(_methodBuilder.DeclaringType);
    }
}