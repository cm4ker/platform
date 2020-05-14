using System;

namespace Aquila.Core.Contracts.Configuration.Migration
{
    public interface IEntityMigrationPlan
    {
        void AddScope(Action<IEntityMigrationScope> scopeAction, int priority);
        void AddScope(IEntityMigrationScope scope, int priority);
    }
}