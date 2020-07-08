namespace Aquila.Core.Contracts.Authentication
{
    public interface IAuthenticationProvider
    {
        IUser Authenticate(IAuthenticationToken token);

        bool CanAuthenticate(IAuthenticationToken token);
    }
}