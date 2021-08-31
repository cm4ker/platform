﻿using System;
using System.Collections.Generic;
using System.Text;
using Aquila.Core.Instance;
using Aquila.Core.Settings;
using Aquila.Data;

namespace Aquila.Core.Test
{
    public class TestSettingsStorage : ISettingsStorage
    {
        private AppConfig _appConfig = new AppConfig()
        {
            AccessPoint = new AccessPointConfig()
            {
                Listener = new List<ListenerConfig>()
                {
                    new ListenerConfig()
                    {
                        Address = "127.0.0.1:12345",
                        Type = PointType.Instance
                    },
                    // new ListenerConfig()
                    // {
                    //     Address = "127.0.0.1:22",
                    //     Type = ListenerType.Admin
                    // }
                }
            },
            Environments = new List<StartupConfig>()
            {
                new StartupConfig()
                {
                    ConnectionString =
                        "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=testdb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False",
                    DatabaseType = SqlDatabaseType.SqlServer
                }
            }
        };

        public T Get<T>() where T : class, new()
        {
            if (typeof(T).Equals(_appConfig.GetType()))
                return _appConfig as T;

            return null;
        }

        public void Save()
        {
        }
    }
}