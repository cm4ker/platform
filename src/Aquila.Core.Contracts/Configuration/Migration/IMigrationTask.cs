using System;
using Aquila.QueryBuilder.Builders;

namespace Aquila.Core.Contracts.Configuration.Migration
{
    public interface IMigrationTask
    {
        bool Completed { get; }
        Guid Id { get; }
        int Step { get; }
        string Name { get; }

        void RollBack(DDLQuery query);
        void Run(DDLQuery query);
    }
}