using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Aquila.Core.Querying.Model;
using Aquila.Core.Querying.Optimizers;
using Aquila.Metadata;
using Aquila.Migrations;
using Aquila.QueryBuilder;
using Aquila.Runtime;
using Aquila.Runtime.Querying;

namespace Aquila.Core.Querying
{
    /// <summary>
    /// Visit logical tree of query and build query for real DBMS 
    /// </summary>
    public class SelectionRealWalker : RealWalkerBase
    {
        private bool _hasNamedSource = false;
        private bool _hasAlias = false;

        /// <inheritdoc />
        public SelectionRealWalker(DatabaseRuntimeContext drContext) : base(drContext)
        {
        }

        /// <inheritdoc />
        public SelectionRealWalker(DatabaseRuntimeContext drContext, QueryMachine qm) : base(drContext, qm)
        {
        }


        public override void VisitQParameter(QParameter arg)
        {
            Qm.ld_param(arg.GetDbName());
        }

        public override void VisitQSelectQuery(QSelectQuery node)
        {
            Qm.bg_query();

            Visit(node.From);
            Visit(node.Where);
            Visit(node.GroupBy);
            Visit(node.OrderBy);
            Visit(node.Select);

            VisitQCriterionList(node.Criteria, node.From);

            Qm.st_query();
        }


        public override void VisitQCast(QCast node)
        {
            TypedExprFactory.CreateSingleTypeExpr(node, Qm, this).Emit();
        }

        public override void VisitQSelect(QSelect node)
        {
            Qm.m_select();

            base.VisitQSelect(node);

            _hasAlias = false;
            _hasNamedSource = false;
        }

        public void VisitQCriterionList(QCriterionList arg, QFrom from)
        {
            //NOTE: if have not from block then we haven't data source => we can't apply criterion
            if (from == null) return;

            //if no criteria then we move forward
            if (arg == null || !arg.Any())
            {
                //capture all source names where we need take a _sec filed
                //in the feature we build resulting toplevel _sec field
                List<string> tlSec = new();

                if (from.Source.HasInternalCriterion)
                {
                    tlSec.Add(from.Source.GetDbName());
                }

                if (from.Joins != null)
                {
                    //if on general table we have not a criteria but we have it on joined

                    foreach (var fromItem in from.Joins)
                    {
                        if (fromItem.Joined.HasInternalCriterion)
                        {
                            tlSec.Add(fromItem.Joined.GetDbName());
                        }
                    }
                }

                if (tlSec.Any())
                {
                    if (tlSec.Count() == 1)
                    {
                        Qm.ld_str(tlSec.First())
                            .ld_str("_sec")
                            .ld_column();
                    }
                    else
                    {
                        var isFirst = true;

                        //total result expression
                        Qm.ld_const(1);

                        foreach (var sec in tlSec)
                        {
                            Qm.ld_str(sec)
                                .ld_str("_sec")
                                .ld_column()
                                .ld_const(1)
                                .eq();

                            if (isFirst)
                            {
                                isFirst = false;
                                continue;
                            }

                            Qm.and();
                        }

                        Qm.when();
                        Qm.ld_const(0);
                        Qm.@case()
                            .@as("_sec");
                    }
                }

                return;
            }

            var emitOr = false;

            //expr
            Qm.ld_const(1);

            foreach (var item in arg)
            {
                //condition
                VisitQCriterion(item);
                Qm.exists();

                if (emitOr)
                    Qm.or();

                emitOr = true;
            }

            if (from.Joins != null)
                foreach (var fromItem in from.Joins)
                {
                    if (fromItem.Joined.HasInternalCriterion)
                    {
                        LoadNamedSource(fromItem.Joined.GetDbName());

                        Qm.ld_str("_sec")
                            .ld_column()
                            .ld_const(1)
                            .is_null()
                            .ld_const(1)
                            .eq()
                            .and();
                    }
                }

            Qm.when();

            //else
            Qm.ld_const(0);
            Qm.@case();

            Qm.@as("_sec");
        }

        public override void VisitQCriterion(QCriterion arg)
        {
            Qm.bg_query();
            Qm.m_from();
            Qm.bg_query().m_select().ld_const(1).@as("_sec_fld").st_query().@as("_sec_dummy");

            Visit(arg.From.Joins);
            Visit(arg.Where);

            Qm.m_select();
            Qm.ld_const(1);

            Qm.st_query();
        }

