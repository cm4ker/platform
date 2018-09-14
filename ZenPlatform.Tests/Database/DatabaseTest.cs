using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Npgsql;
using Xunit;
using ZenPlatform.Tests.Common;

namespace ZenPlatform.Tests.Database
{
    public class DatabaseTest
    {
        [Fact]
        public void ConnectionTest()
        {
            var cs = Factory.GetDatabaseConnectionString();
            NpgsqlConnection test = new NpgsqlConnection(cs);

            try
            {
                test.Open();
            }
            catch
            {

            }

            Assert.True(test.State == ConnectionState.Open);

        }
    }
}
