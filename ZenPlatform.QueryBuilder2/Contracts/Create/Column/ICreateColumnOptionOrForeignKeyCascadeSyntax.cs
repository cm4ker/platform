

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// The interface to specify the column options or the foreign key
    /// </summary>
    public interface ICreateColumnOptionOrForeignKeyCascadeSyntax :
        ICreateColumnOptionSyntax,
        IForeignKeyCascadeSyntax<ICreateColumnOptionSyntax,ICreateColumnOptionOrForeignKeyCascadeSyntax>
    {
    }
}
