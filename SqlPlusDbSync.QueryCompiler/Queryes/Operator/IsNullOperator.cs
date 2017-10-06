namespace SqlPlusDbSync.QueryCompiler.Queryes.Operator
{
    public class IsNullOperator : UnaryOperator
    {
        public IsNullOperator(ISingleResultObject operand) : base(operand)
        {
        }

        public override string Compile()
        {
            return $"{Operand.Compile()} IS NULL";
        }
    }
}