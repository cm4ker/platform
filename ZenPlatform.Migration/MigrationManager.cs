using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration.Contracts;
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
        private readonly IConfigurationManipulator _m;

        public MigrationManager(IDataContextManager dataContextManager, IAssemblyManager assemblyManager,
            IConfigurationManipulator m)
        {
            _assemblyManager = assemblyManager;
            _m = m;
            _dataContextManager = dataContextManager;
        }

        public void Migrate(IXCRoot old, IXCRoot actual)
        {
            _assemblyManager.BuildConfiguration(actual, _dataContextManager.DatabaseType);

            var savedTypes = actual.Data.ComponentTypes;
            var dbTypes = old.Data.ComponentTypes;


            var types = dbTypes.FullJoin(savedTypes, x => x.Guid,
                x => new {component = x.Parent, old = x, actual = default(IXCObjectType)},
                x => new {component = x.Parent, old = default(IXCObjectType), actual = x},
                (x, y) => new {component = x.Parent, old = x, actual = y});

            var query = DDLQuery.New();

            types.ForEach(a => a.component.ComponentImpl.Migrator.GetStep1(a.old, a.actual, query));
            types.ForEach(a => a.component.ComponentImpl.Migrator.GetStep2(a.old, a.actual, query));
            types.ForEach(a => a.component.ComponentImpl.Migrator.GetStep3(a.old, a.actual, query));
            types.ForEach(a => a.component.ComponentImpl.Migrator.GetStep4(a.old, a.actual, query));


            var context = _dataContextManager.GetContext();

            using (var cmd = context.CreateCommand(query.Expression))
            {
                if (!string.IsNullOrEmpty(cmd.CommandText))
                    cmd.ExecuteNonQuery();
            }
        }


        /// <summary>
        /// Проверяет нужно ли выполнять миграцию, если False - то миграцию выполнять не нужно.
        /// </summary>
        /// <param name="old"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public bool CheckMigration(IXCRoot old, IXCRoot actual)
        {
            return !_m.Equals(old, actual);
        }
    }
}