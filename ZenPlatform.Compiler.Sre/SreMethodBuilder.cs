using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ZenPlatform.Compiler.Contracts;
using TypeExtensions = System.Reflection.TypeExtensions;

namespace ZenPlatform.Compiler.Sre
{
    class SreMethodBuilder : SreMethodBase, IMethodBuilder, ISreMethod
    {
        private int _parameterIndex = 0;

        public SreTypeSystem System { get; }

        public MethodInfo Method => _methodBuilder;

        private List<SreDefferedParameter> _parameters = new List<SreDefferedParameter>();
        private readonly MethodBuilder _methodBuilder;
        private Type _returnType;

        public SreMethodBuilder(SreTypeSystem system, MethodBuilder methodBuilder) : base(system,
            methodBuilder)
        {
            System = system;
            _methodBuilder = methodBuilder;
            Generator = new SreEmitter(system, new SreMethodEmitterProvider(methodBuilder));
        }

        public IEmitter Generator { get; }

        public IParameter WithParameter(IType type, bool isOut, bool isRef)
        {
            return WithParameter($"P_{_parameterIndex}", type, isOut, isRef);
        }


        public IParameter WithParameter(string name, IType type, bool isOut, bool isRef)
        {
            var result = new SreDefferedParameter(this, name, type, _parameterIndex++);
            _parameters.Add(result);
            UpdateSignature();
            return result;
        }

        public IMethodBuilder WithReturnType(IType type)
        {
            _returnType = System.GetType(type);

            UpdateSignature();
            return this;
        }

        private void UpdateSignature()
        {
            _methodBuilder.SetSignature(
                _returnType,
                null,
                null,
                _parameters.Select(x => System.GetType(x.Type)).ToArray(),
                null,
                null);
        }

        internal bool IsBaked { get; private set; }

        public void Bake()
        {
            foreach (var p in _parameters)
            {
                p.Bake();
            }

            IsBaked = true;
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