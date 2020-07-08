namespace Aquila.Core.Contracts.Authentication
{
    public interface IAuthenticationToken
    {
        string Name { get; }

        object Credential { get; }
    }
}