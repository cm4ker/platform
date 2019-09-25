

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Set column options or create a new column or set a foreign key cascade rule
    /// </summary>
    public interface ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax :
        ICreateTableColumnOptionOrWithColumnSyntax,
        IForeignKeyCascadeSyntax<ICreateTableColumnOptionOrWithColumnSyntax,ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax>
    {
    }
}
