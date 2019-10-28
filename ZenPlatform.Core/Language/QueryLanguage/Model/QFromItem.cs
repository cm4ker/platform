namespace ZenPlatform.Core.Language.QueryLanguage.Model
{
    public class QFromItem
    {
        public QFromItem(QOn condition, IQDataSource joined, QJoinType joinType)
        {
            Condition = condition;
            Joined = joined;
            JoinType = joinType;
        }

        public QJoinType JoinType { get; }
        public IQDataSource Joined { get; }

        public QOn Condition { get; }
    }
}