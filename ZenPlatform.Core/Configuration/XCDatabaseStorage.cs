using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;
using ZenPlatform.Configuration;
using ZenPlatform.Core.Helpers;
using ZenPlatform.Data;
using ZenPlatform.Initializer;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.QueryBuilder.DML.Insert;
using ZenPlatform.QueryBuilder.DML.Select;
using ZenPlatform.QueryBuilder.DML.Update;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.Core.Configuration
{
    public class XCDatabaseStorage : IXCConfigurationStorage
    {
        private readonly string _tableName;
        private readonly DataContext _context;
        private readonly SqlCompillerBase _compiler;
        private uint _maxId = 100;

        //TODO: Посмотерть использование класса InternalDbContext в качестве аргумента конструктора
        public XCDatabaseStorage(string tableName, DataContext context, SqlCompillerBase compiler)
        {
            _tableName = tableName;
            _context = context;
            _compiler = compiler;
        }

        public Stream GetBlob(string name, string route)
        {
            var query = new SelectQueryNode().From(_tableName).Select(DatabaseConstantNames.CONFIG_TABLE_DATA_FIELD)
                .Where(x => x.Field(DatabaseConstantNames.CONFIG_TABLE_BLOB_NAME_FIELD), "=",
                    x => x.Parameter(DatabaseConstantNames.CONFIG_TABLE_BLOB_NAME_FIELD));

            route = route + ":";

            var cmdText = _compiler.Compile(query);
            using (var cmd = _context.CreateCommand())
            {
                cmd.CommandText = cmdText;
                cmd.AddParameterWithValue(DatabaseConstantNames.CONFIG_TABLE_BLOB_NAME_FIELD, $"{route}{name}");

                MemoryStream ms = new MemoryStream((byte[])cmd.ExecuteScalar());

                return ms;
            }
        }

        public void SaveBlob(string name, string route, Stream stream)
        {
            var searchQuery = new SelectQueryNode()
                .From(_tableName)
                .SelectRaw("1")
                .Where(x => x.Field(DatabaseConstantNames.CONFIG_TABLE_BLOB_NAME_FIELD), "=",
                    x => x.Parameter(DatabaseConstantNames.CONFIG_TABLE_BLOB_NAME_FIELD));

            route = route + ":";

            var cmdText = _compiler.Compile(searchQuery);
            using (var cmd = _context.CreateCommand())
            {
                cmd.CommandText = cmdText;
                //Первый параметр мы здесь добавляем
                cmd.AddParameterWithValue(DatabaseConstantNames.CONFIG_TABLE_BLOB_NAME_FIELD, $"{route}{name}");

                SqlNode query;
                //Если такой ключ удалось найти, значит всё ок, обновляем его, иначе вставляем
                if (cmd.ExecuteScalar() is null)
                {
                    query = new InsertQueryNode()
                        .InsertInto(_tableName)
                        .WithFieldAndValue(x => x.Field(DatabaseConstantNames.CONFIG_TABLE_BLOB_NAME_FIELD),
                            x => x.Parameter(DatabaseConstantNames.CONFIG_TABLE_BLOB_NAME_FIELD))
                        .WithFieldAndValue(x => x.Field(DatabaseConstantNames.CONFIG_TABLE_DATA_FIELD),
                            x => x.Parameter(DatabaseConstantNames.CONFIG_TABLE_DATA_FIELD));
                }
                else
                {
                    query = new UpdateQueryNode()
                        .Update(_tableName)
                        .Set(x => x.Field(DatabaseConstantNames.CONFIG_TABLE_DATA_FIELD),
                            x => x.Parameter(DatabaseConstantNames.CONFIG_TABLE_DATA_FIELD))
                        .Where(x => x.Field(DatabaseConstantNames.CONFIG_TABLE_BLOB_NAME_FIELD), "=",
                            x => x.Parameter(DatabaseConstantNames.CONFIG_TABLE_BLOB_NAME_FIELD));
                }

                cmd.CommandText = _compiler.Compile(query);

                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int)stream.Length);

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