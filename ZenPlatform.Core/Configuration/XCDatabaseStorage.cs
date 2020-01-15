using System;
using System.IO;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Core.Helpers;
using ZenPlatform.Data;
using ZenPlatform.Initializer;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.Core.Configuration
{
    public class XCDatabaseStorage : IXCConfigurationStorage
    {
        private readonly string _tableName;
        private readonly DataContext _context;
        private uint _maxId = 100;

        //TODO: Посмотерть использование класса InternalDbContext в качестве аргумента конструктора
        public XCDatabaseStorage(string tableName, DataContext context, ISqlCompiler compiler = null)
        {
            _tableName = tableName;
            _context = context;
        }

        public Stream GetBlob(string name, string route)
        {
            void Gen(QueryMachine qm)
            {
                qm.bg_query()
                    .m_from()
                    .ld_table(_tableName)
                    .m_where()
                    .ld_column(DatabaseConstantNames.CONFIG_TABLE_BLOB_NAME_FIELD)
                    .ld_param(DatabaseConstantNames.CONFIG_TABLE_BLOB_NAME_FIELD)
                    .eq()
                    .m_select()
                    .ld_column(DatabaseConstantNames.CONFIG_TABLE_DATA_FIELD)
                    .st_query();
            }


            route = route + ":";

            using (var cmd = _context.CreateCommand(Gen))
            {
                cmd.AddParameterWithValue(DatabaseConstantNames.CONFIG_TABLE_BLOB_NAME_FIELD, $"{route}{name}");

                MemoryStream ms = new MemoryStream((byte[]) cmd.ExecuteScalar());

                return ms;
            }
        }

        public void SaveBlob(string name, string route, Stream stream)
        {
            void Gen(QueryMachine qm)
            {
                qm.bg_query()
                    .m_from()
                    .ld_table(_tableName)
                    .m_where()
                    .ld_column(DatabaseConstantNames.CONFIG_TABLE_BLOB_NAME_FIELD)
                    .ld_param(DatabaseConstantNames.CONFIG_TABLE_BLOB_NAME_FIELD)
                    .eq()
                    .m_select()
                    .ld_const(1)
                    .st_query();
            }


            route = route + ":";

            using (var cmd = _context.CreateCommand(Gen))
            {
                //Первый параметр мы здесь добавляем
                cmd.AddParameterWithValue(DatabaseConstantNames.CONFIG_TABLE_BLOB_NAME_FIELD, $"{route}{name}");

                //SqlNode query;
                //Если такой ключ удалось найти, значит всё ок, обновляем его, иначе вставляем
                QueryMachine qm = new QueryMachine();
                if (cmd.ExecuteScalar() is null)
                {
                    qm
                        .bg_query()
                        .m_values()
                        .ld_param(DatabaseConstantNames.CONFIG_TABLE_BLOB_NAME_FIELD)
                        .ld_param(DatabaseConstantNames.CONFIG_TABLE_DATA_FIELD)
                        .m_insert()
                        .ld_table(_tableName)
                        .ld_column(DatabaseConstantNames.CONFIG_TABLE_BLOB_NAME_FIELD)
                        .ld_column(DatabaseConstantNames.CONFIG_TABLE_DATA_FIELD)
                        .st_query();
                }
                else
                {
                    qm
                        .bg_query()
                        .m_where()
                        .ld_column(DatabaseConstantNames.CONFIG_TABLE_BLOB_NAME_FIELD)
                        .ld_param(DatabaseConstantNames.CONFIG_TABLE_BLOB_NAME_FIELD)
                        .eq()
                        .m_set()
                        .ld_column(DatabaseConstantNames.CONFIG_TABLE_DATA_FIELD)
                        .ld_param(DatabaseConstantNames.CONFIG_TABLE_DATA_FIELD)
                        .assign()
                        .m_update()
                        .ld_table(_tableName)
                        .st_query();
                }

                cmd.CommandText = _context.SqlCompiller.Compile((SSyntaxNode) qm.pop());

                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int) stream.Length);

                cmd.AddParameterWithValue(DatabaseConstantNames.CONFIG_TABLE_DATA_FIELD, buffer);

                cmd.ExecuteNonQuery();
            }
        }


        public Stream GetRootBlob()
        {
            return GetBlob("root", "");
        }

        public void SaveRootBlob(Stream stream)
        {
            SaveBlob("root", "", stream);
        }

        public void GetId(Guid confId, ref uint uid)
        {
            if (uid != 0)
                return;

            uid = _maxId++;
        }
    }
}