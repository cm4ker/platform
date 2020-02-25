using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;

namespace ZenPlatform.Compiler.Contracts.Symbols
{
    public class SymbolTable : ISymbolTable
    {
        /*
                        Symbol
              /       /    |     \         \ 
         Property Method Types Variables  Arguments
         
         GetTypeSymbol
         
         GetMethodSymbol
            Arguments
            Overloads
         GetPropertySymbol
         
         
         */

        private SymbolTable _parent;
        private Hashtable _hashtable = new Hashtable();

        public SymbolTable()
        {
        }

        public SymbolTable(SymbolTable parent) : this()
        {
            _parent = parent;
        }

        public ISymbol Add(IAstSymbol astSymbol, params ITypeSymbol[] args)
        {
            return Add(astSymbol, null, args);
        }

        public ISymbol Add(IAstSymbol astSymbol, object codeObject, params ITypeSymbol[] args)
        {
            return Add(astSymbol.Name, astSymbol.SymbolType, astSymbol.SymbolScope, astSymbol, codeObject, args);
        }

        public ISymbol Add(string name, SymbolType type, SymbolScopeBySecurity scope, IAstSymbol syntaxObject,
            object codeObject, params ITypeSymbol[] args)
        {
            string symbolName = PrefixFromType(type) + name;

            if (args != null && args.Length > 0)
            {
                symbolName += "(" + string.Join(',', args.Select(x => x.ToString())) + ")";
            }

            if (_hashtable.Contains(symbolName))
                throw new SymbolException("Symbol already exists in symbol table.");

            Symbol symbol = new Symbol(name, type, scope, syntaxObject, codeObject, args);


            _hashtable.Add(symbolName, symbol);

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
                result.CompileObject = codeObject;
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
            return (T) Find(symbol)?.CompileObject;
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
                SymbolType.Method => "m_",
                SymbolType.Type => "t_",
                SymbolType.Property => "p_",
                SymbolType.Variable => "v_",
                _ => ""
            };
        }
    }
}