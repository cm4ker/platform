

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Interface to set the column options or the new value for existing rows
    /// </summary>
    public interface ICreateColumnOptionSyntax : IColumnOptionSyntax<ICreateColumnOptionSyntax,ICreateColumnOptionOrForeignKeyCascadeSyntax>
    {
        /// <summary>
        /// The value to set against existing rows for the new column.
        /// </summary>
        ICreateColumnOptionSyntax SetExistingRowsTo(object value);
    }
}
