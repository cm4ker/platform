

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Define the primary table columns for a foreign key
    /// </summary>
    public interface ICreateForeignKeyPrimaryColumnSyntax
    {
        /// <summary>
        /// Define the primary table column for a foreign key
        /// </summary>
        /// <param name="column">The column name</param>
        /// <returns>Define the cascade rules</returns>
        ICreateForeignKeyCascadeSyntax PrimaryColumn(string column);

        /// <summary>
        /// Define the primary table columns for a foreign key
        /// </summary>
        /// <param name="columns">The column names</param>
        /// <returns>Define the cascade rules</returns>
        ICreateForeignKeyCascadeSyntax PrimaryColumns(params string[] columns);
    }
}
