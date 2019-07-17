using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        private const int BLOCK_SLOT = 0;

        /// <summary>
        /// Тело функции
        /// </summary>
        public BlockNode Block => _block;

        /// <summary>
        /// Тип функции
        /// </summary>
        public TypeNode Type { get; }

        /// <summary>
        /// Параметры функции
        /// </summary>
        public IReadOnlyList<ParameterNode> Parameters { get; }

        /// <summary>
        /// Аттрибуты функции
        /// </summary>
        public IReadOnlyList<AttributeNode> Attributes { get; }

        /// <summary>
        /// Флаги функции
        /// </summary>
        public FunctionFlags Flags => ((IsClient) ? FunctionFlags.Client : 0)
                                      | ((IsServer) ? FunctionFlags.Server : 0)
                                      | ((IsClientCall) ? FunctionFlags.ServerClientCall : 0);

        private bool IsServer => Attributes.Any(x => x.Type.TypeName == "Server") || !Attributes.Any();
        private bool IsClient => Attributes.Any(x => x.Type.TypeName == "Client");
        private bool IsClientCall => Attributes.Any(x => x.Type.TypeName == "ClientCall");

        /// <summary>
        /// Билдер IL кода
        /// </summary>
        public IEmitter Builder;

        private readonly BlockNode _block;

        public bool IsPublic { get; set; }

        /// <summary>
        /// Создать объект функции
        /// </summary>
        public Function(ILineInfo li, BlockNode block, ImmutableList<ParameterNode> parameters,
            string name, TypeNode type, ImmutableList<AttributeNode> attributes) : base(li)
        {
            _block = block;
            Parameters = parameters ?? ImmutableList<ParameterNode>.Empty;
            Attributes = attributes ?? ImmutableList<AttributeNode>.Empty;
            Name = name;
            Type = type;


            Children.SetSlot(block, BLOCK_SLOT);

            var slot = BLOCK_SLOT + 1;

            foreach (var parameter in Parameters)
            {
                Children.SetSlot(parameter, slot++);
            }

            foreach (var parameter in Attributes)
            {
                Children.SetSlot(parameter, slot++);
            }
        }

        public string Name { get; set; }

        public SymbolType SymbolType => SymbolType.Function;

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitFunction(this);
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

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitField(this);
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

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitProperty(this);
        }
    }
}