﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using ZenPlatform.Configuration;
using ZenPlatform.Core.Helpers;
using ZenPlatform.Data;
using ZenPlatform.QueryBuilder;
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

        public XCDatabaseStorage(string tableName, DataContext context, SqlCompillerBase compiler)
        {
            _tableName = tableName;
            _context = context;
            _compiler = compiler;
        }

        public byte[] GetBlob(string name, string route)
        {
            var query = new SelectQueryNode().From(_tableName).Select("Data")
                .Where(x => x.Field("BlobName"), "=", x => x.Parameter("BlobName"));

            var cmdText = _compiler.Compile(query);
            using (var cmd = _context.CreateCommand())
            {
                cmd.CommandText = cmdText;
                cmd.AddParameterWithValue("BlobName", name);

                return (byte[]) cmd.ExecuteScalar();
            }
        }

        public string GetStringBlob(string name, string route)
        {
            return Encoding.UTF8.GetString(GetBlob(name, route));
        }

        public void SaveBlob(string name, string route, byte[] bytes)
        {
            var searchQuery = new SelectQueryNode()
                .From(_tableName)
                .SelectRaw("1")
                .Where(x => x.Field("BlobName"), "=", x => x.Parameter("BlobName"));

            var cmdText = _compiler.Compile(searchQuery);
            using (var cmd = _context.CreateCommand())
            {
                cmd.CommandText = cmdText;
                cmd.AddParameterWithValue("BlobName", name);

                Node query;
                //Если такой ключ удалось найти, значит всё ок, обновляем его, иначе вставляем
                if (cmd.ExecuteScalar() is null)
                {
                    query = new InsertQueryNode()
                        .InsertInto(_tableName)
                        .WithFieldAndValue(x => x.Field("BlobName"), x => x.Parameter("BlobName"))
                        .WithFieldAndValue(x => x.Field("Data"), x => x.Parameter("Data"));
                }
                else
                {
                    query = new UpdateQueryNode()
                        .Update(_tableName)
                        .Set(x => x.Field("Data"), x => x.Parameter("Data"))
                        .Where(x => x.Field("BlobName"), "=", x => x.Parameter("BlobName"));
                }

                cmd.CommandText = _compiler.Compile(query);
                cmd.AddParameterWithValue("Data", bytes);

                cmd.ExecuteNonQuery();
            }
        }

        public void SaveBlob(string name, string route, string data)
        {
            SaveBlob(name, route, Encoding.UTF8.GetBytes(data));
        }

        public byte[] GetRootBlob()
        {
            return GetBlob("root", "");
        }

        public string GetStringRootBlob()
        {
            return GetStringBlob("root", "");
        }
    }
}