using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace ZenPlatform.QueryBuilder2
{
    class Program
    {
        static void Main(string[] args)
        {
            var q = new SelectQueryNode()
                    .From("table1")
                    .LeftJoin("table2", "someTalbe")
                    .Select("field1", "someTable", "HeyYouAreNewField")
                    .SelectRaw("CASE WHEN 1 = 1 THEN '1' ELSE '2' END")
                ;
        }
    }
}