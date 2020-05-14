using System;
using System.Collections.Generic;
using System.Text;

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
