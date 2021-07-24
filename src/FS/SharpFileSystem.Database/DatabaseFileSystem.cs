using System.Collections.Generic;
using System.IO;
using Aquila.Data;
using Aquila.QueryBuilder;
using Aquila.QueryBuilder.Builders;
using Aquila.QueryBuilder.Model;

namespace SharpFileSystem.Database
{
    public class DatabaseFileSystem : IFileSystem
    {
        internal DataConnectionContext Context { get; }
        internal string TableName { get; }

        public DatabaseFileSystem(string tableName, DataConnectionContext context)
        {
            TableName = tableName;
            Context = context;

            CheckTableExists();
        }

        private void CheckTableExists()
        {
            var q = DDLQuery.New();
            var tab = q.Create()
                .Table(TableName)
                .CheckExists();

            tab.WithColumn("Path").SetType(new ColumnTypeVarChar() { Size = 3000 });
            tab.WithColumn("Name").SetType(new ColumnTypeVarChar() { Size = 3000 });
            tab.WithColumn("IsDirectory").SetType(new ColumnTypeBool());
            tab.WithColumn("Data").SetType(new ColumnTypeVarBinary());

            using var a = Context.CreateCommand(q.Expression);
            a.ExecuteNonQuery();
        }

        public void Dispose()
        {
        }

        public ICollection<FileSystemPath> GetEntities(FileSystemPath path)
        {
            void Gen(QueryMachine qm)
            {
                qm.bg_query()
                    .m_from()
                    .ld_table(TableName)
                    .m_where()
                    .ld_column("Path")
                    .ld_param("Path")
                    .eq()
                    .m_select()
                    .ld_column("Path")
                    .ld_column("Name")
                    .st_query();
            }


            using (var cmd = Context.CreateCommand(Gen))
            {
                cmd.AddOrSetParameterWithValue("Path", path.Path);
                var result = new List<FileSystemPath>();

                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        result.Add(FileSystemPath.Parse($"{r["Path"]}{r["Name"]}"));
                    }
                }

                return result;
            }
        }

        public bool Exists(FileSystemPath path)
        {
            void Gen(QueryMachine qm)
            {
                qm.bg_query()
                    .m_from()
                    .ld_table(TableName)
                    .m_where()
                    .ld_column("Path")
                    .ld_param("Path")
                    .eq()
                    .ld_column("Name")
                    .ld_param("Name")
                    .eq()
                    .and()
                    .m_select()
                    .ld_const("1")
                    .st_query();
            }


            using (var cmd = Context.CreateCommand(Gen))
            {
                cmd.AddOrSetParameterWithValue("Path", path.ParentPath.ToString());
                cmd.AddOrSetParameterWithValue("Name", path.EntityName);

                return cmd.ExecuteScalar() != null;
            }
        }

        public Stream CreateFile(FileSystemPath path)
        {
            var result = new DBFileStream(path, this, FileAccess.Write);
            result.Flush();
            return result;
        }

        public Stream OpenFile(FileSystemPath path, FileAccess access)
        {
            return new DBFileStream(path, this, access);
        }

        public void CreateDirectory(FileSystemPath path)
        {
            using var fs = new DBFileStream(path, this, FileAccess.Write);
            fs.Flush();
        }

        public void Delete(FileSystemPath path)
        {
            void Gen(QueryMachine qm)
            {
                qm.bg_query()
                    .m_delete()
                    .ld_table(TableName)
                    .m_where()
                    .ld_column("Path")
                    .ld_param("Path")
                    .eq()
                    .ld_column("Name")
                    .ld_param("Name")
                    .eq()
                    .st_query();
            }


            using (var cmd = Context.CreateCommand(Gen))
            {
                cmd.AddOrSetParameterWithValue("Path", path.ParentPath.ToString());
                cmd.AddOrSetParameterWithValue("Name", path.EntityName);

                cmd.ExecuteNonQuery();
            }
        }

        public void ClearTable()
        {
            void Gen(QueryMachine qm)
            {
                qm.bg_query()
                    .m_from()
                    .ld_table(TableName)
                    .m_delete()
                    .st_query();
            }

            using var cmd = Context.CreateCommand(Gen);
            cmd.ExecuteNonQuery();
        }
    }
}