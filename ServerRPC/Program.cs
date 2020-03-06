using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using ZenPlatform.Core.Network;
using System.Reflection;
using BufferedDataReaderDotNet;
using ConsoleTables;
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
                "Data source=(LocalDb)\\MSSQLLocalDB; Initial catalog=testdb; Integrated Security= true;"; // MultipleActiveResultSets=True";

            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText = "select name, object_id from sys.tables";
            var reader1 = cmd.ExecuteReader().GetBufferedData().GetDataReader();

            reader1.ToConsole();

            Console.ReadKey();

            // var cmd2 = con.CreateCommand();
            // cmd2.CommandText = "SElect * from sys.tables";
            // var reader2 = cmd2.ExecuteReader();
        }
    }
}