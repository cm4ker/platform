using System.Collections;
using ZenPlatform.Compiler.AST.Definitions.Functions;
using ZenPlatform.Compiler.AST.Infrastructure;

namespace ZenPlatform.Compiler.AST.Definitions.Symbols
{
    public class SymbolTable
    {
        private SymbolTable _parent;
        private Hashtable _hashtable = new Hashtable();

        public SymbolTable()
        {
        }

        public SymbolTable(SymbolTable parent)
        {
            _parent = parent;
        }

        public Symbol Add(IAstSymbol astSymbol)
        {
            return Add(astSymbol.Name, astSymbol.SymbolType, astSymbol, null);
        }

        public Symbol Add(string name, SymbolType type, IAstSymbol syntaxObject, object codeObject)
        {
            string prefix = PrefixFromType(type);

            if (_hashtable.Contains(prefix + name))
                throw new SymbolException("Symbol already exists in symbol table.");

            Symbol symbol = new Symbol(name, type, syntaxObject, codeObject);
            _hashtable.Add(prefix + name, symbol);

            return symbol;
        }

        public Symbol ConnectCodeObject(IAstSymbol v, object codeObject)
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

        public Symbol Find(string name, SymbolType type)
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
            if (type == SymbolType.Structure)
                return "s_";
            if (type == SymbolType.Variable)
                return "v_";
            return "";
        }
    }
}