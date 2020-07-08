using System;
using System.Collections.Generic;
using System.Text;
using Aquila.Core.Contracts.Configuration.Migration;
using Aquila.QueryBuilder.Builders;

namespace Aquila.Migrations
{
    public class MigrationTaskAction : IMigrationTask
    {
        public EntityMigrationScope MigrationPlan { get; }
        public EntityMigrationScope RollBackPlan { get; }
        public MigrationTaskAction(int step, string name = "")
        {
            Step = step;
            Completed = false;
            Id = Guid.NewGuid();
            Name = name;
            MigrationPlan = new EntityMigrationScope();
            RollBackPlan = new EntityMigrationScope();
        }
        public int Step { get; }
        public bool Completed { get; private set; }
        public Guid Id { get; }
        public string Name { get; }

        public void Run(DDLQuery query) 
        {
            var builder = new EntityMigrationPlanSQLBuilder(Guid.Empty);

            builder.Build(MigrationPlan, query);
            
            Completed = true;
        }

        public void RollBack(DDLQuery query)
        {
            var builder = new EntityMigrationPlanSQLBuilder(Guid.Empty);

            builder.Build(RollBackPlan, query);

        }
    }
}
