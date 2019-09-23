

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Define a new column
    /// </summary>
    public interface ICreateTableWithColumnSyntax: IQueryBuilder
    {
        /// <summary>
        /// Define a new column
        /// </summary>
        /// <param name="name">The column name</param>
        /// <returns>Define the columns type</returns>
        ICreateTableColumnAsTypeSyntax WithColumn(string name);


    }
}
