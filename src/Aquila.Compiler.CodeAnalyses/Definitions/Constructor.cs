// using Aquila.Compiler.Roslyn;
// using Aquila.Language.Ast.Misc;
//
// namespace Aquila.Language.Ast.Definitions
// {
//     public partial class Constructor
//     {
//         /// <summary>
//         /// Билдер IL кода
//         /// </summary>
//         public RBlockBuilder Builder;
//
//         public SymbolType SymbolType => SymbolType.Constructor;
//
//         public SymbolScopeBySecurity SymbolScope { get; set; }
//
//         public static Constructor Default => new Constructor(null, new Block(new StatementList()),
//             new ParameterList(), new AttributeList(), null);
//     }
// }