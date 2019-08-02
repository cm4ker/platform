using System.Collections;
using ZenPlatform.Compiler.AST.Definitions.Symbols;
using ZenPlatform.Compiler.AST.Infrastructure;

namespace ZenPlatform.Compiler.Contracts.Symbols
{
    public interface ISymbolTable
    {
        ISymbol Add(IAstSymbol astSymbol);
        ISymbol Add(string name, SymbolType type, IAstSymbol syntaxObject, object codeObject);
        ISymbol ConnectCodeObject(IAstSymbol v, object codeObject);
        bool Contains(string name, SymbolType type);
        ISymbol Find(IAstSymbol symbol);
        T FindCodeObject<T>(IAstSymbol symbol);
        ISymbol Find(string name, SymbolType type);
        void Clear();
    }

    public class SymbolTable : ISymbolTable
    {
        private ISymbolTable _parent;
        private Hashtable _hashtable = new Hashtable();

        public SymbolTable()
        {
        }

        public SymbolTable(ISymbolTable parent)
        {
            _parent = parent;
        }

        public ISymbol Add(IAstSymbol astSymbol)
        {
            return Add(astSymbol.Name, astSymbol.SymbolType, astSymbol, null);
        }

        public ISymbol Add(string name, SymbolType type, IAstSymbol syntaxObject, object codeObject)
        {
            string prefix = PrefixFromType(type);

            if (_hashtable.Contains(prefix + name))
                throw new SymbolException("Symbol already exists in symbol table.");

            Symbol symbol = new Symbol(name, type, syntaxObject, codeObject);
            _hashtable.Add(prefix + name, symbol);

            return symbol;
        }

        public ISymbol ConnectCodeObject(IAstSymbol v, object codeObject)
        {
            var result = Find(v.Name, v.SymbolType);
            if (result == null)
            {
                return Add(v.Name, v.SymbolType, v, codeObject);
            }
            else
            {
                result.CodeObject = codeObject;
            }

            return result;
        }

        public bool Contains(string name, SymbolType type)
        {
            return Find(name, type) != null;
        }

        public ISymbol Find(IAstSymbol symbol)
        {
            return Find(symbol.Name, symbol.SymbolType);
        }

        public T FindCodeObject<T>(IAstSymbol symbol)
        {
            return (T) Find(symbol)?.CodeObject;
        }

        public ISymbol Find(string name, SymbolType type)
        {
            string prefix = PrefixFromType(type);

            if (_hashtable.Contains(prefix + name))
                return (Symbol) _hashtable[prefix + name];
            if (_parent != null)
            {
                return _parent.Find(name, type);
            }

            return null;
        }

        public void Clear()
        {
            _hashtable.Clear();
        }

        private string PrefixFromType(SymbolType type)
        {
            if (type == SymbolType.Function)
                return "f_";
            if (type == SymbolType.Type)
                return "t_";
            if (type == SymbolType.Variable)
                return "v_";
            return "";
        }
    }
}