using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading;
using Aquila.Core.Authentication;
using Aquila.Core.Contracts;
using Aquila.Core.Instance;
using Aquila.Data;
using Aquila.Metadata;
using Aquila.Runtime;
using Aquila.Runtime.Querying;

namespace Aquila.Core
{
    //Context. Not static. Immutable. Thread safe
    public partial class AqContext : IDisposable
    {
        private readonly AqInstance _instance;
        private readonly DataConnectionContext _dcc;
        private readonly DatabaseRuntimeContext _drc;
        private ContextSecTable _usc;

        public AqContext(AqInstance instance)
        {
            _instance = instance;
            _dcc = instance.DataContextManager.GetContext();
            _drc = instance.DatabaseRuntimeContext;
        }

        public AqInstance Instance => _instance;

        public DataConnectionContext DataContext => _dcc;

        public DatabaseRuntimeContext DataRuntimeContext => _drc;

        public MetadataProvider MetadataProvider => _drc.Metadata.GetMetadata();

        public ContextSecTable SecTable
        {
            get
            {
                if (_usc == null)
                {
                    _usc = new ContextSecTable();
                    var sec = _drc.Metadata.GetMetadata().GetSecPoliciesFromRoles(Roles);
                    _usc.Init(sec);
                }

                return _usc;
            }
        }

        public DbCommand CreateCommand() => DataContext.CreateCommand();

        public virtual string User { get; init; } = "Anonymous";

        public virtual IEnumerable<string> Roles { get; init; } = ImmutableArray<string>.Empty;

        public virtual void Dispose()
        {
        }
    }
}