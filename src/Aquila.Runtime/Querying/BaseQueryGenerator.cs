using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Aquila.Core;
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

        public static QInsertQuery GetSaveInsertSingleQuery(SMEntity entity, EntityMetadataCollection em)
        {
            var qm = new QLang(em);

            var name = entity.FullName;
            qm.new_scope()
                .ld_source(name)
                .create(QObjectType.SourceFieldList);

            var props = entity.Properties.ToImmutableArray();
            foreach (var property in props)
            {
                qm.ld_source(name)
                    .ld_field(property.Name)
                    .st_elem();
            }

            qm.insert()
                .create(QObjectType.ExpressionSet)
                .create(QObjectType.ExpressionList);

            foreach (var property in props)
            {
                qm.ld_param(property.Name)
                    .st_elem();
            }

            qm.st_elem()
                .new_insert_query();

            return qm.top<QInsertQuery>();
        }

        public QInsertQuery TransformQuery(QInsertQuery query)
        {
            //SecurityVisitor sec = new SecurityVisitor();
            throw new NotImplementedException();
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
            var ust = new UserSecTable();
            ust.Init(new List<SMSecPolicy>(), context.MetadataCollection);
            var desc = context.DataRuntimeContext.Descriptors.GetEntityDescriptor(typeId);
            var mdId = desc.MetadataId;
            var md = context.MetadataCollection.GetSemanticByName(mdId);

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