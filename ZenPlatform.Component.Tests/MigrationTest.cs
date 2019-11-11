using System;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Xunit;
using Xunit.Abstractions;
using ZenPlatform.Compiler;
using ZenPlatform.ConfigurationExample;
using ZenPlatform.QueryBuilder;


namespace ZenPlatform.Component.Tests
{
    public class MigrationTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public MigrationTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void CreateTest()
        {
            var root = Factory.CreateExampleConfiguration();

            var com = root.Data.Components[0];
            var entity = com.Types.Skip(1).First();
            var script = com.ComponentImpl.Migrator.GetStep1(null, entity);

            var sqlCompiler = SqlCompillerBase.FormEnum(SqlDatabaseType.SqlServer);

            var result = sqlCompiler.Compile(script.Expression);

            _testOutputHelper.WriteLine(result);
        }
    }
}