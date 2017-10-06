using System.Collections.Generic;
using SqlPlusDbSync.QueryCompiler.Queryes.Operator;

namespace SqlPlusDbSync.QueryCompiler.Queryes
{
    public class RelationalOptions
    {
        private List<IBooleanResultOperator> _operators;

        public RelationalOptions()
        {

        }

        public JoinType JoinType { get; set; }

        public IBooleanResultOperator Condition { get; set; }

        public string GetJoinType()
        {
            switch (JoinType)
            {
                case JoinType.Cross:
                    return "CROSS";
                case JoinType.Full:
                    return "FULL";
                case JoinType.Inner:
                    return "INNER";
                case JoinType.Left:
                    return "LEFT";
                case JoinType.Right:
                    return "RIGHT";
                default: return "";
            }
        }
    }

    public class Realtion
    {
        public ITable Table { get; set; }
        public RelationalOptions Options { get; set; }
    }
}