

using System.Collections.Generic;

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Specify the data to insert
    /// </summary>
    public interface IInsertDataSyntax 
    {
        /// <summary>
        /// Specify the data to insert
        /// </summary>
        /// <param name="dataAsAnonymousType">An anonymous object that is used to insert data</param>
        /// <remarks>
        /// The properties are the column names and their values are the row values.
        /// </remarks>
        /// <returns>The next step</returns>
        IInsertDataSyntax Row(object dataAsAnonymousType);

        /// <summary>
        /// Specify the data to insert
        /// </summary>
        /// <param name="data">The dictionary containing column name/value combinations</param>
        /// <returns>The next step</returns>
        IInsertDataSyntax Row(IDictionary<string, object> data);
    }
}
