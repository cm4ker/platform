using System;
using Aquila.Core.Contracts;
using Aquila.Data;
using Aquila.Runtime;

namespace Aquila.Core
{
    public class PlatformContext
    {
        private readonly ISession _session;
        private readonly DataConnectionContext _dcc;
        private readonly DatabaseRuntimeContext _drc;

        public PlatformContext(ISession session)
        {
            _session = session;
            _dcc = _session.DataContext;
            _drc = new DatabaseRuntimeContext();
            _drc.Load(_dcc);
        }

        public DataConnectionContext DataContext => _dcc;

        public DatabaseRuntimeContext DataRuntimeContext => _drc;
    }
}