        public override void VisitQObjectTable(QObjectTable node)
        {
            var ot = node.ObjectType;

            //Inject data source - the idea
            Qm.ld_table(ot.GetDescriptor(DrContext).DatabaseName);

            if (!_hasAlias)
                Qm.@as(node.GetDbName());


            _hasAlias = false;

            base.VisitQObjectTable(node);
        }

        public override void VisitQTable(QTable node)
        {
            if (!_hasAlias)
                Qm.@as(node.GetDbName());

            _hasAlias = false;
        }

        public override void VisitQAdd(QAdd node)
        {
            if (!OptimizeOperation(node, () => Qm.eq(), () => Qm.and()))
            {
                base.VisitQAdd(node);
                Qm.add();
            }
        }

        public override void VisitQEquals(QEquals node)
        {
            if (!OptimizeOperation(node, () => Qm.eq(), () => Qm.and()))
            {
                base.VisitQEquals(node);
                Qm.eq();
            }
        }

        public override void VisitQNotEquals(QNotEquals node)
        {
            if (!OptimizeOperation(node, () => Qm.ne(), () => Qm.or()))
            {
                base.VisitQNotEquals(node);
                Qm.ne();
            }
        }

        private List<SMType> CommonTypes(List<SMType> types1, List<SMType> types2)
        {
            var result = new List<SMType>();
            foreach (var t1 in types1)
            {
                foreach (var t2 in types2)
                {
                    if (t1.IsAssignableFrom(t2))
                    {
                        result.Add(t1);
                    }
                }
            }

            return result;
        }

        public bool OptimizeOperation(QOperationExpression op, Action compareAction, Action concatAction)
        {
            var leftTypes = op.Left.GetExpressionType().ToList();
            var rightTypes = op.Right.GetExpressionType().ToList();

            if (!leftTypes.Any() || !rightTypes.Any())
            {
                throw new Exception("Can't optimize expression with empty types");
            }

            if (leftTypes.Count == 1 && rightTypes.Count == 1)
            {
                return false;
            }

            if (leftTypes.Count > 1 && rightTypes.Count > 1)
            {
                MultiTypedExpr left = TypedExprFactory.CreateMultiTypedExpr(op.Left, Qm, this);
                MultiTypedExpr right = TypedExprFactory.CreateMultiTypedExpr(op.Right, Qm, this);

                var commonTypes = CommonTypes(leftTypes, rightTypes);

                var refEmitted = false;
                bool hasCompareTop = false;
                foreach (var type in commonTypes)
                {
                    if (type.IsPrimitive)
                    {
                        left.EmitValueColumn(type);
                        right.EmitValueColumn(type);
                    }

                    if (!refEmitted && type.IsReference)
                    {
                        left.EmitRefColumn();
                        right.EmitRefColumn();

                        refEmitted = true;
                    }

                    compareAction();

                    if (hasCompareTop)
                        concatAction();

                    hasCompareTop = true;
                }

                left.EmitTypeColumn();
                right.EmitTypeColumn();
                compareAction();
                concatAction();
            }
            else if (leftTypes.Count == 1 && rightTypes.Count > 1)
            {
                IfLeftOrRightOneType(op.Left, op.Right, compareAction, concatAction, leftTypes, true);
            }
            else if (rightTypes.Count == 1 && leftTypes.Count > 1)
            {
                IfLeftOrRightOneType(op.Right, op.Left, compareAction, concatAction, rightTypes);
            }


            return true;
        }

        void IfLeftOrRightOneType(QExpression left, QExpression right, Action compareAction, Action concatAction,
            List<SMType> leftTypes,
            bool flip = false)
        {
            var mt = TypedExprFactory.CreateMultiTypedExpr(right, Qm, this);

            var leftType = leftTypes[0];

            if (flip)
                Visit(left);

            if (leftType.IsPrimitive)
            {
                mt.EmitValueColumn(leftType);
            }
            else if (leftType.IsReference)
            {
                mt.EmitRefColumn();
            }

            if (!flip)
                Visit(left);

            compareAction();

            if (flip)
                Qm.ld_const(leftType.GetTypeId(DrContext));

            mt.EmitTypeColumn();

            if (!flip)
                Qm.ld_const(leftType.GetTypeId(DrContext));
            compareAction();

            concatAction();
        }

        public override void VisitQFrom(QFrom node)
        {
            Qm.m_from();

            Visit(node.Source);

            //analyse join
            if (node.Joins != null)
                foreach (var nodeJoin in node.Joins)
                {
                    VisitQFromItem(nodeJoin);
                }
        }

