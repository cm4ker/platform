namespace SqlPlusDbSync.QueryCompiler.Queryes.Operator
{
    public class OrOperator : BinaryOperator
    {
        public OrOperator(ISingleResultObject operand1, ISingleResultObject operand2) : base(operand1, operand2)
        {
        }

        public override string Compile()
        {
            return $"({Operand1.Compile()} OR {Operand2.Compile()})";
        }
    }
}