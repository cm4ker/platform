using System;
using System.Collections.Generic;
using System.Text;
using Aquila.Core.Contracts.Authentication;
using Microsoft.Extensions.Logging;

namespace Aquila.Core.Authentication
{
    public class BaseAuthenticationProvider : IAuthenticationProvider
    {
        private readonly UserManager _userManager;
        private readonly ILogger<BaseAuthenticationProvider> _logger;

        public BaseAuthenticationProvider(UserManager userManager)
        {
            _userManager = userManager;
        }

        public AqUser Authenticate(IAuthenticationToken token)
        {
            if (!CanAuthenticate(token)) throw new NotSupportedException("Type of token not supported.");

            if (_userManager.Authenticate(token.Name, (string) token.Credential))
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