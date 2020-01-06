using System;
using ZenPlatform.QueryBuilder.Builders;

namespace ZenPlatform.Configuration.Contracts.Data
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