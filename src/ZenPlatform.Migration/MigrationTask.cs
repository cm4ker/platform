using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.Contracts.Migration;
using ZenPlatform.QueryBuilder.Builders;

namespace ZenPlatform.Migration
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
