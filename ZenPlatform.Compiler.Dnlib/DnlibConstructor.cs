using System;
using System.Collections.Generic;
using dnlib.DotNet;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Dnlib
{
    public class DnlibConstructor : IConstructor
    {
        public MethodDef MethodDef { get; }

        public DnlibConstructor(MethodDef methodDef)
        {
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
}