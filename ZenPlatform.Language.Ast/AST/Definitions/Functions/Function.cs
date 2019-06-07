using System.Linq;
using ZenPlatform.Compiler;
using ZenPlatform.Compiler.AST.Definitions.Symbols;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Compiler.Visitor;
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
        public InstructionsBodyNode InstructionsBody;

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
        public Function(ILineInfo li, InstructionsBodyNode instructionsBody, ParameterCollection parameters,
            string name, TypeNode type, AttributeCollection ac) : base(li)
        {
            InstructionsBody = instructionsBody;
            Parameters = parameters ?? new ParameterCollection();
            Name = name;
            Type = type;
            Attributes = ac;
        }

        public string Name { get; set; }

        public SymbolType SymbolType => SymbolType.Function;

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(Type);

            if (Parameters != null)
                foreach (var parameter in Parameters)
                {
                    visitor.Visit(parameter);
                }

            visitor.Visit(InstructionsBody);
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

        public SymbolType SymbolType => SymbolType.Field;
    }
}