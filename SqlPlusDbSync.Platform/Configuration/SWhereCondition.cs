using QueryCompiler;

namespace SqlPlusDbSync.Platform.Configuration
{
    public class SWhereCondition
    {
        public SField Field { get; }
        public SOperand Operand { get; }
        public CompareType CompareType { get; }


        public SWhereCondition(SField field, SOperand operand, CompareType compareType)
        {
            Field = field;
            Operand = operand;
            CompareType = compareType;
        }


    }
}