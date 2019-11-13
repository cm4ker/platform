namespace ZenPlatform.Core.Querying.Model
{
    public partial class QFromItem
    {
        public QFromItem(QOn condition, QDataSource joined, QJoinType joinType)
        {
            Condition = condition;
            Joined = joined;
            JoinType = joinType;
        }

        public QJoinType JoinType { get; }
        public QDataSource Joined { get; }

        public QOn Condition { get; }
    }
}