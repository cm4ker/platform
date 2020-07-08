using System;
using Aquila.Core.Contracts;
using Aquila.Data;

namespace Aquila.Core
{
    public class PlatformContext
    {
        public PlatformContext(ISession session)
        {
        }

        public DataConnectionContext DataContext { get; set; }
    }
}