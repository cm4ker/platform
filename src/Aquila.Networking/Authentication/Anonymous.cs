using System;
using System.Collections.Generic;
using System.Text;
using Aquila.Core.Contracts.Authentication;

namespace Aquila.Core.Authentication
{
    public class Anonymous : IUser
    {
        public Anonymous()
        {
            Roles = new List<IRole>();
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }

        public string Name
        {
            get => "Anonymous";
        }

        public List<IRole> Roles { get; }
    }
}