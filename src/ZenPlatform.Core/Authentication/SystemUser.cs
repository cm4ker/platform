using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Core.Authentication
{
    public class SystemUser : IUser
    {
        public SystemUser()
        {
            Roles = new List<RoleBase>();
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }
        public string Name { get => "System"; }

        public List<RoleBase> Roles { get; }
    }
}
