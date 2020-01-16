using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using ZenPlatform.Compiler.AST.Infrastructure;

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

    public class SymbolTable : ISymbolTable
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

        public ISymbol Add(IAstSymbol astSymbol)
        {
            return Add(astSymbol, null);
        }

        public ISymbol Add(IAstSymbol astSymbol, object codeObject)
        {
            return Add(astSymbol.Name, astSymbol.SymbolType, astSymbol.SymbolScope, astSymbol, codeObject);
        }

        public ISymbol Add(string name, SymbolType type, SymbolScopeBySecurity scope, IAstSymbol syntaxObject,
            object codeObject)
        {
            string prefix = PrefixFromType(type);

            if (_hashtable.Contains(prefix + name))
                throw new SymbolException("Symbol already exists in symbol table.");

            Symbol symbol = new Symbol(name, type, scope, syntaxObject, codeObject);
            _hashtable.Add(prefix + name, symbol);

            return symbol;
        }

        public ISymbol ConnectCodeObject(IAstSymbol v, object codeObject)
        {
            var result = Find(v.Name, v.SymbolType, v.SymbolScope);
            if (result == null)
            {
                return Add(v.Name, v.SymbolType, v.SymbolScope, v, codeObject);
            }
            else
            {
                result.CodeObject = codeObject;
            }

            return result;
        }

        public bool Contains(string name, SymbolType type, SymbolScopeBySecurity scope)
        {
            return Find(name, type, scope) != null;
        }

        public ISymbol Find(IAstSymbol symbol)
        {
            return Find(symbol.Name, symbol.SymbolType, symbol.SymbolScope);
        }

        public T FindCodeObject<T>(IAstSymbol symbol)
        {
            return (T) Find(symbol)?.CodeObject;
        }

        public ISymbol Find(string name, SymbolType type, SymbolScopeBySecurity scope)
        {
            foreach (SymbolType val in Enum.GetValues(typeof(SymbolType)))
            {
                if (!type.HasFlag(val)) continue;

                ISymbol result = null;

                string prefix = PrefixFromType(val);

                if (_hashtable.Contains(prefix + name))
                {
                    return (Symbol) _hashtable[prefix + name];
                }

                if (_parent != null)
                {
                    result = _parent.Find(name, type, scope);
                }

                if (result != null)
                    return result;
            }

            return null;
        }


        public IEnumerable<ISymbol> GetAll(SymbolType type)
        {
            var prefix = PrefixFromType(type);
            return _hashtable.Cast<DictionaryEntry>().Where(x => ((string) x.Key).StartsWith(prefix))
                .Select(x => (Symbol) x.Value);
        }


        public void Clear()
        {
            _hashtable.Clear();
        }

        private string PrefixFromType(SymbolType type)
        {
            return type switch
            {
                SymbolType.Method => "f_",
                SymbolType.Type => "t_",
                SymbolType.Property => "p_",
                SymbolType.Variable => "v_",
                _ => ""
            };
        }
    }
}