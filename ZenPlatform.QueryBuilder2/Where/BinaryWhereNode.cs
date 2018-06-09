namespace ZenPlatform.QueryBuilder2
{
    public class BinaryWhereNode : WhereExpression
    {
        public SqlNode Operand1 { get; }
        public SqlNode Operation { get; }
        public SqlNode Operand2 { get; }


        /// <summary>
        /// Используются RawSqlNode для всех типов значений, очень удобно для подставления имен полей и так далее
        /// </summary>
        /// <param name="operand1"></param>
        /// <param name="operation"></param>
        /// <param name="operand2"></param>
        public BinaryWhereNode(string operand1, string operation, string operand2)
        {
            Operand1 = new RawSqlNode(operand1);
            Operation = new RawSqlNode(operation);
            Operand2 = new RawSqlNode(operand2);
        }

        public BinaryWhereNode(SqlNode node1, string operation, SqlNode node2)
        {
            Operand1 = node1;
            Operand2 = node2;
            Operation = new RawSqlNode(operation);
        }
    }
}