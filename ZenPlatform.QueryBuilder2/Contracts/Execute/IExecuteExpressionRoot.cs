

using System;
using System.Collections.Generic;
using System.Data;


namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Executes some SQL
    /// </summary>
    public interface IExecuteExpressionRoot 
    {
        /// <summary>
        /// Executes an SQL statement
        /// </summary>
        /// <param name="sqlStatement">The SQL statement to execute</param>
        void Sql(string sqlStatement);

        /// <summary>
        /// Executes an SQL script loaded from the given file
        /// </summary>
        /// <param name="pathToSqlScript">The file to read the SQL script from</param>
        void Script(string pathToSqlScript);

        /// <summary>
        /// Executes an SQL script loaded from the given file
        /// </summary>
        /// <param name="pathToSqlScript">The file to read the SQL script from</param>
        /// <param name="parameters">The parameters to be replaced in the SQL script</param>
        void Script(string pathToSqlScript, IDictionary<string, string> parameters);

        /// <summary>
        /// Calls an action to execute dynamically generated SQL statements
        /// </summary>
        /// <param name="operation">The operation to execute on a given connection and transaction</param>
        void WithConnection(Action<IDbConnection, IDbTransaction> operation);

        /// <summary>
        /// Executes an SQL script loaded from an embedded ressource
        /// </summary>
        /// <param name="embeddedSqlScriptName">The name of the embedded SQL script (partial matches allowed)</param>
        void EmbeddedScript(string embeddedSqlScriptName);

        /// <summary>
        /// Executes an SQL script loaded from an embedded ressource
        /// </summary>
        /// <param name="embeddedSqlScriptName">The name of the embedded SQL script (partial matches allowed)</param>
        /// <param name="parameters">The parameters to be replaced in the SQL script</param>
        void EmbeddedScript(string embeddedSqlScriptName, IDictionary<string, string> parameters);
    }
}
