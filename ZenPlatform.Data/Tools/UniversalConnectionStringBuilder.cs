using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using Npgsql;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Data.Tools
{
    /// <summary>
    /// Универсальный билдер строки подключения к базе данных, основывается на типе базы данных
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


        /// <summary>
        /// База данных
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// Сервер
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// Порт сервера
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Пароль
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

                        //TODO: Сделать так что бы обрабатывался номер порта, насколько япомню, это достигалось за счёт конкатенации его через запятую с имененм сервереа "[ServerName], [Port]"

                        return sb.ConnectionString;
                    }
            }

            return "";
        }
    }
}
