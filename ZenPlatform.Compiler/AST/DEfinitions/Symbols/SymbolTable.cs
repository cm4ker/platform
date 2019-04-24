using System.Collections;
using ZenPlatform.Compiler.AST.Definitions.Functions;
using ZenPlatform.Compiler.AST.Infrastructure;

namespace ZenPlatform.Compiler.AST.Definitions.Symbols
{
    public class SymbolTable
    {
        private SymbolTable _parent = null;
        private Hashtable _hashtable = new Hashtable();

        public SymbolTable()
        {
        }

        public SymbolTable(SymbolTable parent)
        {
            _parent = parent;
        }

        public Symbol Add(Variable variable)
        {
            return Add(variable.Name, SymbolType.Variable, variable, null);
        }

        public Symbol Add(Function function)
        {
            return Add(function.Name, SymbolType.Function, function, null);
        }

        public Symbol Add(Structure structure)
        {
            return Add(structure.Name, SymbolType.Structure, structure, null);
        }

        public Symbol Add(string name, SymbolType type, object syntaxObject, object codeObject)
        {
            string prefix = PrefixFromType(type);

            if (_hashtable.Contains(prefix + name))
                throw new SymbolException("Symbol already exists in symbol table.");

            Symbol symbol = new Symbol(name, type, syntaxObject, codeObject);
            _hashtable.Add(prefix + name, symbol);

            return symbol;
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
            else if (_parent != null)
            {
                return _parent.Find(name, type);
            }
            else
                return null;
        }
        
        

        private string PrefixFromType(SymbolType type)
        {
            if (type == SymbolType.Function)
                return "f_";
            else if (type == SymbolType.Structure)
                return "s_";
            else if (type == SymbolType.Variable)
                return "v_";
            return "";
        }
    }
}