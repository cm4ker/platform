

using System.Data;

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Defines the cascading rules of a foreign key constraint
    /// </summary>
    public interface ICreateForeignKeyCascadeSyntax 
    {
        /// <summary>
        /// Defines the DELETE rule
        /// </summary>
        /// <param name="rule">The rule to apply to DELETE operations</param>
        /// <returns>Specify other rules</returns>
        ICreateForeignKeyCascadeSyntax OnDelete(Rule rule);

        /// <summary>
        /// Defines the UPDATE rule
        /// </summary>
        /// <param name="rule">The rule to apply to UPDATE operations</param>
        /// <returns>Specify other rules</returns>
        ICreateForeignKeyCascadeSyntax OnUpdate(Rule rule);

        /// <summary>
        /// Defines the UPDATE and DELETE rule
        /// </summary>
        /// <param name="rule">The rule to apply to UPDATE and DELETE operations</param>
        void OnDeleteOrUpdate(Rule rule);
    }
}
