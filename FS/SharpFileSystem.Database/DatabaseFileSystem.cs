using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;

namespace SharpFileSystem.Database
{
    public class DatabaseFileSystem : IFileSystem
    {
        private DbConnection _conn;
        private string _tableName;

        /*
         TABLE SCRIPT

         Create table
            vTable(
	            Path varchar(3000),
	            Name varchar(3000),
	            IsDirectory bit,
	            Data varbinary(max)
            )

         */

        public DatabaseFileSystem(string connectionString, string tableName = "vTable")
        {
            _conn = new SqlConnection(connectionString);
            _conn.Open();
            _tableName = tableName;
        }

        public void Dispose()
        {
            _conn.Close();
        }

        public ICollection<FileSystemPath> GetEntities(FileSystemPath path)
        {
            var result = new List<FileSystemPath>();
            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = $"SELECT Path, Name FROM {_tableName} WHERE Path = @Path";
                var param = cmd.CreateParameter();
                param.Value = path.Path;
                param.ParameterName = "path";
                cmd.Parameters.Add(param);

                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        result.Add(FileSystemPath.Parse($"{r["Path"]}{r["Name"]}"));
                    }
                }
            }

            return result;
        }

        public bool Exists(FileSystemPath path)
        {
            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = $"SELECT 1 Name FROM {_tableName} WHERE Path = @Path and Name = @name";
                var param = cmd.CreateParameter();
                param.Value = path.ParentPath.ToString();
                param.ParameterName = "path";
                cmd.Parameters.Add(param);

                param = cmd.CreateParameter();
                param.Value = path.EntityName;
                param.ParameterName = "name";
                cmd.Parameters.Add(param);

                return cmd.ExecuteScalar() != null;
            }
        }

        public Stream CreateFile(FileSystemPath path)
        {
            var result = new DBFileStream(path, _conn.CreateCommand(),_tableName, false);
            result.Flush();
            return result;
        }

        public Stream OpenFile(FileSystemPath path, FileAccess access)
        {
            return new DBFileStream(path, _conn.CreateCommand(), _tableName,true);
        }

        public void CreateDirectory(FileSystemPath path)
        {
            new DBFileStream(path, _conn.CreateCommand(), _tableName, false).Flush();
        }

        public void Delete(FileSystemPath path)
        {
            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = $"DELETE FROM {_tableName} WHERE Path = @Path and Name = @name";
                var param = cmd.CreateParameter();
                param.Value = path.Path;
                param.ParameterName = "path";
                cmd.Parameters.Add(param);

                param = cmd.CreateParameter();
                param.Value = path.EntityName;
                param.ParameterName = "name";
                cmd.Parameters.Add(param);

                cmd.ExecuteNonQuery();
            }
        }

        public void ClearTable()
        {
            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = $"DELETE FROM {_tableName}";
                cmd.ExecuteNonQuery();
            }
        }
    }
}
