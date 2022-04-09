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

public interface IAqInstance
{
    DatabaseRuntimeContext DatabaseRuntimeContext { get; }
    bool PendingChanges { get; }
    MigrationManager MigrationManager { get; }
    string Name { get; }
    DataContextManager DataContextManager { get; }
    Assembly BLAssembly { get; }

    /// <summary>
    /// Global objects
    /// </summary>
    Dictionary<string, object> Globals { get; set; }

    void Initialize(StartupConfig config);
    void Migrate();
    void Deploy(Stream packageStream);
    void UpdateAssembly(Assembly asm);
}