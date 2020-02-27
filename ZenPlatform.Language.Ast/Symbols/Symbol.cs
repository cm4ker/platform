using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Functions;

namespace ZenPlatform.Language.Ast.Symbols
{
    /// <summary>
    /// Тип пространства, где доступен данный символ
    /// </summary>
    public enum SymbolScopeBySecurity
    {
        /// <summary>
        /// Оыбласть видимости не известна
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Пользовательское
        /// </summary>
        User = 1,

        /// <summary>
        /// Системное
        /// </summary>
        System = 2 >> 1,

        /// <summary>
        /// Символ виден и для системного и для пользовательского использования
        /// </summary>
        Shared = User | System,
    }

    public class VariableSymbol : Symbol
    {
        public VariableSymbol(IAstSymbol v) : base(v.Name, SymbolType.Variable)
        {
            SyntaxObject = v;
        }

        public object CompileObject { get; private set; }

        public IAstSymbol SyntaxObject { get; }

        public void Connect(object compileObject)
        {
            CompileObject = compileObject;
        }
    }

    public class MethodSymbol : Symbol
    {
        private IType declaringType;

        private readonly Dictionary<Function, IMethod> _overloads;

        public MethodSymbol(Function f) : base(f.Name, SymbolType.Method)
        {
            _overloads = new Dictionary<Function, IMethod>();

            DeclareOverload(f);
        }

        public void DeclareOverload(Function f)
        {
            if (f.Name != Name)
            {
                throw new Exception("this is not overload of this function");
            }

            if (_overloads.ContainsKey(f))
                throw new Exception("You already have this overload");

            _overloads.Add(f, null);
        }

        public void ConnectOverload(Function f, IMethod method)
        {
            if (!_overloads.ContainsKey(f))
            {
                DeclareOverload(f);
            }

            declaringType ??= method.DeclaringType;

            _overloads[f] = method;
        }

        public (Function method, IMethod clrMethod) SelectOverload(IType[] args)
        {
            if (_overloads.Count == 0)
                throw new Exception(
                    "This method is empty. Please before select overloads add some (Use method DeclareOverload or ConnectOverload)");
            if (_overloads.Count == 1)
                return (_overloads.First().Key, _overloads.First().Value);


            if (declaringType != null)
            {
                var list = _overloads.Select(x => (x.Key, x.Value));
                foreach (var candidate in list)
                {
                    var method = candidate.Value;
                    var resolved = declaringType.FindMethod(method.Name, args);

                    if (method == resolved)
                        return candidate;
                }
            }

            throw new Exception("Overload not found");
        }
    }

    public class ConstructorSymbol : Symbol
    {
        private readonly Dictionary<Constructor, IConstructor> _overloads;

        public ConstructorSymbol(Constructor c) : base(c.Name, SymbolType.Constructor)
        {
            _overloads = new Dictionary<Constructor, IConstructor>();

            DeclareOverload(c);
        }


        public void DeclareOverload(Constructor f)
        {
            if (f.Name != Name)
            {
                throw new Exception("this is not overload of this function");
            }

            if (_overloads.ContainsKey(f))
                throw new Exception("You already have this overload");

            _overloads.Add(f, null);
        }

        public void ConnectOverload(Constructor f, IConstructor method)
        {
            if (!_overloads.ContainsKey(f))
            {
                throw new Exception(
                    $"this symbol m_{Name} not contains this overload. You must firstly invoke DeclareOverload for this function");
            }

            _overloads[f] = method;
        }

        public void SelectOverload(object[] args)
        {
            //select overload algorithm
        }
    }

    public class TypeSymbol : Symbol
    {
        public TypeSymbol(TypeEntity type) : base(type.Name, SymbolType.Type)
        {
            Type = type;
        }

        public IType ClrType { get; private set; }

        public TypeEntity Type { get; }

        public void Connect(IType type)
        {
            ClrType = type;
        }
    }

    public class PropertySymbol : Symbol
    {
        public PropertySymbol(Property p) : base(p.Name, SymbolType.Property)
        {
            Property = p;
        }

        public Property Property { get; }

        public IProperty ClrProperty { get; private set; }

        public void Connect(IProperty prop)
        {
            ClrProperty = prop;
        }
    }

    /// <summary>
    /// Символ. Объект на который можно сослаться в коде
    /// </summary>
    public abstract class Symbol
    {
        public string Name { get; }

        public virtual SymbolType Type { get; }

        /// <summary>
        ///  Конструктор символа
        /// </summary>
        /// <param name="name">Symbel name</param>
        /// <param name="type">Symbol kind type</param>
        /// <param name="syntaxObject"><see cref="AstNode"/> object</param>
        /// <param name="compileObject">The connected object from the codegeneration backend</param>
        public Symbol(string name, SymbolType type)
        {
            Name = name;
            Type = type;
        }
    }
}