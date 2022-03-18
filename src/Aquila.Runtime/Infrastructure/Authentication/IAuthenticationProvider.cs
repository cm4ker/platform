using Aquila.Core.Authentication;

namespace Aquila.Core.Contracts.Authentication
{
    public interface IAuthenticationProvider
    {
        AqUser Authenticate(IAuthenticationToken token);

        bool CanAuthenticate(IAuthenticationToken token);
    }
}