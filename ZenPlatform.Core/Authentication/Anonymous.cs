using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Core.Authentication
{
    public class Anonymous : IUser
    {
        public Anonymous()
        {
            Roles = new List<RoleBase>();
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }
        public string Name { get => "Anonymous"; }

        public List<RoleBase> Roles { get; }
    }
}
