using System;
using System.Linq;
using Aquila.Data;
using Aquila.Initializer;
using Aquila.Logging;
using Aquila.Metadata;
using Aquila.Migrations;
using Aquila.Runtime.Querying;
using RamDisk;
using SqlInMemory;
using Xunit;

namespace Aquila.Runtime.Tests.DB
{
    public class DatabaseFixture : IDisposable
    {
        private IDisposable _inmemDb;
        public UserSecTable Ust { get; private set; }
        public DatabaseRuntimeContext DrContext { get; private set; }
        public DataConnectionContext DcContext { get; private set; }

        public DatabaseFixture()
        {
            try
            {
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

            var cm = new DataContextManager();
            cm.Initialize(SqlDatabaseType.SqlServer, TestMetadata.DefaultConnetionString);

            DrContext = new DatabaseRuntimeContext();
            DcContext = cm.GetContext();

            Ust = new UserSecTable();
            Ust.Init(TestMetadata.GetTestMetadata().GetSecPolicies().ToList(), TestMetadata.GetTestMetadata());

            DrContext.PendingMetadata.SetMetadata(TestMetadata.GetTestMetadata());
            DrContext.SaveAll(DcContext);

            Assert.True(DrContext.PendingMetadata.GetMetadata().Metadata.Any());

            var mm = new MigrationManager(cm, new NLogger<MigrationManager>());
            mm.Migrate();

            DrContext = new DatabaseRuntimeContext();
            DrContext.LoadAll(DcContext);
        }


        public void Dispose()
        {
            _inmemDb?.Dispose();
        }
    }
}