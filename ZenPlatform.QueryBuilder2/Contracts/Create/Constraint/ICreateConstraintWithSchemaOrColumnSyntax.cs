

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Specify the constraint table schmea or the columns
    /// </summary>
    public interface ICreateConstraintWithSchemaOrColumnSyntax : ICreateConstraintColumnsSyntax,
        ICreateConstraintWithSchemaSyntax
    {
    }
}
