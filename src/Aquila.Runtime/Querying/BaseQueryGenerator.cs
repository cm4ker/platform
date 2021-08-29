using System.Collections.Immutable;
using System.Linq;
using Aquila.Core.Querying.Model;
using Aquila.Metadata;
using Aquila.QueryBuilder;
using Aquila.QueryBuilder.Model;
using Aquila.QueryBuilder.Visitor;

namespace Aquila.Runtime.Querying
{
    public class CRUDQueryGenerator
    {
        public string GetSaveUpdate(SMEntity entity, DatabaseRuntimeContext drc)
        {
            var e_desc = drc.FindEntityDescriptor(entity.FullName);
            var qm = new QueryMachine();
            var paramNum = 0;
            qm.bg_query()
                .m_from()
                .ld_table(e_desc.DatabaseName)
                .@as("t")
                .m_where()
                .ld_column("id")
                .ld_param($"p_{paramNum++}")
                .eq()
                .m_set();

            var props = entity.Properties.ToImmutableArray();

            foreach (var property in props)
            {
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

        public string GetSaveInsert(SMEntity entity, DatabaseRuntimeContext drc)
        {
            var e_desc = drc.FindEntityDescriptor(entity.FullName);
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

        public string GetLoad(SMEntity entity, DatabaseRuntimeContext drc)
        {
            var e_desc = drc.FindEntityDescriptor(entity.FullName);
            var id_desc = drc.FindEntityDescriptor(entity.IdProperty.FullName);

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
    }
}