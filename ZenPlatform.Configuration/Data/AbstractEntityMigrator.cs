using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration.Data.Contracts.Entity;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.QueryBuilder.Builders.Create;
using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.QueryBuilder.Contracts;
using ZenPlatform.QueryBuilder.Model;
using ZenPlatform.QueryBuilder.Builders;

namespace ZenPlatform.Configuration.Data
{
    public abstract class AbstractEntityMigrator : IEntityMigrator
    {

        protected XCObjectTypeBase Old { get; private set; }
        protected XCObjectTypeBase Actual { get; private set; }

        private MultiNode _expression;

        protected ICreateExpressionRoot Create { get; private set; }

        protected IAlterExpressionRoot Alter { get; private set; }

        protected abstract void Migration();
    

        public IList<SqlNode> GetScript(XCObjectTypeBase old, XCObjectTypeBase actual)
        {
            _expression = new MultiNode();
            Create = new CreateExpressionRoot(_expression);
            Alter = new AlterExpressionRoot(_expression);
            Old = old;
            Actual = actual;
            Migration();

            return _expression.Nodes.Select(n => (SqlNode)(n)).ToList();
        }
    }

}

namespace ZenPlatform.Configuration.Data
{

    public class EntityMigrator : AbstractEntityMigrator
    {
        protected override void Migration()
        {
            Create.Table("asdasd")
                .WithColumn("sdasds");

            foreach(var prop in Actual.GetProperties())
            {
                Alter.Column(prop.Name).OnTable(Actual.Name).AsBinary();
            }
        }
    }
}