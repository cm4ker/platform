﻿namespace Aquila.QueryBuilder.Common
{
    public interface IAliasedNode
    {
        string Alias { get; }

        bool IsAliased { get; }
    }
}