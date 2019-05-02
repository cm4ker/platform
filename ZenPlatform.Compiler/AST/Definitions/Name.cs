namespace ZenPlatform.Compiler.AST.Definitions
{
    /// <summary>
    /// Именованоое поле (переменная). В последствии раскручивания дерева эта переменная запишется в таблицу символов
    /// </summary>
    public class Name : Infrastructure.Expression
    {
        public string Value;

        public Name(string value)
        {
            Value = value;
        }
    }

    public class FieldExpression : Infrastructure.Expression
    {
        public Infrastructure.Expression Expression { get; }
        public string Name { get; }

        public FieldExpression(Infrastructure.Expression expression, string name)
        {
            Expression = expression;
            Name = name;
            Line = expression.Line;
            Position = expression.Position;
        }
    }
}