using System;
using System.Collections;

namespace ZenPlatform.Compiler.Contracts.Symbols
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

        public Symbol Add(IAstSymbol astSymbol, object codeObject = null)
        {
            return Add(astSymbol.Name, astSymbol, codeObject);
        }

        public Symbol Add(string name, IAstSymbol syntaxObject, object codeObject)
        {
            string prefix = PrefixFromType(syntaxObject);

            if (_hashtable.Contains(prefix + name))
                throw new Exception("Symbol already exists in symbol table.");

            Symbol symbol = new Symbol(name, syntaxObject, codeObject);
            _hashtable.Add(prefix + name, symbol);

            return symbol;
        }

        public Symbol ConnectCodeObject(IAstSymbol v, object codeObject)
        {
            var result = Find(v);
            if (result == null)
            {
                return Add(v, codeObject);
            }
            else
            {
                result.CodeObject = codeObject;
            }

            return result;
        }

        public bool Contains(IAstSymbol symbol)
        {
            return Find(symbol) != null;
        }

        public Symbol Find(IAstSymbol symbol)
        {
            string prefix = PrefixFromType(symbol);

            if (_hashtable.Contains(prefix + symbol.Name))
                return (Symbol) _hashtable[prefix + symbol.Name];
            if (_parent != null)
            {
                return _parent.Find(symbol);
            }

            return null;
        }

        public void Clear()
        {
            _hashtable.Clear();
        }

        private string PrefixFromType(IAstSymbol symbol)
        {
            TypeCode colorBand = 0;
            var a = colorBand switch
                {
                TypeCode.Boolean => new object(),
                };


            return symbol switch {
                ILocal l => "_v",
                IMethod m => "_m",
                IType t => "_t",
                _ => throw new Exception("This symbol not sypported")
                };
        }
    }
}