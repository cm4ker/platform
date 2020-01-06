using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.QueryBuilder.Builders;

namespace ZenPlatform.Migration
{
    public class MigrationTaskAction : IMigrationTask
    {
        private Action<DDLQuery> _run;
        private Action<DDLQuery> _rollback;
        public MigrationTaskAction(int step, Action<DDLQuery> run, Action<DDLQuery> rollback, string name = "")
        {
            Step = step;
            _run = run;
            _rollback = rollback;
            Completed = false;
            Id = Guid.NewGuid();
            Name = name;
        }
        public int Step { get; }
        public bool Completed { get; private set; }
        public Guid Id { get; }
        public string Name { get; }

        public void Run(DDLQuery query) 
        {
            _run(query); 
            Completed = true;
        }

        public void RollBack(DDLQuery query) => _rollback(query);
    }
}
