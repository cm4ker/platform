using System;
using Npgsql;
using Microsoft.Data.SqlClient;

namespace Aquila.Data.Tools
{
    /// <summary>
    /// Universal connection string builder
    /// </summary>
    public class UniversalConnectionStringBuilder
    {
        public UniversalConnectionStringBuilder()
        {
        }

        public UniversalConnectionStringBuilder(SqlDatabaseType dbType)
        {
            SqlDatabaseType = dbType;
        }

        public SqlDatabaseType SqlDatabaseType { get; set; }

        public static UniversalConnectionStringBuilder FromConnectionString(SqlDatabaseType dbType,
            string connectionString)
        {
            switch (dbType)
            {
                case SqlDatabaseType.SqlServer:
                {
                    var sb = new UniversalConnectionStringBuilder(dbType);
                    var sqlServerSb = new SqlConnectionStringBuilder(connectionString);

                    var serverPort = sqlServerSb.InitialCatalog.Split(',');

                    sb.Database = serverPort[0];
                    sb.Server = sqlServerSb.DataSource;
                    sb.Username = sqlServerSb.UserID;
                    if (serverPort.Length > 1)
                        sb.Port = int.Parse(serverPort[1]);
                    sb.Password = sqlServerSb.Password;

                    return sb;
                }
                case SqlDatabaseType.Postgres:
                {
                    var sb = new UniversalConnectionStringBuilder(dbType);
                    var conSb = new NpgsqlConnectionStringBuilder(connectionString);

                    sb.Database = conSb.Database;
                    sb.Server = conSb.Host;
                    sb.Username = conSb.Username;
                    sb.Port = conSb.Port;
                    sb.Password = conSb.Password;

                    return sb;
                }
            }

            throw new NotSupportedException();
        }


        /// <summary>
        /// DB
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// Server
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// Port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }


        public string GetConnectionString()
        {
            switch (SqlDatabaseType)
            {
                case SqlDatabaseType.Postgres:
                {
                    var sb = new NpgsqlConnectionStringBuilder();

                    sb.Username = Username;
                    sb.Password = Password;
                    sb.Host = Server;
                    sb.Port = (Port == 0) ? 5432 : Port;
                    sb.Database = Database;

                    return sb.ConnectionString;
                }

                case SqlDatabaseType.SqlServer:
                {
                    var sb = new SqlConnectionStringBuilder();

                    sb.UserID = Username;
                    sb.Password = Password;
                    sb.DataSource = Server;
                    sb.InitialCatalog = Database;

                    //TODO: Port handling like a "[ServerName], [Port]"

                    return sb.ConnectionString;
                }
            }

            return "";
        }
    }
}