using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Common;
using System.Reflection;
using System.Threading;
using Aquila.Core.Authentication;
using Aquila.Core.Contracts;
using Aquila.Core.Instance;
using Aquila.Data;
using Aquila.Metadata;
using Aquila.Runtime;

namespace Aquila.Core
{
    //Context. Not static. Immutable. Thread safe
    public partial class AqContext : IDisposable
    {
        private readonly AqInstance _instance;
        private readonly DataConnectionContext _dcc;
        private readonly DatabaseRuntimeContext _drc;

        public AqContext(AqInstance instance)
        {
            _instance = instance;
            _dcc = instance.DataContextManager.GetContext();
            _drc = instance.DatabaseRuntimeContext;
        }

        public AqInstance Instance => _instance;

        public DataConnectionContext DataContext => _dcc;

        public DatabaseRuntimeContext DataRuntimeContext => _drc;

        public DbCommand CreateCommand() => DataContext.CreateCommand();

        public virtual string User => "Anonymous";

        public virtual IEnumerable<string> Roles => ImmutableArray<string>.Empty;

        public virtual void Dispose()
        {
        }
    }
}