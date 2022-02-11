using System;
using System.Collections;
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
        public Stack<List<QCriterion>> _criteriaStack = new();

        public SecurityVisitor(MetadataProvider em, UserSecTable sec)
        {
            _m = new QLang(em);
            _sec = sec;
        }


        public override QLangElement VisitQInsertSelectQuery(QInsertSelectQuery arg)
        {
            var target = arg.Insert.Target;
            if (target is QObjectTable ot)
            {
                if (_sec.TryClaimPermission(ot.ObjectType, SecPermission.Create, out var claim))
                {
                    var oldSelect = arg.Select;

                    var criteria = claim.Criteria.SelectMany(x => x.Value).Select(x =>
                    {
                        //make it substituted
                        oldSelect.From.Source.Substituted = true;

                        //force load source table
                        _m.ld_ref(oldSelect.From.Source);


                        var c = x.cString;
                        QLang.Parse(_m, c);
                        return (QCriterion)_m.pop();
                    }).ToImmutableArray();

                    var newSelect =
                        new QSelectQuery(null, oldSelect.Select, null, null, null, oldSelect.From,
                            new QCriterionList(criteria));

                    return new QInsertSelectQuery(newSelect, arg.Insert);
                }
            }

            return arg;
        }

        private IEnumerable<QCriterion> GetUpdateCriteria(QDataSource s1, QDataSource s2, UserSecPermission claim)
        {
            foreach (var criteria in claim.Criteria)
            {
                foreach (var criterion in criteria.Value)
                {
                    //1st load main table and make criterion and next load newvalues table and again make criterion

                    //make it substituted
                    s1.Substituted = true;

                    //force load source table
                    _m.ld_ref(s1);

                    QLang.Parse(_m, criterion.cString);
                    yield return (QCriterion)_m.pop();


                    //make it substituted
                    s2.Substituted = true;

                    //force load source table
                    _m.ld_ref(s1);

                    QLang.Parse(_m, criterion.cString);
                    yield return (QCriterion)_m.pop();
                }
            }
        }

        public override QLangElement VisitQUpdateQuery(QUpdateQuery arg)
        {
            var ot = arg.From.Source.Find<QObjectTable>().FirstOrDefault();
            var target = arg.From.Source;
            var newValues = arg.From.Joins[0].Joined;


            if (ot != null)
            {
                if (_sec.TryClaimPermission(ot.ObjectType, SecPermission.Update, out var claim))
                {
                    return new QUpdateQuery(arg.Update, arg.Set, arg.From, arg.Where,
                        new QCriterionList(GetUpdateCriteria(target, newValues, claim).ToImmutableArray()));
                }
            }

            return arg;
        }

        public override QLangElement VisitQDeleteQuery(QDeleteQuery arg)
        {
            var ot = arg.Delete.Target.Find<QObjectTable>().FirstOrDefault();
            var target = arg.Delete.Target;

            if (ot != null)
            {
                if (_sec.TryClaimPermission(ot.ObjectType, SecPermission.Delete, out var claim))
                {
                    var criteria = claim.Criteria.SelectMany(x => x.Value).Select(x =>
                    {
                        var sub = arg.Delete.Target;
                        sub.Substituted = true;
                        _m.ld_ref(sub);
                        var c = x.cString;
                        QLang.Parse(_m, c);
                        return (QCriterion)_m.pop();
                    }).ToImmutableArray();

                    return new QDeleteQuery(arg.Delete, arg.From, arg.Where, new QCriterionList(criteria));
                }
            }

            return arg;
        }

        public override QLangElement VisitQInsertQuery(QInsertQuery arg)
        {
            /*
            TRANSFORM 
            INSERT INTO Table(ColList) VALUES(@p1, @p2... @pn)
            
            into
            
            INSERT INTO Table(ColList)
            SELECT *
            FROM (SELECT @p1 AS NameField1, @p2 AS NameField2, @p3 AS NameField3 ..) AS TS
            WHERE            
             Criteria
             */

            var target = arg.Insert.Target;
            if (target is QObjectTable ot)
            {
                if (_sec.TryClaimPermission(ot.ObjectType, SecPermission.Create, out var claim))
                {
                    var select = new QSelect(new QFieldList(arg.Values[0]
                        .Select(x => (QField)new QAliasedSelectExpression(x, ((QParameter)x).Name))
                        .ToImmutableArray()));

                    var nq = new QAliasedDataSource(new QNestedQuery(new QSelectQuery(null, select, null, null, null,
                        null,
                        QCriterionList.Empty)), "TS");

                    var criteria = claim.Criteria.SelectMany(x => x.Value).Select(x =>
                    {
                        _m.ld_ref(nq);
                        var c = x.cString;
                        QLang.Parse(_m, c);
                        return (QCriterion)_m.pop();
                    }).ToImmutableArray();

                    var q = new QSelectQuery(null, new QSelect(new QFieldList(nq.GetFields().ToImmutableArray())), null,
                        null, null, new QFrom(null, nq), new QCriterionList(criteria));

                    return new QInsertSelectQuery(q, arg.Insert);
                }
            }

            return arg;
        }

        public override QLangElement VisitQSelectQuery(QSelectQuery arg)
        {
            var from = (QFrom)Visit(arg.From);
            var where = (QWhere)Visit(arg.Where);
            var groupby = (QGroupBy)Visit(arg.GroupBy);
            var having = (QHaving)Visit(arg.Having);
            var select = (QSelect)Visit(arg.Select);
            var orderby = (QOrderBy)Visit(arg.OrderBy);

            if (!_criteriaStack.TryPop(out var list))
                list = new();

            //Fill the criteria for this query
            var query = new QSelectQuery(orderby, select, having, groupby, where, from,
                new QCriterionList(list.ToImmutableArray()));

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

                _criteriaStack.Push(claim.Criteria.SelectMany(x => x.Value).Select(x =>
                {
                    _m.ld_ref(ds);
                    var c = x.cString;
                    QLang.Parse(_m, c);
                    return (QCriterion)_m.pop();
                }).ToList());
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
                //TODO: potential here can be a collision. Need create deterministic randomizer witout collisio
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
                return subs.GetField(arg.GetName());
            }

            return arg;
        }

        public override QLangElement VisitQSourceFieldExpression(QSourceFieldExpression arg)
        {
            if (_subs.TryGetValue(arg.PlatformSource, out var subs))
            {
                return subs.GetField(arg.GetName());
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
                var query = new QSelectQuery(null, select, null, null, null, from,
                    new QCriterionList(_criteriaStack.Pop().ToImmutableArray()));

                var newDs = new QAliasedDataSource(new QNestedQuery(query), ads.Alias);

                _subs[ads] = newDs;

                //
                var condition = (QExpression)Visit(arg.Condition);
                return new QFromItem(condition, newDs, arg.JoinType);
            }

            return base.VisitQFromItem(arg);
        }
    }
}