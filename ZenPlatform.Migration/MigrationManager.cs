
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Core.Assemblies;
using ZenPlatform.Data;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.Builders;

namespace ZenPlatform.Migration
{
    public class MigrationManager : IMigrationManager
    {
        private readonly IDataContextManager _dataContextManager;
        private readonly IAssemblyManager _assemblyManager;
        public MigrationManager(IDataContextManager dataContextManager, IAssemblyManager assemblyManager)
        {
            _assemblyManager = assemblyManager;
            _dataContextManager = dataContextManager;
        }

        public void Migrate(XCRoot old, XCRoot actual)
        {

            _assemblyManager.BuildConfiguration(actual, _dataContextManager.DatabaseType);

            var savedTypes = actual.Data.ComponentTypes;
            var dbTypes = old.Data.ComponentTypes;


            var types = dbTypes.FullJoin(savedTypes, x => x.Guid,
                x => new { component = x.Parent, old = x, actual = default(XCObjectTypeBase) },
                x => new { component = x.Parent, old = default(XCObjectTypeBase), actual = x },
                (x, y) => new { component = x.Parent, old = x, actual = y });

            var query = DDLQuery.New();

            types.ForEach(a => a.component.ComponentImpl.Migrator.GetStep1(a.old, a.actual, query));
            types.ForEach(a => a.component.ComponentImpl.Migrator.GetStep2(a.old, a.actual, query));
            types.ForEach(a => a.component.ComponentImpl.Migrator.GetStep3(a.old, a.actual, query));
            types.ForEach(a => a.component.ComponentImpl.Migrator.GetStep4(a.old, a.actual, query));


            var context = _dataContextManager.GetContext();
            
            using (var cmd = context.CreateCommand(query.Expression))
            {
                cmd.ExecuteNonQuery();
            }
            
        }


        /// <summary>
        /// Проверяет нужно ли выполнять миграцию, если False - то миграцию выполнять не нужно.
        /// </summary>
        /// <param name="old"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public bool CheckMigration(XCRoot old, XCRoot actual)
        {

            return old.GetHash() != actual.GetHash();
        }
    }
}
