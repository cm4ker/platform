using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Aquila.Metadata;
using Aquila.Runtime.Querying;

namespace Aquila.Core.Querying.Model
{
    /// <summary>
    /// Rebuild tree with security
    /// </summary>
    public class SecurityVisitor : QLangTreeRewriter
    {
        private readonly UserSecTable _sec;
        private QLang _m;
        private Dictionary<QDataSource, QDataSource> _subs = new();

        public List<QCriterion> _criteria = new();

        public SecurityVisitor(EntityMetadataCollection em, UserSecTable sec)
        {
            _m = new QLang(em);
            _sec = sec;
        }

        public override QLangElement VisitQQuery(QQuery arg)
        {
            var from = (QFrom)Visit(arg.From);
            var where = (QWhere)Visit(arg.Where);
            var groupby = (QGroupBy)Visit(arg.GroupBy);
            var having = (QHaving)Visit(arg.Having);
            var select = (QSelect)Visit(arg.Select);
            var orderby = (QOrderBy)Visit(arg.OrderBy);

            //Fill the criteria for this query
            var query = new QQuery(orderby, select, having, groupby, where, from,
                new QCriterionList(_criteria.ToImmutableArray()));
            //not use this criteria for next queries
            _criteria = new();

            return query;
        }

        private static Random random = new Random();

        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        void EmitCriteria(QAliasedDataSource ds, QObjectTable ot)
        {
            //this is object... transform it if has Criteria
            if (_sec.TryClaimPermission(ot.ObjectType, SecPermission.Read, out var claim))
            {
                //transform it to the nested query for checking rows inside the table
                ds.Substituted = true;

                _criteria = claim.Criteria.SelectMany(x => x.Value).Select(x =>
                {
                    _m.ld_ref(ds);
                    var c = x.cString;
                    QLang.Parse(_m, c);
                    return (QCriterion)_m.pop();
                }).ToList();
            }
            else
            {
                //Access denied
            }
        }

        public override QLangElement VisitQFrom(QFrom arg)
        {
            QDataSource resultDs;

            if (arg.Source is QObjectTable ot)
            {
                var ds = new QAliasedDataSource(ot, RandomString(10));
                EmitCriteria(ds, ot);
                _subs[ot] = ds;


                resultDs = ds;
            }
            else if (arg.Source is QAliasedDataSource { ParentSource: QObjectTable ot2 } ads)
            {
                EmitCriteria(ads, ot2);

                resultDs = arg.Source;
            }
            else
            {
                var source = (QDataSource)Visit(arg.Source);
                resultDs = source;
            }

            var joinList = (QJoinList)Visit(arg.Joins);

            return new QFrom(joinList, resultDs);
        }

        public override QLangElement VisitQObjectTable(QObjectTable arg)
        {
            //deny to create new objects in tree (we need old structure)

            if (_subs.TryGetValue(arg, out var newArg))
                return newArg;

            return arg;
        }

        public override QLangElement VisitQAliasedDataSource(QAliasedDataSource arg)
        {
            if (_subs.TryGetValue(arg, out var newArg))
                return newArg;

            var newDs = (QAliasedDataSource)base.VisitQAliasedDataSource(arg);
            _subs[arg] = newDs;

            return newDs;
        }

        public override QLangElement VisitQIntermediateSourceField(QIntermediateSourceField arg)
        {
            if (_subs.TryGetValue(arg.DataSource, out var subs))
            {
                return subs.GetFields().Where(x => x.GetName() == arg.GetName()).First();
            }

            return arg;
        }

        public override QLangElement VisitQSourceFieldExpression(QSourceFieldExpression arg)
        {
            if (_subs.TryGetValue(arg.PlatformSource, out var subs))
            {
                return subs.GetFields().Where(x => x.GetName() == arg.GetName()).First();
            }

            return arg;
        }

        public override QLangElement VisitQFromItem(QFromItem arg)
        {
            /*
             
             Transfrom
             
             FROM {Source1}
             JOIN  {Source2}
            
             TO
             
             FROM {Source1}
             JOIN (SELECT Fields FROM {Source2} {WithCriteria}) X             
             
             */

            QObjectTable ot = null;
            QAliasedDataSource ads = null;

            if (arg.Joined is QObjectTable tot)
            {
                ot = tot;
                ads = new QAliasedDataSource(tot, RandomString(10));
                _subs[arg.Joined] = ads;
            }
            else if (arg.Joined is QAliasedDataSource { ParentSource: QObjectTable tot2 } tads)
            {
                ot = tot2;
                ads = tads;
            }

            if (ot != null && ads != null)
            {
                EmitCriteria(ads, ot);

                var select = new QSelect(new QFieldList(ads.GetFields().ToImmutableArray()));
                var from = new QFrom(QJoinList.Empty, arg.Joined);

                //TODO: optimize selection in inner query (only used field needs remaining)
                var query = new QQuery(null, select, null, null, null, from,
                    new QCriterionList(_criteria.ToImmutableArray()));

                var newDs = new QAliasedDataSource(new QNestedQuery(query), ads.Alias);

                //erase the value for next emitting
                _criteria = new();

                _subs[ads] = newDs;

                //
                var condition = (QExpression)Visit(arg.Condition);
                return new QFromItem(condition, newDs, arg.JoinType);
            }

            return base.VisitQFromItem(arg);
        }
    }
}