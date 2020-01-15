using MoreLinq;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Xunit;
using Xunit.Abstractions;
using ZenPlatform.Compiler;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.ConfigurationExample;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.Builders;
using ZenPlatform.Test.Tools;

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
            var actual = ConfigurationFactory.Create();


            var savedTypes = actual.Data.ObjectTypes;


            var query = DDLQuery.New();
            savedTypes.ForEach(a => a.Parent.ComponentImpl.Migrator.GetStep1(null, a, query));
            savedTypes.ForEach(a => a.Parent.ComponentImpl.Migrator.GetStep2(null, a, query));
            savedTypes.ForEach(a => a.Parent.ComponentImpl.Migrator.GetStep3(null, a, query));
            savedTypes.ForEach(a => a.Parent.ComponentImpl.Migrator.GetStep4(null, a, query));


            var sqlCompiler = SqlCompillerBase.FormEnum(SqlDatabaseType.SqlServer);
            var result = sqlCompiler.Compile(query.Expression);
        }

        [Fact]
        public void MultiTest()
        {
            var actual = ConfigurationFactory.Create();


            var old = ConfigurationFactory.Create();

            var savedTypes = actual.Data.ObjectTypes;
            var dbTypes = old.Data.ObjectTypes;

            var types = dbTypes.FullJoin(savedTypes, x => x.Guid,
                x => new {component = x.Parent, old = x, actual = default(IXCObjectType)},
                x => new {component = x.Parent, old = default(IXCObjectType), actual = x},
                (x, y) => new {component = x.Parent, old = x, actual = y});

            var query = DDLQuery.New();
            types.ForEach(a => a.component.ComponentImpl.Migrator.GetStep1(a.old, a.actual, query));
            types.ForEach(a => a.component.ComponentImpl.Migrator.GetStep2(a.old, a.actual, query));
            types.ForEach(a => a.component.ComponentImpl.Migrator.GetStep3(a.old, a.actual, query));
            types.ForEach(a => a.component.ComponentImpl.Migrator.GetStep4(a.old, a.actual, query));


            var sqlCompiler = SqlCompillerBase.FormEnum(SqlDatabaseType.SqlServer);
            var result = sqlCompiler.Compile(query.Expression);
        }
    }
}