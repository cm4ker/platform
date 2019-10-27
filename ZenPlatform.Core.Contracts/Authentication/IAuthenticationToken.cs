using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Core.Authentication
{
    public interface IAuthenticationToken
    {
        string Name { get; }

        object Credential { get; }
    }
}