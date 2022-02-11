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

        public static QUpdateQuery GetSaveUpdateQuery(SMEntity entity, MetadataProvider em)
        {
            /*
             NOTE:
             User can't update objects that user's has not access
             We have to check values before and after update
             */

            var ds = new QObjectTable(entity);
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

            var where = new QWhere(new QEquals(targetSource.GetField(entity.IdProperty.Name), idParam));

            return new QUpdateQuery(new QUpdate(targetSource), qset,
                new QFrom(new QJoinList(new[] { joinedValues }.ToImmutableArray()), targetSource), where,
                QCriterionList.Empty);
        }

        public static QInsertSelectQuery GetSaveInsertQuery(SMEntity entity, MetadataProvider em)
        {
            /*
             NOTE:
             We must insert tables BEFORE object insertion.
             In this way platform can handle sec rights dependent on tables 
             */

            var ds = new QObjectTable(entity);
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

            // var qm = new QLang(em);
            //
            // var name = entity.FullName;
            // qm.new_scope()
            //     .ld_source(name)
            //     .create(QObjectType.SourceFieldList);
            //
            // var props = entity.Properties.ToImmutableArray();
            // foreach (var property in props)
            // {
            //     qm.ld_source(name)
            //         .ld_field(property.Name)
            //         .st_elem();
            // }
            //
            // qm.insert()
            //     .create(QObjectType.ExpressionSet)
            //     .create(QObjectType.ExpressionList);
            //
            // foreach (var property in props)
            // {
            //     qm.ld_param(property.Name)
            //         .st_elem();
            // }
            //
            // qm.st_elem()
            //     .new_insert_query();
            //
            // return qm.top<QInsertQuery>();
        }

        public static QDeleteQuery GetDeleteQuery(SMEntity entity, MetadataProvider em)
        {
            var source = new QAliasedDataSource(new QObjectTable(entity), "TS");
            QDelete delete = new QDelete(source);
            QFrom from = new QFrom(null, source);
            QParameter param = new QParameter("Id");
            QWhere where = new QWhere(new QEquals(source.GetField("Id"), param));
            return new QDeleteQuery(delete, from, where, QCriterionList.Empty);
        }

        public static string CompileInsert(SMEntity entity, AqContext context)
        {
            QLangElement query = GetSaveInsertQuery(entity, context.MetadataProvider);
            return CompileCore(query, context, new InsertionRealWalker(context.DataRuntimeContext));
        }

        public static string CompileUpdate(SMEntity entity, AqContext context)
        {
            QLangElement query = GetSaveUpdateQuery(entity, context.MetadataProvider);
            return CompileCore(query, context, new UpdationRealWalker(context.DataRuntimeContext));
        }

        public static string CompileDelete(SMEntity entity, AqContext context)
        {
            QLangElement query = GetDeleteQuery(entity, context.MetadataProvider);
            return CompileCore(query, context, new DeletionRealWalker(context.DataRuntimeContext));
        }

        private static string CompileCore(QLangElement query, AqContext context, RealWalkerBase rw)
        {
            query = new SecurityVisitor(context.MetadataProvider, context.SecTable).Visit(query);
            new PhysicalNameWalker(context.DataRuntimeContext).Visit(query);
            rw.Visit(query);
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

        public static string GetSecUpdate(string baseQuery, int typeId, AqContext context)
        {
            var ust = new ContextSecTable();
            ust.Init(new List<SMSecPolicy>());
            var desc = context.DataRuntimeContext.Descriptors.GetEntityDescriptor(typeId);
            var mdId = desc.MetadataId;
            var md = context.MetadataProvider.GetSemanticByName(mdId);

            if (ust.TryClaimPermission(md, SecPermission.Update, out var claim))
            {
                //QLang q = new QLang();
                //q.@select();
            }
            else
            {
                //access denied
            }

            return baseQuery + "";
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