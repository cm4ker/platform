

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Interface to alter/add a column with an optional foreign key
    /// </summary>
    public interface IAlterTableColumnOptionOrAddColumnOrAlterColumnOrForeignKeyCascadeSyntax :
        IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax,
        IForeignKeyCascadeSyntax<IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax, IAlterTableColumnOptionOrAddColumnOrAlterColumnOrForeignKeyCascadeSyntax>
    {
    }
}
