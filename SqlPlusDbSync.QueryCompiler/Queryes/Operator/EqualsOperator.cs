namespace SqlPlusDbSync.QueryCompiler.Queryes.Operator
{
    public class EqualsOperator : BinaryOperator
    {
        public EqualsOperator(ISingleResultObject operand1, ISingleResultObject operand2) : base(operand1, operand2)
        {

        }

        public override string Compile()
        {
            return $"{Operand1.Compile()} = {Operand2.Compile()}";
        }
    }
}