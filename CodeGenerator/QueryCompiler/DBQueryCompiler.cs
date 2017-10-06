﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using QueryCompiler.Schema;

namespace QueryCompiler
{
    public class DBQueryCompiler
    {
        private readonly DBContext _context;
        private readonly DBSchemaManager _schemaManager;

        public DBQueryCompiler(DBContext context)
        {
            _context = context;
            _schemaManager = new DBSchemaManager(_context);
        }

        public DBSelectQuery CreateSelect()
        {
            return new DBSelectQuery();
        }

        public DBUpdateQuery CreateUpdate()
        {
            return new DBUpdateQuery();
        }

        public DBDeleteQuery CreateDelete()
        {
            return new DBDeleteQuery();
        }

        public DBInsertQuery CreateInsert()
        {
            return new DBInsertQuery();
        }

        public DBSetIdentityInsert CreateSetIdentity(string tableName, bool isOn)
        {
            return new DBSetIdentityInsert(tableName, isOn);
        }

        public DBTable CreateTable(string name, string alias = "")
        {
            DBTable table = new DBTable(name, _schemaManager, alias);
            return table;
        }
    }
}
