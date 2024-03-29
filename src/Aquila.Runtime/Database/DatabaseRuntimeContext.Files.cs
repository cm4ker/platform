﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Aquila.Core;
using Aquila.Core.Assemlies;
using Aquila.Data;
using Aquila.QueryBuilder;
using DBConsts = Aquila.Initializer.DBConstNames.Files;


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
                        .ld_column(DBConsts.NAME_COLUMN)
                        .ld_param(DBConsts.NAME_COLUMN)
                        .eq()
                        .m_select()
                        .ld_column(DBConsts.DATA_COLUMN)
                        .st_query();
                }


                using (var cmd = dcc.CreateCommand(Gen))
                {
                    cmd.AddParameterWithValue(DBConsts.NAME_COLUMN, name);

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
                        .@as("T0")
                        .m_where()
                        .ld_const("1")
                        .ld_const("1")
                        .eq()
                        .m_delete()
                        .ld_table("T0")
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
                        .ld_column(DBConsts.TYPE_COLUMN)
                        .ld_column(DBConsts.NAME_COLUMN)
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
                        .ld_param(DBConsts.TYPE_COLUMN)
                        .ld_param(DBConsts.NAME_COLUMN)
                        .ld_param(DBConsts.DATA_COLUMN)
                        .m_insert()
                        .ld_table(_tableName)
                        .ld_column(DBConsts.TYPE_COLUMN)
                        .ld_column(DBConsts.NAME_COLUMN)
                        .ld_column(DBConsts.DATA_COLUMN)
                        .st_query();
                }

                using (var cmd = dcc.CreateCommand(Gen))
                {
                    cmd.AddParameterWithValue(DBConsts.NAME_COLUMN, descriptor.Name);
                    cmd.AddParameterWithValue(DBConsts.TYPE_COLUMN, descriptor.Type);


                    cmd.AddParameterWithValue(DBConsts.DATA_COLUMN, blob);

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

                destenation.Clear(dcc);
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