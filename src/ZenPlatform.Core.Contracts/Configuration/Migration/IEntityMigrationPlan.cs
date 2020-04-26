using System;
using System.Collections.Generic;

namespace ZenPlatform.Configuration.Contracts.Migration
{
    public interface IEntityMigrationPlan
    {
        void AddScope(Action<IEntityMigrationScope> scopeAction, int priority);
        void AddScope(IEntityMigrationScope scope, int priority);
    }
}