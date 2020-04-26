using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Core.Authentication
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
