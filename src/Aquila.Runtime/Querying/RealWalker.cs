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

namespace Aquila.Core.Querying
{
    /*
     Invoice
     (
        MyProperty  (int, string, ContractRef)
        MyProperty2 (int) = 0;

        struct Prop10
        {
            public int Type;
            public int Int;
            public string String;
            public ContractLink ContractRef;
        }
       
        Invoice.MyProperty
     )
          
     1) Сопоставить типы обеих сторон
        String        String
        Int           Int
        Data
     2) 
     
     A = CASE 1 > 0 THEN A ELSE B (Types(A) + Types(B))
      V
     AI = BI
     AND AS = BS
     AND AD = BD
     AND AB = BB
     AND AType = BType 
     
     E1:
     A (int, string) = B (int, string) => 
     
     A.Type = B.Type
     AND A.Int = B.Int
     AND A.String = B.String
     
     E2:
     A(int, string) = B(int) =>
     
     A.Type = 1
     AND A.Int = B.Int
     
     E3:
     Cast(A (int, string) as int) = B (int, string) => 
     
     CASE When A.Type = 1 THEN A.Int
          When A.Type = 2 THEN CAST(A.String AS int) END = B.Int
     
     E4:
     Case 1 > n Then A (int, string) 
                Else B (date, string, Ref) End = C (int, Ref) => 
          
     Case 1 > n Then
                    A.Type
                Else
                    B.Type 
     End = C.Type
     AND     
     Case 1 > n Then
                    A.Int
                Else
                    default(int) 
     End = C.Int
     AND
     Case 1 > n Then
                    default(ref)
                Else
                    B.Ref End = C.Ref
     End
          
     E5:
     Cast(Case 1 > n Then A (int, string) 
                Else B (date, string, Ref) End as int) = C (int, Ref) =>    
     
     Case when 1 > n Then 
                        Case
                            When A.Type = 1 Then A.Int
                            When A.Type = 2 Then CAST(A.String AS int) 
                        End 
                     Else
                      Cast(B.String AS int)
     End = C.Int
     */
    /// <summary>
    /// Обходит дерево логического запроса и строит на его основе дерево реального SQL зарпоса 
    /// </summary>
    public class RealWalker : QLangWalker
    {
        private readonly DatabaseRuntimeContext _drContext;
        private QueryMachine _qm;
        private StringWriter _l;
        private bool _hasNamedSource = false;
        private bool _hasAlias = false;


        public string Log => _l.ToString();

        internal DatabaseRuntimeContext DrContext => _drContext;

        private List<SMSecPolicy> _polices = new();

        /*
        
            // 1. We can throw the SecException on construct query if user haven't access to the table
            // 2. We need create SecTable
        
            UserSecTable
            Object\Permission        | Create | Update | Read | Delete |
                                     +--------+--------+------+--------+
                            Store    |   x    |        |  x   |   x*   | 
                                     |________+________+______+________+
                            Invoice  |        |   x*   |      |        |
                                     |________+________+______+________+
                            Document |   x    |   x    |  x   |   x    |
                                     |________+________+______+________+
            
            Where x* - is permission with lookup query (more complex case) NOTE: Lookup queries can be more than one
         */

        private class UserSecPermission
        {
            public static Dictionary<SMType, UserSecPermission> FromPolicy(SMSecPolicy policy)
            {
                var subjects = policy.Subjects;
                var criteria = policy.Criteria;

                var result = new Dictionary<SMType, UserSecPermission>();

                var a = (from s in subjects
                        join c in criteria
                            on s.Subject equals c.Subject
                            into tmp
                        from t in tmp.DefaultIfEmpty()
                        group new { Sbuject = s.Subject, Permission = s.Permission, Criteria = tmp.ToList() } by s
                            .Subject
                    );

                foreach (var t in a)
                {
                    var up = new UserSecPermission();

                    foreach (var values in t)
                    {
                        up.Permission |= values.Permission;

                        foreach (var criterion in values.Criteria)
                        {
                            up.Criteria[criterion.Permission].Add(criterion.Query);
                        }
                    }

                    result[t.Key] = up;
                }

                // TODO: this is primitive algo for creating user sec permissions dictionary
                // we need more complex algo for this
                // 1. Check for the same criterion query
                // 2. Merge UserSecPermission then we have many secs;

                return result;
            }

