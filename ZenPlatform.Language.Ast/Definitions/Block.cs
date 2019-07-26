using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST;
using ZenPlatform.Language.Ast.Definitions.Functions;
using ZenPlatform.Language.Ast.Infrastructure;

namespace ZenPlatform.Language.Ast.Definitions
{
    /// <summary>
    /// Блок инструкций
    /// </summary>
    public partial class Block : IScoped
    {
        /// <summary>
        /// Создать блок из коллекции инструкций
        /// </summary>
        public Block(List<Statement> statements) : this(null, statements)
        {
            if (statements == null)
                return;

            Statements = statements;
        }
    }

    /// <summary>
    /// Описывает тело типа (методы, поля, события, конструкторы и т.д.)
    /// </summary>
    public partial class TypeBody
    {
        public TypeBody(ImmutableList<Member> members) : this(null,
            members.Where(x => x is Function).Cast<Function>().ToList(),
            members.Where(x => x is Field).Cast<Field>().ToList(),
            members.Where(x => x is Property).Cast<Property>().ToList())
        {
        }
    }
}