

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Interface for adding/altering columns or column options
    /// </summary>
    public interface IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax :
        IColumnOptionSyntax<IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax, IAlterTableColumnOptionOrAddColumnOrAlterColumnOrForeignKeyCascadeSyntax>,
        IAlterTableAddColumnOrAlterColumnSyntax
    {
        /// <summary>
        /// The value to set against existing rows for the new column.  Only used for creating columns, not altering them.
        /// </summary>
        IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax SetExistingRowsTo(object value);
    }
}
