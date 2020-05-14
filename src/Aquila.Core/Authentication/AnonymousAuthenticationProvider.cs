using System;
using System.Collections.Generic;
using System.Text;
using Aquila.Core.Contracts.Authentication;

namespace Aquila.Core.Authentication
{
    public class AnonymousAuthenticationProvider
        : IAuthenticationProvider
    {
        public IUser Authenticate(IAuthenticationToken token)
        {
            return new Anonymous();
        }

        public bool CanAuthenticate(IAuthenticationToken token)
        {
            return true;
        }
    }
}
