using System;
using System.Collections.Generic;
using System.Text;
using Aquila.Core.Contracts.Authentication;

namespace Aquila.Core.Authentication
{
    [Serializable]
    public class UserPasswordAuthenticationToken: IAuthenticationToken
    {
        public string Name { get; }
        public object Credential { get; }

        public UserPasswordAuthenticationToken(string name, string credential)
        {
            Name = name;
            Credential = credential;
        }
    }
}
