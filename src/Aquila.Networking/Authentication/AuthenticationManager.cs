using System;
using System.Collections.Generic;
using System.Linq;
using Aquila.Core.Contracts.Authentication;

namespace Aquila.Core.Authentication
{
    public class AuthenticationManager : IAuthenticationManager
    {

        private readonly List<IAuthenticationProvider> _providers = new List<IAuthenticationProvider>();

 
        public void RegisterProvider(IAuthenticationProvider provider)
        {
            _providers.Add(provider);
        }

        public IUser Authenticate(IAuthenticationToken token)
        {
             var provider = _providers.First(p => p.CanAuthenticate(token));
             return provider.Authenticate(token);
        }
    }
}
