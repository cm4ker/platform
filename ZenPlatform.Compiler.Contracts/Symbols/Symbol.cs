namespace ZenPlatform.Compiler.Contracts.Symbols
{
    /// <summary>
    /// Тип пространства, где доступен данный символ
    /// </summary>
    public enum SymbolScope
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

    public interface ISymbol
    {
        /// <summary>
        /// Имя символа
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Тип символа
        /// </summary>
        SymbolType Type { get; set; }

        /// <summary>
        /// связанный с символом синтаксический объект
        /// </summary>
        IAstSymbol SyntaxObject { get; set; }


        /// <summary>
        /// Связанный с символом объект кода
        /// </summary>
        object CodeObject { get; set; }

        /// <summary>
        ///  Область видимости символа
        /// </summary>
        SymbolScope SymbolScope { get; set; }
    }

    /// <summary>
    /// Символ. Объект на который можно сослаться в коде
    /// </summary>
    public class Symbol : ISymbol
    {
        public string Name { get; set; }


        public SymbolType Type { get; set; }


        public IAstSymbol SyntaxObject { get; set; }


        public object CodeObject { get; set; }


        public SymbolScope SymbolScope { get; set; }

        /// <summary>
        ///  Конструктор символа
        /// </summary>
        /// <param name="name">Symbel name</param>
        /// <param name="type">Symbol kind type</param>
        /// <param name="syntaxObject"><see cref="AstNode"/> object</param>
        /// <param name="codeObject">The connected object from the codegeneration backend</param>
        public Symbol(string name, SymbolType type, SymbolScope scope, IAstSymbol syntaxObject, object codeObject)
        {
            Name = name;
            Type = type;
            SyntaxObject = syntaxObject;
            CodeObject = codeObject;
            SymbolScope = scope;
        }
    }
}