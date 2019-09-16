using System;
using System.Collections.Generic;

namespace ZenPlatform.Core.Authentication
{
    public interface IUser
    {
        Guid Id { get; }
        string Name { get; }
        List<RoleBase> Roles { get; }
    }
}