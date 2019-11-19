using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Antlr4.Runtime.Atn;
using dnlib.DotNet;
using MoreLinq.Extensions;
using Portable.Xaml;
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

        private string ConvertType(XCTypeBase type)
        {
            if (type is XCPrimitiveType)
            {
                if (type is XCBinary b) return $"varbinary{b.Size}";
                if (type is XCGuid) return "guid";
                if (type is XCInt) return "int";
                if (type is XCNumeric n) return $"numeric({n.Scale}, {n.Precision})";
                if (type is XCDateTime) return "datetime";
                if (type is XCBoolean) return "bool";
                if (type is XCString s) return $"varchar({s.Size})";
            }

            if (type is XCObjectTypeBase) return "guid";

            throw new Exception("Unknown type");
        }

        public override object VisitQCast(QCast node)
        {
            QSourceFieldExpression sf = null;
            string fieldName = null;

            if (node.BaseExpression is QSourceFieldExpression)
            {
                sf = (QSourceFieldExpression) node.BaseExpression;
            }
            else if ((node.BaseExpression is QIntermediateSourceField ife && ife.Field is QSourceFieldExpression))
            {
                fieldName = ife.DataSource.GetDbName();
                sf = (QSourceFieldExpression) ife.Field;
            }

            if (sf != null)
            {
                var schemas = sf.Property.GetPropertySchemas();

                if (sf.Property.Types.Count > 1)
                {
                    var typeSchema = schemas.First(x => x.SchemaType == XCColumnSchemaType.Type);

                    foreach (var schema in schemas.Where(x => x.SchemaType != XCColumnSchemaType.Type))
                    {
                        if (string.IsNullOrEmpty(fieldName))
                            _qm.ld_column(schema.FullName);
                        else
                        {
                            _qm.ld_str(fieldName);
                            _qm.ld_str(schema.FullName);
                            _qm.ld_column();
                        }

                        _qm.ld_type(ConvertType(node.Type));
                        _qm.cast();

                        if (string.IsNullOrEmpty(fieldName))
                            _qm.ld_column(typeSchema.FullName);
                        else
                        {
                            _qm.ld_str(fieldName);
                            _qm.ld_str(typeSchema.FullName);
                            _qm.ld_column();
                        }

                        _qm.ld_const(schema.PlatformType.Id);
                        _qm.eq();

                        _qm.when();
                    }

                    _qm.@case();
                }
            }
            else
            {
                base.VisitQCast(node);

                _qm.ld_type(ConvertType(node.Type));
                _qm.cast();
            }

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
                _qm.eq();
            }
            else
            {
                //Нужно понять, какая у нас ситуация 

                //1: Pure equals
                if (node.Left is QSourceFieldExpression leftField && node.Right is QSourceFieldExpression rightField)
                {
                    var commonTypes = CommonTypes(leftTypes, rightTypes);

                    foreach (var type in commonTypes)
                    {
                        var leftSchema =
                            leftField.Property.GetPropertySchemas().FirstOrDefault(x => x.PlatformType == type) ??
                            throw new Exception($"Can't find in Property: {leftField.Property.Name} type {type}");

                        var rightSchema =
                            rightField.Property.GetPropertySchemas().FirstOrDefault(x => x.PlatformType == type) ??
                            throw new Exception($"Can't find in Property: {rightField.Property.Name} type {type}");

                        _qm.ld_column(leftSchema.FullName);
                        _qm.ld_column(leftSchema.FullName);
                        _qm.eq();
                    }
                }

                //2: Pure equals with intermediate field
                else if (node.Left is QIntermediateSourceField ileft &&
                         node.Right is QIntermediateSourceField iright &&
                         ileft.Field is QSourceFieldExpression l && iright.Field is QSourceFieldExpression r)
                {
                    var commonTypes = CommonTypes(leftTypes, rightTypes);

                    var lpropSchema = l.Property.GetPropertySchemas();
                    var rpropSchema = r.Property.GetPropertySchemas();

                    var tr = rpropSchema.First(x => x.SchemaType == XCColumnSchemaType.Type);
                    var tl = lpropSchema.First(x => x.SchemaType == XCColumnSchemaType.Type);

                    _qm.ld_str(ileft.DataSource.GetDbName());
                    _qm.ld_str(tl.FullName);
                    _qm.ld_column();

                    _qm.ld_str(iright.DataSource.GetDbName());
                    _qm.ld_str(tr.FullName);
                    _qm.ld_column();

                    _qm.eq();

                    foreach (var type in commonTypes)
                    {
                        var leftSchema =
                            lpropSchema.FirstOrDefault(x => x.PlatformType.Equals(type)) ??
                            throw new Exception($"Can't find in Property: {l.Property.Name} type {type}");

                        var rightSchema =
                            rpropSchema.FirstOrDefault(x => x.PlatformType.Equals(type)) ??
                            throw new Exception($"Can't find in Property: {r.Property.Name} type {type}");

                        _qm.ld_str(ileft.DataSource.GetDbName());
                        _qm.ld_str(leftSchema.FullName);
                        _qm.ld_column();

                        _qm.ld_str(iright.DataSource.GetDbName());
                        _qm.ld_str(rightSchema.FullName);
                        _qm.ld_column();

                        _qm.eq();

                        _qm.and();
                    }
                }

                else if (node.Left is QCast cast)
                {
                    Visit(node.Left);

                    if (node.Right is QSourceFieldExpression ||
                        (node.Right is QIntermediateSourceField ifs && ifs.Field is QSourceFieldExpression))
                    {
                        string tableName = "";

                        if (node.Right is QIntermediateSourceField qsf)
                        {
                            tableName = qsf.DataSource.GetDbName();
                        }

                        var sfield =
                            (QSourceFieldExpression) ((node.Right as QIntermediateSourceField)?.Field) ??
                            (node.Right as QSourceFieldExpression);

                        var schema = sfield.Property.GetPropertySchemas()
                            .FirstOrDefault(x => x.PlatformType.Equals(cast.GetExpressionType().First()));

                        if (schema != null)
                        {
                            if (string.IsNullOrEmpty(tableName))
                                _qm.ld_column(schema.FullName);
                            else
                            {
                                _qm.ld_str(tableName);
                                _qm.ld_str(schema.FullName);
                                _qm.ld_column();
                            }
                        }
                        else
                        {
                            _qm.ld_null();
                        }

                        _qm.eq();
                    }
                }
            }

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