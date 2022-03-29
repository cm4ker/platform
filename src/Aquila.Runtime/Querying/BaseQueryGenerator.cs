using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Aquila.Core;
using Aquila.Core.Querying;
using Aquila.Core.Querying.Model;
using Aquila.Metadata;
using Aquila.QueryBuilder;
using Aquila.QueryBuilder.Model;
using Aquila.QueryBuilder.Visitor;

namespace Aquila.Runtime.Querying
{
    public class CRUDQueryGenerator
    {
        public static string GetSaveUpdate(SMEntity entity, DatabaseRuntimeContext drc)
        {
            var e_desc = drc.Descriptors.GetEntityDescriptor(entity.FullName);
            var id_desc = drc.Descriptors.GetEntityDescriptor(entity.IdProperty.FullName);

            var qm = new QueryMachine();
            var paramNum = 0;
            qm.bg_query()
                .m_from()
                .ld_table(e_desc.DatabaseName)
                .@as("t")
                .m_where()
                .ld_column(id_desc.DatabaseName)
                .ld_param($"p_{paramNum++}")
                .eq()
                .m_set();

            var props = entity.Properties.ToImmutableArray();

            foreach (var property in props)
            {
                //we not need update the id property
                if (property.IsIdProperty)
                    continue;

                foreach (var column in property.GetSchema(drc))
                {
                    qm.ld_column(column.FullName)
                        .ld_param($"p_{paramNum++}")
                        .assign();
                }
            }

            qm.m_update()
                .ld_table("t")
                .st_query();
            var builder = new MsSqlBuilder();

            return builder.Visit((SSyntaxNode)qm.peek());
        }

        public static string GetSaveInsert(SMEntity entity, DatabaseRuntimeContext drc)
        {
            var e_desc = drc.Descriptors.GetEntityDescriptor(entity.FullName);

            var qm = new QueryMachine();
            var paramNum = 0;

            qm.bg_query()
                .m_values();

            var props = entity.Properties.ToImmutableArray();

            foreach (var property in props)
            {
                foreach (var column in property.GetSchema(drc))
                {
                    qm.ld_param($"p_{paramNum++}");
                }
            }

            qm.m_insert()
                .ld_table(e_desc.DatabaseName);

            foreach (var property in props)
            {
                foreach (var column in property.GetSchema(drc))
                {
                    qm.ld_column(column.FullName);
                }
            }

            qm.st_query();
            var builder = new MsSqlBuilder();

            return builder.Visit((SSyntaxNode)qm.peek());
        }

        public static QUpdateQuery GetSaveUpdateQuery(SMEntityOrTable entity, MetadataProvider em)
        {
            /*
             NOTE:
             User can't update objects that user's has not access
             We have to check values before and after update
             */

            var (ds, idFieldName) = GetDS(entity);

            var targetSource = new QAliasedDataSource(ds, "TS");

            var select = new QSelect(new QFieldList(
                entity.Properties.Select(x =>
                        (QField)new QAliasedSelectExpression(new QTypedParameter(x.Name, x.Types), x.Name))
                    .ToImmutableArray()));

            var joinedValues = new QFromItem(null,
                new QAliasedDataSource(
                    new QNestedQuery(new QSelectQuery(null, select, null, null, null, null, QCriterionList.Empty)),
                    "Values"), QJoinType.Cross);

            QExpression idParam = null;


            var assigns = targetSource.GetFields()
                .Select(x =>
                {
                    var name = x.GetName();
                    var field = joinedValues.Joined.GetField(name);

                    //TODO: not update Id field
                    //QTypedParameter param = new QTypedParameter(name, x.GetExpressionType());

                    if (name == "Id")
                    {
                        idParam = field;
                    }

                    return new QAssign(x, field);
                })
                .ToImmutableArray();

            if (idParam == null)
                throw new Exception("The id param is null");

            var qset = new QSet(new QAssignList(assigns));

            var where = new QWhere(new QEquals(targetSource.GetField(idFieldName), idParam));

            return new QUpdateQuery(new QUpdate(targetSource), qset,
                new QFrom(new QJoinList(new[] { joinedValues }.ToImmutableArray()), targetSource), where,
                QCriterionList.Empty);
        }

        public static QInsertSelectQuery GetSaveInsertQuery(SMEntityOrTable entity, MetadataProvider em)
        {
            var (ds, idFieldName) = GetDS(entity);

            var insert =
                new QInsert(
                    new QSourceFieldList(entity.Properties.Select(x => new QSourceFieldExpression(ds, x))
                        .ToImmutableArray()), ds);

            var select = new QSelect(new QFieldList(
                entity.Properties.Select(x =>
                        (QField)new QAliasedSelectExpression(new QTypedParameter(x.Name, x.Types), x.Name))
                    .ToImmutableArray()));

            var nq = new QAliasedDataSource(new QNestedQuery(new QSelectQuery(null, select, null, null, null,
                null,
                QCriterionList.Empty)), "TS");


            var q = new QSelectQuery(null,
                new QSelect(new QFieldList(nq.GetFields().Select(x => (QField)new QSelectExpression(x))
                    .ToImmutableArray())),
                null,
                null, null, new QFrom(null, nq), QCriterionList.Empty);

            return new QInsertSelectQuery(q, insert);
        }

