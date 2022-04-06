using System;
using System.IO;
using System.Linq;
using Aquila.Core;
using Aquila.Core.Instance;
using Aquila.Core.Test;
using Aquila.Data;
using Aquila.Initializer;
using Aquila.Logging;
using Aquila.Metadata;
using Aquila.Migrations;
using Aquila.Runtime.Querying;
using Microsoft.Extensions.DependencyInjection;
using RamDisk;
using SqlInMemory;
using Xunit;

namespace Aquila.Runtime.Tests.DB
{
    public class DatabaseFixture : IDisposable
    {
        private IDisposable _inmemDb;
        public ContextSecTable Ust { get; private set; }

        public AqInstance Instance { get; private set; }

        public AqContext Context { get; private set; }

        public DatabaseFixture()
        {
            try
            {
                if (DriveInfo.GetDrives().Any(d => d.Name.ToUpper()[0] == 'Z'))
                    RamDrive.Unmount();
            }
            catch (InvalidOperationException ex)
            {
                // do nothing
            }

            _inmemDb = SqlInMemoryDb.Create(TestMetadata.DefaultConnetionString);
            try
            {
                InitCore();
            }
            catch
            {
                _inmemDb.Dispose();
                throw;
            }
        }

        private void InitCore()
        {
            Assert.True(SqlHelper.DatabaseExists(TestMetadata.DefaultConnetionString));
            MigrationRunner.Migrate(TestMetadata.DefaultConnetionString, SqlDatabaseType.SqlServer);

            var service = TestEnvSetup.GetServerService();
            var manager = service.GetService<IAqInstanceManager>();
            Instance = manager.GetInstance("Library");

            var drContext = Instance.DatabaseRuntimeContext;
            var dcContext = Instance.DataContextManager.GetContext();

            Ust = new ContextSecTable();
            Ust.Init(TestMetadata.GetTestMetadata().GetSecPolicies().ToList());

            drContext.PendingMetadata.SetMetadata(TestMetadata.GetTestMetadata());
            drContext.SaveAll(dcContext);

            Assert.True(drContext.PendingMetadata.GetMetadata().EntityMetadata.Any());

            Instance.Migrate();

            Context = new AqContext(Instance) { User = "Test", Roles = new[] { TestMetadata.TestSecName } };
        }


        public void Dispose()
        {
            _inmemDb?.Dispose();
        }
    }
}