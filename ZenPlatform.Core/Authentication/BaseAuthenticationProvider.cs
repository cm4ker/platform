using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Core.Authentication
{
    public class BaseAuthenticationProvider : IAuthenticationProvider
    {
        private readonly IUserManager _userManager;
        public BaseAuthenticationProvider(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public IUser Authenticate(IAuthenticationToken token)
        {
            if (!CanAuthenticate(token)) throw new NotSupportedException("Type of token not supported.");

            if (_userManager.Authenticate(token.Name, (string)token.Credential))
            {
                return _userManager.FindUserByName(token.Name);
            }
            else throw new NotAuthorizedException();
        }

        public bool CanAuthenticate(IAuthenticationToken token)
        {
            return token is UserPasswordAuthenticationToken;
        }
    }
}
