using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using ZenPlatform.Core.Network;
using System.IO;
using System.Reflection;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Microsoft.Extensions.DependencyInjection;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.Logging;

namespace ZenPlatform.ServerRPC
{
    class Program
    {
        static void Main(string[] args)
        {
            DbConnection con = new SqlConnection();

            con.ConnectionString =
                "Data source=PC702\\Asna; Initial catalog=asna_apt_019; User Id=sa; Password=sapwd123;"; // MultipleActiveResultSets=True";

            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText = "select * from sys.tables";
            var reader1 = cmd.ExecuteReader();
            
            var cmd2 = con.CreateCommand();
            cmd2.CommandText = "SElect * from sys.tables";
            var reader2 = cmd2.ExecuteReader();
        }
    }
}