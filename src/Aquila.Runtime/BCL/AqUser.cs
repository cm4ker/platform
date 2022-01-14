using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;
using Aquila.Core.Contracts.Authentication;

namespace Aquila.Core.Authentication
{
    public class AqUser
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class AqSecPolicy
    {
        public string Name { get; set; }

        public List<string> Subjects { get; set; }

        public AqSecPermission Permission { get; set; }

        public AqSecPolicyCriteria Criteria { get; set; }
    }

    /// <summary>
    /// This criteria need for select certain objects and extract permissions only on this objects
    /// </summary>
    public class AqSecPolicyCriteria
    {
        public AqSecPermission Permission { get; set; }

        public List<string> Subjects { get; set; }

        public string Query { get; set; }
    }

    [Flags]
    public enum AqSecPermission
    {
        Insert,
        Read,
        Update,
        Delete,
    }
}