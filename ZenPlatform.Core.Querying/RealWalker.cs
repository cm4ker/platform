using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dnlib.DotNet;
using ZenPlatform.Configuration.Structure.Data.Types;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using ZenPlatform.Core.Querying.Model;
using ZenPlatform.Language.Ast.Infrastructure;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Core.Querying
{
    /*
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
    public class RealWalker : QLangWalker
    {
        private QueryMachine _qm;
        private StringWriter _l;
        private bool _hasNamedSource = false;
        private bool _hasAlias = false;

        public string Log => _l.ToString();

        public RealWalker()
        {
            _qm = new QueryMachine();
            _l = new StringWriter();
        }

        public QueryMachine QueryMachine => _qm;


        public override object VisitQQuery(QQuery node)
        {
            _qm.bg_query();
            _l.WriteLine("ct_query");

            Visit(node.From);
            Visit(node.Select);

            _qm.st_query();
            _l.WriteLine("st_query");

            return null;
        }

        public override object VisitQCast(QCast node)
        {
            base.VisitQCast(node);

            _qm.ld_col_type("string");
            _qm.cast();

            return null;
        }

        public override object VisitQSelect(QSelect node)
        {
            _qm.m_select();
            _l.WriteLine("m_select");

            base.VisitQSelect(node);
            return null;
        }


        public override object VisitQObjectTable(QObjectTable node)
        {
            var ot = node.ObjectType;

            ot.Parent.ComponentImpl.QueryInjector.InjectDataSource(_qm, ot, null);

            if (!_hasAlias)
                _qm.@as(node.GetDbName());

            _hasAlias = false;

            return base.VisitQObjectTable(node);
        }

        private enum TypesComparerOp
        {
        }

        private List<XCTypeBase> CommonTypes(List<XCTypeBase> types1, List<XCTypeBase> types2)
        {
            var result = new List<XCTypeBase>();
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

        public override object VisitQEquals(QEquals node)
        {
            var leftTypes = node.Left.GetExpressionType().ToList();
            var rightTypes = node.Right.GetExpressionType().ToList();

            if (!leftTypes.Any() || !rightTypes.Any())
            {
                throw new Exception("Can't optimize expression with empty types");
            }

            if (leftTypes.Count == 1 && rightTypes.Count == 1)
            {
                //Если количество типов одно и тоже мы просто визитируем дальше
                base.VisitQEquals(node);
            }
            else
            {
                //Нужно понять, какая у нас ситуация
                //1: Pure equals
                if (node.Left is QObjectField leftField && node.Right is QObjectField rightField)
                {
                    var commonTypes = CommonTypes(leftTypes, rightTypes);

                    foreach (var type in commonTypes)
                    {
                        leftField.Property.GetPropertySchemas().Where(x => x.PlatformType == type);
                    }
                }
            }

            _qm.eq();

            return null;
        }

        public override object VisitQFrom(QFrom node)
        {
            _qm.m_from();
            _l.WriteLine("m_from");

            Visit(node.Source);

            foreach (var nodeJoin in node.Joins)
            {
                VisitQFromItem(nodeJoin);
            }

            return null;
        }

        public override object VisitQFromItem(QFromItem node)
        {
            Visit(node.Joined);
            Visit(node.Condition);

            _qm.@join();

            return null;
        }

        public override object VisitQNestedQuery(QNestedQuery node)
        {
            return base.VisitQNestedQuery(node);
        }

        public override object VisitQSelectExpression(QSelectExpression node)
        {
            return base.VisitQSelectExpression(node);
        }

        public override object VisitQAliasedDataSource(QAliasedDataSource node)
        {
            _hasAlias = true;

            base.VisitQAliasedDataSource(node);

            _qm.@as(node.GetDbName());

            return null;
        }

        private void LoadNamedSource(string arg)
        {
            if (_hasNamedSource) return;

            _qm.ld_str(arg);
            _hasNamedSource = true;
        }

        public override object VisitQIntermediateSourceField(QIntermediateSourceField node)
        {
            LoadNamedSource(node.DataSource.GetDbName());

            if (node.DataSource is QAliasedDataSource ads)
            {
                base.VisitQIntermediateSourceField(node);
            }
            else if (node.DataSource is QNestedQuery)
            {
                var schema = PropertyHelper.GetPropertySchemas(node.GetDbName(), node.GetExpressionType().ToList());
                GenColumn(schema);
            }

            return null;
        }

        public override object VisitQSourceFieldExpression(QSourceFieldExpression node)
        {
            var schema = node.Property.GetPropertySchemas();

            LoadNamedSource(node.ObjectTable.GetDbName());

            GenColumn(schema);

            return null;
        }

        private void GenColumn(IEnumerable<XCColumnSchemaDefinition> schema)
        {
            string tabName = null;
            string alias = null;

            if (_hasNamedSource)
                tabName = (string) _qm.pop();

            if (_hasAlias)
                alias = (string) _qm.pop();

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

        public override object VisitQAliasedSelectExpression(QAliasedSelectExpression node)
        {
            _hasAlias = true;
            _qm.ld_str(node.GetDbName());

            var res = base.VisitQAliasedSelectExpression(node);

            return res;
        }
    }
}