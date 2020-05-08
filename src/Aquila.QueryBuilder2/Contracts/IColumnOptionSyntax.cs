

namespace Aquila.QueryBuilder.Contracts
{
    /// <summary>
    /// Specify column options
    /// </summary>
    /// <typeparam name="TNext">The interface of the next step</typeparam>
    /// <typeparam name="TNextFk">The interface of the next step after a foreign key definition</typeparam>
    public interface IColumnOptionSyntax<TNext> 

    {
        /// <summary>
        /// Sets the default function for the column
        /// </summary>
        /// <param name="method">The function providing the default value</param>
        /// <returns>The next step</returns>
        TNext WithDefault(SystemMethods method);

        /// <summary>
        /// Sets the default value for the column
        /// </summary>
        /// <param name="value">The default value</param>
        /// <returns>The next step</returns>
        TNext WithDefaultValue(object value);

        /// <summary>
        /// Sets the columns description
        /// </summary>
        /// <param name="description">The description</param>
        /// <returns>The next step</returns>
        TNext WithColumnDescription(string description);

        /// <summary>
        /// Sets the columns identity configuration
        /// </summary>
        /// <returns>The next step</returns>
        TNext Identity();

        /// <summary>
        /// Create an index for this column
        /// </summary>
        /// <returns>The next step</returns>
        TNext Indexed();

        /// <summary>
        /// Create an index for this column
        /// </summary>
        /// <param name="indexName">The index name</param>
        /// <returns>The next step</returns>
        TNext Indexed(string indexName);

        /// <summary>
        /// Define the column as primary key
        /// </summary>
        /// <returns>The next step</returns>
        TNext PrimaryKey();

        /// <summary>
        /// Define the column as primary key
        /// </summary>
        /// <param name="primaryKeyName">The primary key constraint name</param>
        /// <returns>The next step</returns>
        TNext PrimaryKey(string primaryKeyName);

        /// <summary>
        /// Specify the column as nullable
        /// </summary>
        /// <returns>The next step</returns>
        TNext Nullable();

        /// <summary>
        /// Specify the column as not-nullable
        /// </summary>
        /// <returns>The next step</returns>
        TNext NotNullable();

        /// <summary>
        /// Specify a unique index for the column
        /// </summary>
        /// <returns>The next step</returns>
        TNext Unique();

        /// <summary>
        /// Specify a unique index for the column
        /// </summary>
        /// <param name="indexName">The index name</param>
        /// <returns>The next step</returns>
        TNext Unique(string indexName);

        /// <summary>
        /// Specifies a foreign key
        /// </summary>
        /// <param name="primaryTableName">The primary table name</param>
        /// <param name="primaryColumnName">The primary tables column name</param>
        /// <returns>The next step</returns>
        TNext ForeignKey(string primaryTableName, string primaryColumnName);

        /// <summary>
        /// Specifies a foreign key
        /// </summary>
        /// <param name="foreignKeyName">The foreign key name</param>
        /// <param name="primaryTableName">The primary table name</param>
        /// <param name="primaryColumnName">The primary tables column name</param>
        /// <returns>The next step</returns>
        TNext ForeignKey(string foreignKeyName, string primaryTableName, string primaryColumnName);

        /// <summary>
        /// Specifies a foreign key
        /// </summary>
        /// <param name="foreignKeyName">The foreign key name</param>
        /// <param name="primaryTableSchema">The primary tables schema name</param>
        /// <param name="primaryTableName">The primary table name</param>
        /// <param name="primaryColumnName">The primary tables column name</param>
        /// <returns>The next step</returns>
        TNext ForeignKey(string foreignKeyName, string primaryTableSchema, string primaryTableName, string primaryColumnName);
    }
}
