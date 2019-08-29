using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Core.Authentication
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
