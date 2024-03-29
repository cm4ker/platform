﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Aquila.Migrations
{
    public class EntityMigrationPlan : IEnumerable<IEntityMigrationItem>
    {
        protected class EntityMigrationScopeItem
        {
            public EntityMigrationScope Scope { get; set; }
            public int Priority { get; set; }
        }

        private List<EntityMigrationScopeItem> _plan;

        protected IEnumerable<EntityMigrationScopeItem> GetPlan() => _plan;

        public EntityMigrationPlan()
        {
            _plan = new List<EntityMigrationScopeItem>();
        }

        public void AddPlan(EntityMigrationPlan plan)
        {
            _plan.AddRange(plan.GetPlan());
        }

        public void AddScope(EntityMigrationScope scope, int priority)
        {
            var item = new EntityMigrationScopeItem()
            {
                Scope = scope,
                Priority = priority
            };

            _plan.Add(item);
        }

        public void AddScope(Action<EntityMigrationScope> scopeAction, int priority)
        {
            var scope = new EntityMigrationScope();
            scopeAction(scope);

            var item = new EntityMigrationScopeItem()
            {
                Scope = scope,
                Priority = priority
            };

            _plan.Add(item);
        }

        public IEnumerator GetEnumerator()
        {
            return _plan.OrderBy(o => o.Priority).SelectMany(s => s.Scope).GetEnumerator();
        }

        IEnumerator<IEntityMigrationItem> IEnumerable<IEntityMigrationItem>.GetEnumerator()
        {
            return (IEnumerator<IEntityMigrationItem>)GetEnumerator();
        }
    }
}