﻿using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Core.Settings;

namespace ZenPlatform.Core.Test
{
    public class TestSettingsStorage : ISettingsStorage
    {
        private AppConfig _appConfig = new AppConfig()
        {
            AccessPoint = new AccessPointConfig()
            {
                Listener = new List<ListenerConfig>() {
                    new ListenerConfig() {
                        Address = "127.0.0.1:12345",
                        Type = ListenerType.Test }
                }
            },
            Environments = new List<StartupConfig>() {
                new StartupConfig() {
                    ConnectionString = "",
                    DatabaseType = QueryBuilder.SqlDatabaseType.SqlServer
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