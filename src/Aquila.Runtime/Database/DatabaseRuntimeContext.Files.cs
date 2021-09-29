using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Aquila.Core;
using Aquila.Core.Assemlies;
using Aquila.Data;
using Aquila.QueryBuilder;

namespace Aquila.Runtime
{
    public partial class DatabaseRuntimeContext
    {
        /// <summary>
        /// Contest for working with files table
        /// </summary>
        public class FilesRC
        {
            private readonly string _tableName;

            public FilesRC(string tableName)
            {
                _tableName = tableName;
            }

            private FileDescriptor Map(DbDataReader reader)
            {
                return new FileDescriptor()
                {
                    Type = (FileType)reader.GetInt32(0),
                    Name = reader.GetString(1),
                };
            }

            public byte[] GetFile(DataConnectionContext dcc, string name)
            {
                void Gen(QueryMachine qm)
                {
                    qm
                        .bg_query()
                        .m_from()
                        .ld_table(_tableName)
                        .m_where()
                        .ld_column("name")
                        .ld_param("name")
                        .eq()
                        .m_select()
                        .ld_column("data")
                        .st_query();
                }


                using (var cmd = dcc.CreateCommand(Gen))
                {
                    cmd.AddParameterWithValue("name", name);

                    return (byte[])cmd.ExecuteScalar();
                }
            }

            public byte[] GetFile(DataConnectionContext dcc, FileDescriptor descriptor)
            {
                return GetFile(dcc, descriptor.Name);
            }

            public byte[] GetMainAssembly(DataConnectionContext dcc)
            {
                var desc = GetFileDescriptors(dcc).FirstOrDefault(x => (x.Type & FileType.MainAssembly) > 0);

                if (desc == null)
                    return null;

                return GetFile(dcc, desc);
            }

            public void Clear(DataConnectionContext dcc)
            {
                void Gen(QueryMachine qm)
                {
                    qm
                        .bg_query()
                        .m_from()
                        .ld_table(_tableName)
                        .m_where()
                        .ld_const("1")
                        .ld_const("1")
                        .eq()
                        .m_delete()
                        .st_query();
                }

                using (var cmd = dcc.CreateCommand(Gen))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            public IEnumerable<FileDescriptor> GetFileDescriptors(DataConnectionContext dcc)
            {
                var list = new List<FileDescriptor>();


                void Gen(QueryMachine qm)
                {
                    qm
                        .bg_query()
                        .m_from()
                        .ld_table(_tableName)
                        //.eq()
                        //.and()
                        .m_select()
                        .ld_column("type")
                        .ld_column("name")
                        .st_query();
                }


                using (var cmd = dcc.CreateCommand(Gen))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(Map(reader));
                        }
                    }

                    return list;
                }
            }

            public void SaveFile(DataConnectionContext dcc, FileDescriptor descriptor, byte[] blob)
            {
                void Gen(QueryMachine qm)
                {
                    qm
                        .bg_query()
                        .m_values()
                        .ld_param("type")
                        .ld_param("name")
                        .ld_param("data")
                        .m_insert()
                        .ld_table(_tableName)
                        .ld_column("type")
                        .ld_column("name")
                        .ld_column("data")
                        .st_query();
                }

                using (var cmd = dcc.CreateCommand(Gen))
                {
                    cmd.AddParameterWithValue("name", descriptor.Name);
                    cmd.AddParameterWithValue("type", descriptor.Type);


                    cmd.AddParameterWithValue("data", blob);

                    cmd.ExecuteNonQuery();
                }
            }

            public void RemoveFile(DataConnectionContext dcc, string hash)
            {
                throw new NotImplementedException();
            }

            public void TransferTo(DataConnectionContext dcc, FilesRC destenation)
            {
                dcc.BeginTransaction();
                
                Clear(dcc);
                dcc.CreateCommand((qm) =>
                    qm.bg_query()
                        .m_from()
                        .ld_table(_tableName)
                        .@as("t1")
                        .m_select()
                        .ld_column("*", "t1")
                        .m_insert()
                        .ld_table(destenation._tableName)
                        .st_query()).ExecuteNonQuery();
                
                dcc.CommitTransaction();
            }
        }
    }
}