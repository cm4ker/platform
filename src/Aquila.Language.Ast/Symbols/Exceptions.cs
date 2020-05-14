using System;

namespace Aquila.Language.Ast.Symbols
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