            public SecPermission Permission { get; set; }

            public Dictionary<SecPermission, List<string>> Criteria { get; set; }
        }

        private class UserSecTable
        {
            private Dictionary<SMType, UserSecPermission> _rows;

            public UserSecTable(List<SMSecPolicy> polices)
            {
            }

            public UserSecPermission this[SMType type]
            {
                get { return _rows[type]; }
            }

            public bool TryClaimSec(SMType type, out UserSecPermission permission)
            {
                permission = this[type];
                return permission != null;
            }

            public bool TryClaimPermission(SMType type, SecPermission permission, out UserSecPermission claim)
            {
                var result = this[type];
                if (result != null)
                {
                    claim = result;
                    if (permission.HasFlag(SecPermission.Create) && !result.Permission.HasFlag(SecPermission.Create))
                    {
                        claim = null;
                        return false;
                    }

                    if (permission.HasFlag(SecPermission.Read) && !result.Permission.HasFlag(SecPermission.Read))
                    {
                        claim = null;
                        return false;
                    }

                    if (permission.HasFlag(SecPermission.Update) && !result.Permission.HasFlag(SecPermission.Update))
                    {
                        claim = null;
                        return false;
                    }

                    if (permission.HasFlag(SecPermission.Delete) && !result.Permission.HasFlag(SecPermission.Delete))
                    {
                        claim = null;
                        return false;
                    }
                }
                else
                {
                    claim = null;
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Create instance of real walker class
        /// </summary>
        /// <param name="drContext"></param>
        public RealWalker(DatabaseRuntimeContext drContext)
        {
            _drContext = drContext;

            _qm = new QueryMachine();
            _l = new StringWriter();
        }

        public QueryMachine QueryMachine => _qm;

        public override void VisitQParameter(QParameter arg)
        {
            _qm.ld_param(arg.GetDbName());
        }

        public override void VisitQQuery(QQuery node)
        {
            _qm.bg_query();
            _l.WriteLine("ct_query");

            Visit(node.From);
            Visit(node.Where);
            Visit(node.GroupBy);
            Visit(node.OrderBy);
            Visit(node.Select);

            _qm.st_query();
            _l.WriteLine("st_query");
        }


        public override void VisitQCast(QCast node)
        {
            TypedExprFactory.CreateSingleTypeExpr(node, _qm, this).Emit();
        }

        public override void VisitQSelect(QSelect node)
        {
            _qm.m_select();
            _l.WriteLine("m_select");

            base.VisitQSelect(node);
        }

        public override void VisitQObjectTable(QObjectTable node)
        {
            var ot = node.ObjectType;

            //Inject data source - the idea
            _qm.ld_table(ot.GetDescriptor(_drContext).DatabaseName);

            if (!_hasAlias)
                _qm.@as(node.GetDbName());

            _hasAlias = false;

            base.VisitQObjectTable(node);
        }

        public override void VisitQTable(QTable node)
        {
            if (!_hasAlias)
                _qm.@as(node.GetDbName());

            _hasAlias = false;
        }

        public override void VisitQAdd(QAdd node)
        {
            if (!OptimizeOperation(node, () => _qm.eq(), () => _qm.and()))
            {
                base.VisitQAdd(node);
                _qm.add();
            }
        }

        public override void VisitQEquals(QEquals node)
        {
            if (!OptimizeOperation(node, () => _qm.eq(), () => _qm.and()))
            {
                base.VisitQEquals(node);
                _qm.eq();
            }
        }

        public override void VisitQNotEquals(QNotEquals node)
        {
            if (!OptimizeOperation(node, () => _qm.ne(), () => _qm.or()))
            {
                base.VisitQNotEquals(node);
                _qm.ne();
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
                MultiTypedExpr left = TypedExprFactory.CreateMultiTypedExpr(op.Left, _qm, this);
                MultiTypedExpr right = TypedExprFactory.CreateMultiTypedExpr(op.Right, _qm, this);

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
            var mt = TypedExprFactory.CreateMultiTypedExpr(right, _qm, this);

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
                _qm.ld_const(leftType.GetTypeId(_drContext));

            mt.EmitTypeColumn();

            if (!flip)
                _qm.ld_const(leftType.GetTypeId(_drContext));
            compareAction();

            concatAction();
        }

        public override void VisitQFrom(QFrom node)
        {
            _qm.m_from();
            _l.WriteLine("m_from");

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
            _qm.m_group_by();
            Visit(arg.Expressions);
        }

        public override void VisitQWhere(QWhere node)
        {
            _qm.m_where();
            _l.WriteLine("m_where");

            Visit(node.Expression);
        }

        public override void VisitQOrderBy(QOrderBy arg)
        {
            _qm.m_order_by();
            _l.WriteLine("m_order_by");

            Visit(arg.Expressions);
        }

        public override void VisitQOrderExpression(QOrderExpression arg)
        {
            Visit(arg.Expression);
            if (arg.SortingDirection == QSortDirection.Descending)
                _qm.desc();
            else
                _qm.asc();
        }

        public override void VisitQConst(QConst node)
        {
            string alias = "";
            if (_hasAlias)
                alias = (string)_qm.pop();
            _qm.ld_const(node.Value);

            if (_hasAlias)
                _qm.@as(alias);
            _hasAlias = false;
        }

        public override void VisitQFromItem(QFromItem node)
        {
            Visit(node.Joined);
            Visit(node.Condition);

            _qm.@join();
        }

        public override void VisitQAliasedDataSource(QAliasedDataSource node)
        {
            _hasAlias = true;

            base.VisitQAliasedDataSource(node);

            _qm.@as(node.GetDbName());
        }

        private void LoadNamedSource(string arg)
        {
            if (_hasNamedSource) return;

            _qm.ld_str(arg);
            _hasNamedSource = true;
        }

        public override void VisitQIntermediateSourceField(QIntermediateSourceField node)
        {
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
                var schema = DRContextHelper.GetPropertySchemas(node.GetDbName(), node.GetExpressionType().ToList());
                GenColumn(schema);
            }
        }

        public override void VisitQSourceFieldExpression(QSourceFieldExpression node)
        {
            var schema = node.Property.GetSchema(_drContext);

            LoadNamedSource(node.PlatformSource.GetDbName());

            GenColumn(schema);
        }

        private void GenColumn(IEnumerable<ColumnSchemaDefinition> schema)
        {
            string tabName = null;
            string alias = null;

            if (_hasNamedSource)
                tabName = (string)_qm.pop();

            if (_hasAlias)
                alias = (string)_qm.pop();

            foreach (var def in schema)
            {
                _qm.ld_str(tabName);
                _qm.ld_str(def.FullName);
                _qm.ld_column();

                if (_hasAlias)
                    _qm.@as(def.Prefix + alias + def.Postfix);
            }

            _hasNamedSource = false;
            _hasAlias = false;
        }

        public override void VisitQAliasedSelectExpression(QAliasedSelectExpression node)
        {
            _hasAlias = true;
            _qm.ld_str(node.GetDbName());

            base.VisitQAliasedSelectExpression(node);

            if (_hasAlias) //alias not handled
            {
                var item = _qm.pop(); //item
                _qm.pop(); //alias
                _qm.push(item);
                _qm.@as(node.GetDbName());
            }
        }
    }
}