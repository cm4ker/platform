using System;

namespace ZenPlatform.Compiler.AST.Infrastructure
{
    public class SymbolException : Exception
    {
        public SymbolException(string message) : base(message)
        {
        }
    }

    public class VerifierException : Exception
    {
        public VerifierException(string message) : base(message)
        {
        }
    }
}