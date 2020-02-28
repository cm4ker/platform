using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Functions;

namespace ZenPlatform.Language.Ast.Symbols
{
    public static class SymbolTableExtensions
    {
        public static Symbol FindOrDeclareVariable(this SymbolTable st, IAstSymbol syntaxObject, object compileObject)
        {
            var symbol = st.Find<VariableSymbol>(syntaxObject);

            if (symbol == null)
                symbol = st.AddVariable(syntaxObject);

            symbol.Connect(compileObject);

            return symbol;
        }

        public static MethodSymbol AddMethod(this SymbolTable st, Function syntaxObject, IMethod compileObject)
        {
            var symbol = st.Find<MethodSymbol>(syntaxObject);

            if (symbol == null)
                symbol = st.AddMethod(syntaxObject);

            symbol.ConnectOverload(syntaxObject, compileObject);

            return symbol;
        }
    }

    public class SymbolTable
    {
        private SymbolTable _parent;
        private Hashtable _hashtable = new Hashtable();

        public SymbolTable()
        {
        }

        public SymbolTable(SymbolTable parent) : this()
        {
            _parent = parent;
        }

        public TypeSymbol AddType(TypeEntity te)
        {
            var typeSymbol = new TypeSymbol(te);

            RegisterSymbol(typeSymbol);

            return typeSymbol;
        }

        public VariableSymbol AddVariable(IAstSymbol p)
        {
            var variableSymbol = new VariableSymbol(p);

            RegisterSymbol(variableSymbol);

            return variableSymbol;
        }

        public MethodSymbol AddMethod(Function m)
        {
            var methodSymbol = Find<MethodSymbol>(m);
            if (methodSymbol == null)
            {
                methodSymbol = new MethodSymbol(m);
                RegisterSymbol(methodSymbol);
            }

            return methodSymbol;
        }

        public ConstructorSymbol AddConstructor(Constructor m)
        {
            var methodSymbol = new ConstructorSymbol(m);

            RegisterSymbol(methodSymbol);

            return methodSymbol;
        }

        public PropertySymbol AddProperty(Property p)
        {
            var symbol = new PropertySymbol(p);

            RegisterSymbol(symbol);

            return symbol;
        }

        private void RegisterSymbol(Symbol symbol)
        {
            string symbolName = PrefixFromType(symbol.Type) + symbol.Name;

            if (_hashtable.Contains(symbolName))
                throw new SymbolException("Symbol already exists in symbol table.");

            _hashtable.Add(symbolName, symbol);
        }

        public bool Contains(string name, SymbolType type, SymbolScopeBySecurity scope)
        {
            return Find(name, type, scope) != null;
        }

        public T Find<T>(IAstSymbol symbol) where T : Symbol
        {
            return (T) Find(symbol.Name, symbol.SymbolType, symbol.SymbolScope);
        }

        public T Find<T>(string name, SymbolScopeBySecurity scope)
            where T : Symbol
        {
            if (typeof(T) == typeof(TypeSymbol))
                return (T) Find(name, SymbolType.Type, scope);
            if (typeof(T) == typeof(VariableSymbol))
                return (T) Find(name, SymbolType.Variable, scope);
            if (typeof(T) == typeof(ConstructorSymbol))
                return (T) Find(name, SymbolType.Constructor, scope);
            if (typeof(T) == typeof(MethodSymbol))
                return (T) Find(name, SymbolType.Method, scope);
            if (typeof(T) == typeof(PropertySymbol))
                return (T) Find(name, SymbolType.Property, scope);

            throw new Exception($"Unknown symbol type {typeof(T)}");
        }

        public Symbol Find(string name, SymbolType type, SymbolScopeBySecurity scope)
        {
            foreach (SymbolType val in Enum.GetValues(typeof(SymbolType)))
            {
                if (!type.HasFlag(val)) continue;

                Symbol result = null;

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

        public IEnumerable<T> GetAll<T>(SymbolType type) where T : Symbol
        {
            var prefix = PrefixFromType(type);
            return _hashtable.Cast<DictionaryEntry>().Where(x => ((string) x.Key).StartsWith(prefix))
                .Select(x => (T) x.Value);
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