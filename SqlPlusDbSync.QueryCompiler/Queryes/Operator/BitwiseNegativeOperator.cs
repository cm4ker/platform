namespace SqlPlusDbSync.QueryCompiler.Queryes.Operator
{
    public class BitwiseNegativeOperator : UnaryOperator
    {
        public BitwiseNegativeOperator(ISingleResultObject operand) : base(operand)
        {
        }

        public override string Compile()
        {
            return "~" + Operand.Compile();
        }
    }
}