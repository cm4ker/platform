using System.Collections.Generic;
using System.IO;
using System.Linq;
using ZenPlatform.Configuration.Structure.Data.Types;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using ZenPlatform.Core.Querying.Model;
using ZenPlatform.Language.Ast.Infrastructure;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Core.Querying
{
    public static class QLangExtensions
    {
        public static void SetDbName(this QItem item, string name)
        {
            item.AttachedPropery["DbName"] = name;
        }

        public static string GetDbName(this QItem item)
        {
            if (item.AttachedPropery.TryGetValue("DbName", out var result))
                return (string) result;
            else
                return null;
        }
    }

    public class PhysicalNameWalker : QLangWalker
    {
        public int _aliasCount;
        public int _fieldCount;
        public int _tableCount;

        public override object VisitQQuery(QQuery node)
        {
            VisitQFrom(node.From);
            VisitQSelect(node.Select);

            return null;
        }

        public override object VisitQSourceFieldExpression(QSourceFieldExpression node)
        {
            node.SetDbName($"{node.Property.DatabaseColumnName}");
            return base.VisitQSourceFieldExpression(node);
        }

        public override object VisitQAliasedSelectExpression(QAliasedSelectExpression node)
        {
            node.SetDbName($"A{_aliasCount++}");
            return base.VisitQAliasedSelectExpression(node);
        }

        public override object VisitQIntermediateSourceField(QIntermediateSourceField node)
        {
            base.VisitQIntermediateSourceField(node);
            node.SetDbName(node.Field.GetDbName());
            return null;
        }

        public override object VisitQSelectExpression(QSelectExpression node)
        {
            base.VisitQSelectExpression(node);
            node.SetDbName(node.Element.GetDbName());
            return null;
        }
    }

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

            return base.VisitQObjectTable(node);
        }

        public override object VisitQEquals(QEquals node)
        {
            base.VisitQEquals(node);
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
            base.VisitQAliasedDataSource(node);

            _qm.@as(node.Alias);
            return null;
        }

        private IEnumerable<XCColumnSchemaDefinition> Get(string name, List<XCTypeBase> types)
        {
            var done = false;

            if (types.Count == 1)
                yield return new XCColumnSchemaDefinition(XCColumnSchemaType.NoSpecial, types[0], name, false);
            if (types.Count > 1)
            {
                yield return new XCColumnSchemaDefinition(XCColumnSchemaType.Type, null, name,
                    false, "", "_Type");

                foreach (var type in types)
                {
                    if (type is XCPrimitiveType)
                        yield return new XCColumnSchemaDefinition(XCColumnSchemaType.Value, type,
                            name, false, "", $"_{type.Name}");

                    if (type is XCObjectTypeBase obj && !done)
                    {
                        yield return new XCColumnSchemaDefinition(XCColumnSchemaType.Ref, type, name,
                            !obj.Parent.ComponentImpl.DatabaseObjectsGenerator.HasForeignColumn, "", "_Ref");

                        done = true;
                    }
                }
            }
        }

        public override object VisitQIntermediateSourceField(QIntermediateSourceField node)
        {
            if (node.DataSource is QAliasedDataSource ads)
            {
                _qm.ld_str(ads.Alias);
                _hasNamedSource = true;
                _l.WriteLine($"ld_str({ads.Alias})");

                base.VisitQIntermediateSourceField(node);
            }
            else if (node.DataSource is QNestedQuery)
            {
                var schema = Get(node.GetDbName(), node.GetExpressionType().ToList());
                GenColumn(schema);
            }

            return null;
        }

        public override object VisitQSourceFieldExpression(QSourceFieldExpression node)
        {
            var schema = node.Property.GetPropertySchemas();

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