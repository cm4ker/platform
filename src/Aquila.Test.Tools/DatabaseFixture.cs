﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Aquila.Core;
using Aquila.Core.Infrastructure.Settings;
using Aquila.Core.Instance;
using Aquila.Core.Test;
using Aquila.Data;
using Aquila.Initializer;
using Aquila.Metadata;
using Aquila.Runtime.Querying;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Configurations.Databases;
using DotNet.Testcontainers.Containers.Modules.Abstractions;
using DotNet.Testcontainers.Containers.Modules.Databases;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Aquila.Test.Tools
{
    public class DatabaseFixture : IAsyncLifetime
    {
        private readonly TestcontainerDatabase _container = new TestcontainersBuilder<PostgreSqlTestcontainer>()
            .WithDatabase(new PostgreSqlTestcontainerConfiguration()
            {
                Password = "password",
                Database = "db",
                Username = "User"
            })
            .Build();

        public ContextSecTable Ust { get; private set; }

        public AqInstance Instance { get; private set; }

        public AqContext Context { get; private set; }
        
        public string? ConnectionString => _container.ConnectionString;

        public DatabaseFixture()
        {
        }

        private void InitCore()
        {
            var cs = _container.ConnectionString;
            MigrationRunner.Migrate(cs, SqlDatabaseType.Postgres);

            var service = TestEnvSetup.GetServerService();

            var config = service.GetService<ISettingsStorage>();
            config.Get<AppConfig>().Instances.Add(new StartupConfig
            {
                ConnectionString = _container.ConnectionString,
                DatabaseType = SqlDatabaseType.Postgres,
                InstanceName = "Library"
            });

            var manager = service.GetService<IAqInstanceManager>();
            Instance = manager.TryGetInstance("Library") ?? throw new InvalidOperationException("Instance not found");

            var drContext = Instance.DatabaseRuntimeContext;
            var dcContext = Instance.DataContextManager.GetContext();

            Ust = new ContextSecTable();
            Ust.Init(TestMetadata.GetTestMetadata().GetSecPolicies());

            drContext.PendingMetadata.SetMetadata(TestMetadata.GetTestMetadata());
            drContext.SaveAll(dcContext);

            Assert.True(drContext.PendingMetadata.GetMetadata().EntityMetadata.Any());

            Instance.Migrate();

            Context = new AqContext(Instance) { User = "Test", Roles = new[] { TestMetadata.TestSecName } };
        }

        public async Task InitializeAsync()
        {
            await this._container.StartAsync();
            InitCore();
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            await _container.DisposeAsync();
        }
    }
}