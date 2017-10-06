namespace SqlPlusDbSync.QueryCompiler.Queryes.Operator
{
    public class NotOperator : UnaryOperator
    {
        public NotOperator(ISingleResultObject operand) : base(operand)
        {
        }

        public override string Compile()
        {
            return $"NOT ({Operand.Compile()})";
        }
    }
}