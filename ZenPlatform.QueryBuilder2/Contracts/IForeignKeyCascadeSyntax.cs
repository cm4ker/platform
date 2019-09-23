using System.Data;

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Base interface for specifying the foreign key cascading
    /// </summary>
    /// <typeparam name="TNext">The interface for the next step after specifying the cascade rules for both DELETE and UPDATE</typeparam>
    /// <typeparam name="TNextFk">The interface for the next step after specifying the cascade rule for either DELETE or UPDATE</typeparam>
    public interface IForeignKeyCascadeSyntax<TNext,TNextFk>
    {
        /// <summary>
        /// Specify the behavior for DELETEs
        /// </summary>
        /// <param name="rule">The rule to apply for DELETEs</param>
        /// <returns>The next step</returns>
        TNextFk OnDelete(Rule rule);

        /// <summary>
        /// Specify the behavior for UPDATEs
        /// </summary>
        /// <param name="rule">The rule to apply for UPDATEs</param>
        /// <returns>The next step</returns>
        TNextFk OnUpdate(Rule rule);

        /// <summary>
        /// Specify the behavior for UPDATEs and DELETEs
        /// </summary>
        /// <param name="rule">The rule to apply for UPDATEs and DELETEs</param>
        /// <returns>The next step</returns>
        TNext OnDeleteOrUpdate(Rule rule);
    }
}
