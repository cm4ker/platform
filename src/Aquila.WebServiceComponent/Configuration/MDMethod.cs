using System;
using System.Collections.Generic;
using Aquila.Configuration.Common;

namespace Aquila.WebServiceComponent.Configuration
{
    public class MDMethod
    {
        public MDMethod()
        {
            Arguments = new List<MDArgument>();
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public MDType ResultType { get; set; }

        public List<MDArgument> Arguments { get; set; }
    }
}