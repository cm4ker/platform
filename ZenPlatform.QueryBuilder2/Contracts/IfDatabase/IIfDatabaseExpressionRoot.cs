
using System;



namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Defines fluent expressions that can be conditionally executed
    /// </summary>
    public interface IIfDatabaseExpressionRoot
    {
        /// <summary>
        /// Creates an ALTER expression
        /// </summary>
        IAlterExpressionRoot Alter { get; }

        /// <summary>
        /// Creates CREATE expression
        /// </summary>
        ICreateExpressionRoot Create { get; }

        /// <summary>
        /// Creates a DELETE expression
        /// </summary>
        IDeleteExpressionRoot Delete { get; }

        /// <summary>
        /// Renames a database object
        /// </summary>
        IRenameExpressionRoot Rename { get; }

        /// <summary>
        /// Inserts data into a table
        /// </summary>
        IInsertExpressionRoot Insert { get; }

        /// <summary>
        /// Execute some SQL
        /// </summary>
        IExecuteExpressionRoot Execute { get; }

        /// <summary>
        /// Check if a database object exists
        /// </summary>
        ISchemaExpressionRoot Schema { get; }

        /// <summary>
        /// Updates data in a table
        /// </summary>
        IUpdateExpressionRoot Update { get; }

        /// <summary>
        /// Invokes a delegate that can be used to create database specific expressions
        /// </summary>
        /// <param name="delegation">The delegate to call if the database type matches</param>
        void Delegate(Action delegation);
    }
}
