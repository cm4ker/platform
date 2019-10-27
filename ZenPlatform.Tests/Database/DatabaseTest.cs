using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Npgsql;
using Xunit;
using ZenPlatform.ConfigurationExample;
using ZenPlatform.Data.Tools;
using ZenPlatform.QueryBuilder;


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

        [Fact]
        public void UniversalConnetionStringBuilderPostgresTest()
        {
            var sb = new UniversalConnectionStringBuilder(SqlDatabaseType.Postgres);
            sb.Database = "db";
            sb.Password = "password";
            sb.Username = "username";
            sb.Server = "localhost";
            sb.Port = 10;

            var cs = sb.GetConnectionString();

            var expected = "Username=username;Password=password;Host=localhost;Port=10;Database=db";

            var sb2 = UniversalConnectionStringBuilder.FromConnectionString(SqlDatabaseType.Postgres, expected);

            Assert.Equal(expected, cs);

            Assert.Equal("username", sb2.Username);
            Assert.Equal("password", sb2.Password);
            Assert.Equal("localhost", sb2.Server);
            Assert.Equal(10, sb2.Port);
            Assert.Equal("db", sb2.Database);

        }
    }
}
