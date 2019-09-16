using System;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ServiceStack.Script;
using Xunit;
using Xunit.Abstractions;
using ZenPlatform.Compiler;
using ZenPlatform.QueryBuilder;
using ZenPlatform.Tests.Common;

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
            var scripts = com.ComponentImpl.Migrator.GetScript(null, entity);

            var sqlCompiler = new PostgresCompiller();

            var builder = new StringBuilder();

            foreach (var script in scripts)
            {
                var result = sqlCompiler.Compile(script);
                builder.Append(result);
                builder.Append("\n\n");
            }

            _testOutputHelper.WriteLine(builder.ToString());
        }
    }
}