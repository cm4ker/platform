using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
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

    public class AqSec
    {
        public string MdName { get; set; }

        public AqSecRight Right { get; set; }
    }

    [Flags]
    public enum AqSecRight
    {
        Insert,
        Read,
        Update,
        Delete,
    }

    public class AqSecSet : List<AqSecRight>
    {
        
    }
}