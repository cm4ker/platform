

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Set column options or define a new column
    /// </summary>
    public interface ICreateTableColumnOptionOrWithColumnSyntax :
        IColumnOptionSyntax<ICreateTableColumnOptionOrWithColumnSyntax,ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax>,
        ICreateTableWithColumnSyntax
    {
    }
}
