using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Aquila.Metadata;
using Aquila.Runtime.Querying;

namespace Aquila.Core.Querying.Model
{
    public enum QueryContext
    {
        None = 0,
        From = 1,
        GroupBy = 2,
        Having = 3,
        Where = 4,
        Select = 5,
        OrderBy = 6
    }

    public class LogicScope
    {
        public LogicScope()
        {
            _aliasedDS = new Dictionary<string, QDataSource>();
            _scopedDS = ImmutableArray<QDataSource>.Empty;
            Criteria = new();
        }

        public QueryContext QueryContext;

        /// <summary>
        /// Names of datasources
        /// </summary>
        private Dictionary<string, QDataSource> _aliasedDS { get; }

        private ImmutableArray<QDataSource> _scopedDS;

        public List<QCriterion> Criteria { get; set; }


        /// <summary>
        /// Push data to the scope. Data from this DS will be available in this scope
        /// </summary>
        /// <param name="ds"></param>
        public void AddDS(QDataSource ds, string alias = "")
        {
            _scopedDS = _scopedDS.Add(ds);

            if (!string.IsNullOrEmpty(alias))
            {
                if (!_aliasedDS.TryAdd(alias, ds))
                {
                    throw new Exception($"ERROR: Name collision {alias}");
                }

                return;
            }

            if (ds is QAliasedDataSource ads)
            {
                if (!_aliasedDS.TryAdd(ads.Alias, ds))
                {
                    throw new Exception($"ERROR: Name collision {ads.Alias}");
                }
            }
        }

        public void RemoveDS(QDataSource ds)
        {
            _scopedDS = _scopedDS.Remove(ds);

            if (ds is QAliasedDataSource a)
            {
                _aliasedDS.Remove(a.Alias);
            }
        }

        public void ReplaceDS(QDataSource oldDS, QDataSource newDS)
        {
            RemoveDS(oldDS);
            AddDS(newDS);
        }

        public void RemoveDS(string alias)
        {
            _aliasedDS.Remove(alias, out var ds);
            _scopedDS = _scopedDS.Remove(ds);
        }

        public bool TryGetDS(string alias, out QDataSource ds)
        {
            return _aliasedDS.TryGetValue(alias, out ds);
        }

        public ImmutableArray<QDataSource> GetScopedDS() => _scopedDS;
    }
}