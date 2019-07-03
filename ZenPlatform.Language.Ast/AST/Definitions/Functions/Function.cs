using System.Linq;
using ZenPlatform.Compiler;
using ZenPlatform.Compiler.AST.Definitions.Symbols;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Definitions.Statements;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions.Functions
{
    /// <summary>
    /// Describes a function.
    /// </summary>
    public class Function : Member, IAstSymbol
    {
        /// <summary>
        /// Тело функции
        /// </summary>
        public BlockNode Block;

        /// <summary>
        /// Тип функции
        /// </summary>
        public TypeNode Type;

        /// <summary>
        /// Параметры функции
        /// </summary>
        public ParameterCollection Parameters;

        /// <summary>
        /// Аттрибуты функции
        /// </summary>
        public AttributeCollection Attributes;

        public FunctionFlags Flags => ((IsClient) ? FunctionFlags.Client : 0)
                                      | ((IsServer) ? FunctionFlags.Server : 0)
                                      | ((IsClientCall) ? FunctionFlags.ServerClientCall : 0);

        private bool IsServer => Attributes.Any(x => x.Type.Type.Name == "Server") || !Attributes.Any();
        private bool IsClient => Attributes.Any(x => x.Type.Type.Name == "Client");
        private bool IsClientCall => Attributes.Any(x => x.Type.Type.Name == "ClientCall");

        /// <summary>
        /// Билдер IL кода
        /// </summary>
        public IEmitter Builder;

        public bool IsPublic { get; set; }

        /// <summary>
        /// Создать объект функции
        /// </summary>
        public Function(ILineInfo li, BlockNode block, ParameterCollection parameters,
            string name, TypeNode type, AttributeCollection ac) : base(li)
        {
            Block = block;
            Parameters = parameters ?? new ParameterCollection();
            Name = name;
            Type = type;
            Attributes = ac;
        }

        public string Name { get; set; }

        public SymbolType SymbolType => SymbolType.Function;

        public override void Accept<T>(IVisitor<T> visitor)
        {
            visitor.Visit(Type);

            if (Parameters != null)
                foreach (var parameter in Parameters)
                {
                    visitor.Visit(parameter);
                }

            visitor.Visit(Block);
        }
    }


    public class Field : Member, IAstSymbol
    {
        public Field(ILineInfo lineInfo, string name, TypeNode type) : base(lineInfo)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; set; }
        public TypeNode Type { get; set; }

        public SymbolType SymbolType => SymbolType.Variable;

        public override void Accept<T>(IVisitor<T> visitor)
        {
            visitor.Visit(Type);
        }
    }

    public class Property : Member, IAstSymbol
    {
        public Property(ILineInfo lineInfo, string name, TypeNode type, bool hasGet, bool hasSet) : base(lineInfo)
        {
            HasGetter = hasGet;
            HasSetter = hasSet;
            Type = type;
            Name = name;
        }

        public string Name { get; set; }

        public SymbolType SymbolType => SymbolType.Variable;

        public bool HasGetter { get; set; }
        public bool HasSetter { get; set; }


        public TypeNode Type { get; set; }

        public BlockNode Getter { get; set; }
        public BlockNode Setter { get; set; }

        public override void Accept<T>(IVisitor<T> visitor)
        {
            visitor.Visit(Type);

            if (Getter != null) visitor.Visit(Getter);
            if (Setter != null) visitor.Visit(Setter);
        }
    }
}