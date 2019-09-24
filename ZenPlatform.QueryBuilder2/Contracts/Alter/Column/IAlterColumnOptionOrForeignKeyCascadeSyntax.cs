

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Interface to change the column modification options or specifying the
    /// foreign key constraints
    /// </summary>
    public interface IAlterColumnOptionOrForeignKeyCascadeSyntax :
        IAlterColumnOptionSyntax,
        IForeignKeyCascadeSyntax<IAlterColumnOptionSyntax, IAlterColumnOptionOrForeignKeyCascadeSyntax>
    {
    }
}