        public override void VisitQGroupBy(QGroupBy arg)
        {
            Qm.m_group_by();
            Visit(arg.Expressions);
        }

        public override void VisitQWhere(QWhere node)
        {
            Qm.m_where();

            Visit(node.Expression);
        }

        public override void VisitQOrderBy(QOrderBy arg)
        {
            Qm.m_order_by();

            Visit(arg.Expressions);
        }

        public override void VisitQOrderExpression(QOrderExpression arg)
        {
            Visit(arg.Expression);
            if (arg.SortingDirection == QSortDirection.Descending)
                Qm.desc();
            else
                Qm.asc();
        }

        public override void VisitQConst(QConst node)
        {
            string alias = "";
            if (_hasAlias)
                alias = (string)Qm.pop();
            Qm.ld_const(node.Value);

            if (_hasAlias)
                Qm.@as(alias);
            _hasAlias = false;
        }

        public override void VisitQFromItem(QFromItem node)
        {
            Visit(node.Joined);
            Visit(node.Condition);

            Qm.@join();
        }

        public override void VisitQAliasedDataSource(QAliasedDataSource node)
        {
            _hasAlias = true;

            base.VisitQAliasedDataSource(node);

            Qm.@as(node.GetDbName());
        }

        private void LoadNamedSource(string arg)
        {
            if (_hasNamedSource) return;

            Qm.ld_str(arg);
            _hasNamedSource = true;
        }

        public override void VisitQIntermediateSourceField(QIntermediateSourceField node)
        {
            //NOTE: Intermidiate field representation contextual source change
            //For example from NotAliasedObject to Aliased
            //From NestedQuery to FromItem

            LoadNamedSource(node.DataSource.GetDbName());

            if (node.DataSource is QAliasedDataSource ads)
            {
                base.VisitQIntermediateSourceField(node);
            }
            else if (node.DataSource is QNestedQuery)
            {
                // var schema = _tm.GetPropertySchemas(node.GetDbName(), node.GetExpressionType().ToList());
                // GenColumn(schema);
            }
        }

        public override void VisitQNestedQueryField(QNestedQueryField node)
        {
            LoadNamedSource(node.DataSource.GetDbName());

            if (node.DataSource is QAliasedDataSource ads)
            {
                base.VisitQNestedQueryField(node);
            }
            else if (node.DataSource is QNestedQuery)
            {
                var types = node.GetExpressionType().ToList();

                if (types.Any())
                {
                    var schema = DRContextHelper.GetPropertySchemas(node.GetDbName(), types);
                    GenColumn(schema);
                }
                else
                {
                    //possible it parameter. it has not types
                    //we can't predict what user passthrough
                    //just try to render it AS IS 

                    string tabName = null;
                    string alias = null;

                    if (_hasNamedSource)
                        tabName = (string)Qm.pop();

                    var columnName = "";

                    if (node.Field is QAliasedSelectExpression)
                        columnName = node.GetDbName();
                    else
                        columnName = node.Field.GetDbName();

                    Qm.ld_column(columnName, tabName);

                    _hasNamedSource = false;
                    _hasAlias = false;
                }
            }
        }

        public override void VisitQSourceFieldExpression(QSourceFieldExpression node)
        {
            var schema = node.Property.GetSchema(DrContext);

            LoadNamedSource(node.PlatformSource.GetDbName());

            GenColumn(schema);
        }

        private void GenColumn(IEnumerable<ColumnSchemaDefinition> schema)
        {
            string tabName = null;
            string alias = null;

            if (_hasNamedSource)
                tabName = (string)Qm.pop();

            if (_hasAlias)
                alias = (string)Qm.pop();

            foreach (var def in schema)
            {
                Qm.ld_str(tabName);
                Qm.ld_str(def.FullName);
                Qm.ld_column();

                if (_hasAlias)
                    Qm.@as(def.Prefix + alias + def.Postfix);
            }

            _hasNamedSource = false;
            _hasAlias = false;
        }

        public override void VisitQAliasedSelectExpression(QAliasedSelectExpression node)
        {
            _hasAlias = true;
            Qm.ld_str(node.GetDbName());

            base.VisitQAliasedSelectExpression(node);

            if (_hasAlias) //alias not handled
            {
                var item = Qm.pop(); //item
                Qm.pop(); //alias
                Qm.push(item);
                Qm.@as(node.GetDbName());
            }
        }
    }
}