using System;
using System.Data.Common;
using System.Threading;
using Aquila.Core.Contracts;
using Aquila.Data;
using Aquila.Runtime;

namespace Aquila.Core
{
    //Context. Not static. Immutable. Thread safe
    public class AqContext
    {
        private readonly ISession _session;
        private readonly DataConnectionContext _dcc;
        private readonly DatabaseRuntimeContext _drc;

        public AqContext(ISession session)
        {
            _session = session;
            _dcc = _session.DataContext;
            _drc = new DatabaseRuntimeContext();
            _drc.Load(_dcc);
        }

        public DataConnectionContext DataContext => _dcc;

        public DatabaseRuntimeContext DataRuntimeContext => _drc;

        public DbCommand CreateCommand() => DataContext.CreateCommand();
    }
}