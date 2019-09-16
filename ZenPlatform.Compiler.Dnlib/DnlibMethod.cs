using System;
using System.Collections.Generic;
using dnlib.DotNet;
using ZenPlatform.Compiler.Contracts;
using IMethod = ZenPlatform.Compiler.Contracts.IMethod;
using IType = ZenPlatform.Compiler.Contracts.IType;

namespace ZenPlatform.Compiler.Dnlib
{
    public class DnlibMethod : IMethod
    {
        public MethodDef MethodDef { get; }

        public DnlibMethod(MethodDef methodDef)
        {
            MethodDef = methodDef;
        }

        public bool Equals(IMethod other)
        {
            throw new NotImplementedException();
        }

        public string Name { get; }
        public bool IsPublic { get; }
        public bool IsStatic { get; }
        public IType ReturnType { get; }
        public IReadOnlyList<IParameter> Parameters { get; }
        public IType DeclaringType { get; }

        public IMethod MakeGenericMethod(IType[] typeArguments)
        {
            throw new NotImplementedException();
        }
    }
}