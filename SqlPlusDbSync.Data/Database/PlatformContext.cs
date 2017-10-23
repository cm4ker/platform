using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using QueryCompiler.Schema;
using SqlPlusDbSync.Shared;

namespace SqlPlusDbSync.Data.Database
{
    public class PlatformContext : DBContext
    {
        public PlatformContext(string connectionString) : base(connectionString, false)
        {

        }
    }
}
