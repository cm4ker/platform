namespace ZenPlatform.Core.Querying.Model
{
    public partial class QAliasedSelectExpression : QSelectExpression
    {
        public override string GetName()
        {
            return Alias;
        }

        public override string ToString()
        {
            return $"{Expression} AS {Alias}";
        }
    }
}