using System;
using Mono.Cecil;
using ZenPlatform.Compiler.Cecil.Backend;

namespace ZenPlatform.Compiler
{
    public interface IType : IAstNode
    {
        string Name { get; set; }

        bool IsArray { get; }
    }

    public class UnresolvedType : IType
    {
        public UnresolvedType(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public bool IsArray => throw new NotImplementedException();
    }

    public class CecilType : IType
    {
        private readonly TypeReference _tr;
        private readonly TypeSystem _typeSystem;

        public CecilType(TypeReference tr, TypeSystem typeSystem)
        {
            _tr = tr;
            _typeSystem = typeSystem;
        }

        public string Name
        {
            get => _tr.Name;
            set => throw new NotImplementedException();
        }

        public bool IsArray => _tr.IsArray;
    }
}