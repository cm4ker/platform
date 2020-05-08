using System;
using System.Collections.Generic;
using System.Text;

namespace Aquila.Core.Authentication
{
    public interface IAuthenticationProvider
    {
        IUser Authenticate(IAuthenticationToken token);

        bool CanAuthenticate(IAuthenticationToken token);
    }
}