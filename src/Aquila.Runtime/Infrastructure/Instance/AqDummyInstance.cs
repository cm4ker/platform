using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Aquila.Core.Authentication;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Network;
using Aquila.Data;
using Aquila.Migrations;
using Aquila.Runtime;

namespace Aquila.Core.Instance;

public class AqDummyInstance : IAqInstance
{
    public DatabaseRuntimeContext DatabaseRuntimeContext { get; }
    public bool PendingChanges { get; }
    public IList<ISession> Sessions { get; }
    public IInvokeService InvokeService { get; }
    public MigrationManager MigrationManager { get; }
    public string Name { get; }
    public DataContextManager DataContextManager { get; }
    public Assembly BLAssembly { get; }
    public Dictionary<string, object> Globals { get; set; }

    public void Initialize(StartupConfig config)
    {
    }

    public void Migrate()
    {
    }

    public void Deploy(Stream packageStream)
    {
    }

    public void UpdateAssembly(Assembly asm)
    {
    }

    public ISession CreateSession(AqUser user)
    {
        throw new NotSupportedException();
    }
}