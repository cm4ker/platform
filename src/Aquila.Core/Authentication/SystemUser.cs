﻿using System;
using System.Collections.Generic;
using System.Text;
using Aquila.Core.Contracts.Authentication;

namespace Aquila.Core.Authentication
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
