using System.Collections.Generic;

namespace ZenPlatform.Compiler.Contracts.Symbols
{
    public interface ISymbolTable
    {
        ISymbol Add(IAstSymbol astSymbol);

        ISymbol Add(string name, SymbolType type, SymbolScopeBySecurity scope, IAstSymbol syntaxObject,
            object codeObject);

        ISymbol ConnectCodeObject(IAstSymbol v, object codeObject);

        bool Contains(string name, SymbolType type, SymbolScopeBySecurity scope);

        ISymbol Find(IAstSymbol symbol);

        T FindCodeObject<T>(IAstSymbol symbol);

        ISymbol Find(string name, SymbolType type, SymbolScopeBySecurity scope);

        IEnumerable<ISymbol> GetAll(SymbolType type);


        void Clear();
    }
}