        private static (QPlatformDataSource ds, string idFieldName) GetDS(SMEntityOrTable entity)
        {
            QPlatformDataSource ds;
            string idFieldName;

            switch (entity)
            {
                case SMEntity ent:
                    ds = new QObject(ent);
                    idFieldName = ent.IdProperty.Name;

                    break;
                case SMTable tbl:
                    ds = new QTable(tbl);
                    idFieldName = tbl.ParentProperty.Name;

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(entity), entity, null);
            }

            return (ds, idFieldName);
        }

        public static QDeleteQuery GetDeleteQuery(SMEntityOrTable entity, MetadataProvider em)
        {
            var (ds, idFieldName) = GetDS(entity);

            var source = new QAliasedDataSource(ds, "TS");
            QDelete delete = new QDelete(source);
            QFrom from = new QFrom(null, source);
            QParameter param = new QParameter("Id");
            QWhere where = new QWhere(new QEquals(source.GetField(idFieldName), param));
            return new QDeleteQuery(delete, from, where, QCriterionList.Empty);
        }

        public static QSelectQuery GetSelectQuery(SMEntityOrTable entity, MetadataProvider em)
        {
            var (ds, idFieldName) = GetDS(entity);

            var source = new QAliasedDataSource(ds, "TS");
            QSelect select = new QSelect(new QFieldList(source.GetFields().ToImmutableArray()));
            QFrom from = new QFrom(null, source);
            QParameter param = new QParameter("Id");
            QWhere where = new QWhere(new QEquals(source.GetField(idFieldName), param));
            return new QSelectQuery(null, select, null, null, where, from, QCriterionList.Empty);
        }

        public static string CompileSelect(SMEntityOrTable entity, AqContext context, out QLangElement query,
            out bool hasCriteria)
        {
            var selectQuery = GetSelectQuery(entity, context.MetadataProvider);
            hasCriteria = selectQuery.HasCriteria;
            query = selectQuery;

            return CompileCore(query, context, new SelectionRealWalker(context.DataRuntimeContext),
                out query);
        }

        public static string CompileInsert(SMEntityOrTable entity, AqContext context,
            out QLangElement query)
        {
            query = GetSaveInsertQuery(entity, context.MetadataProvider);
            return CompileCore(query, context, new InsertionRealWalker(context.DataRuntimeContext),
                out query);
        }

        public static string CompileUpdate(SMEntityOrTable entity, AqContext context, out QLangElement query)
        {
            query = GetSaveUpdateQuery(entity, context.MetadataProvider);
            return CompileCore(query, context, new UpdationRealWalker(context.DataRuntimeContext),
                out query);
        }

        public static string CompileDelete(SMEntityOrTable entity, AqContext context, out QLangElement query)
        {
            query = GetDeleteQuery(entity, context.MetadataProvider);
            return CompileCore(query, context, new DeletionRealWalker(context.DataRuntimeContext),
                out query);
        }

        private static string CompileCore(QLangElement query, AqContext context, RealWalkerBase rw,
            out QLangElement transformedQuery)
        {
            //transform query with security context
            transformedQuery = new SecurityVisitor(context.MetadataProvider, context.SecTable).Visit(query);
            new PhysicalNameWalker(context.DataRuntimeContext).Visit(transformedQuery);
            rw.Visit(transformedQuery);
            return context.DataContext.SqlCompiller.Compile((SSyntaxNode)rw.QueryMachine.peek());
        }

        public static string GetLoad(SMEntity entity, DatabaseRuntimeContext drc)
        {
            var e_desc = drc.Descriptors.GetEntityDescriptor(entity.FullName);
            var id_desc = drc.Descriptors.GetEntityDescriptor(entity.IdProperty.FullName);

            var qm = new QueryMachine();
            var paramNum = 0;

            qm.bg_query()
                .m_from()
                .ld_table(e_desc.DatabaseName)
                .m_where()
                .ld_column(id_desc.DatabaseName)
                .ld_param("p0")
                .eq()
                .m_select()
                ;

            foreach (var prop in entity.Properties)
            {
                foreach (var schema in prop.GetSchema(drc))
                {
                    qm.ld_column(schema.FullName);
                }
            }

            qm.st_query();
            var builder = new MsSqlBuilder();

            return builder.Visit((SSyntaxNode)qm.peek());
        }

        /*
         We need create query in context of the object and user
         
         User have some security rights and we have to apply this information to result query
         
         now we generate this code:
         
         var cmd = <ctx>.CreateCommand(InsertObjectQuery); <-- problem here. We need transform query with sec aspect
         
         cmd.AddParam("p0", value0);
         ... more params
         
         cmd.ExecuteNonQuery();
         cmd.Dispose();
         */
    }
}