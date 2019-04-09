namespace PrototypePlatformLanguage.AST
{
    /// <summary>
    /// Describes a code body.
    /// </summary>
    public class MethodBody
    {
        public StructureCollection Structures = null;
        public StatementCollection Statements = null;

        //public Compilation.SymbolTable SymbolTable = null;

        /// <summary>
        /// Create a body from a collection of statements. 
        /// Functions, variables, structures and regular statements will be separated.
        /// </summary>
        public MethodBody(StatementCollection statements)
        {
            if (statements == null)
                return;

            Structures = new StructureCollection();
            Statements = new StatementCollection();

            foreach (Statement statement in statements)
            {
                if (statement.GetType() == typeof(Structure))
                    Structures.Add((Structure) statement);
                else
                    Statements.Add(statement);
            }
        }
    }

    /// <summary>
    /// Описывает тело типа (методы, поля, события, конструкторы и т.д.)
    /// </summary>
    public class TypeBody
    {
        public FunctionCollection Functions = null;


        public TypeBody(MemberCollection members)
        {
            if (members == null)
                return;

            Functions = new FunctionCollection();

            foreach (Member member in members)
            {
                if (member is Function func)
                    Functions.Add(func);
            }
        }
    }
}