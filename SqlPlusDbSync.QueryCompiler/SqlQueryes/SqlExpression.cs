using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlPlusDbSync.QueryCompiler.Queryes;

namespace SqlPlusDbSync.QueryCompiler.SqlQueryes
{
    public abstract class SqlExpression
    {
        public virtual string Compile()
        {
            throw new NotImplementedException();
        }
    }



    public class SqlTableSourceExpression : SqlExpression
    {
        public string Alias { get; set; }

        public List<Transformation> Transformations { get; set; }
    }

    public class SqlTableExpression : SqlTableSourceExpression
    {
        public List<SqlFieldExpression> Fields { get; set; }
    }

    public class SqlBooleanExpression
    {

    }



    public class SqlSelectExpression : SqlTableSourceExpression
    {

        public List<SqlSelectItem> Select { get; set; }

        public SqlTableSourceExpression Table { get; set; }

        public List<SqlFieldExpression> GroupBy { get; set; }
        public List<SqlFieldExpression> OrderBy { get; set; }

        public SqlBooleanExpression Where { get; set; }

        public List<JoinExpression> Joins { get; set; }

        public void AddJoin(JoinType joinType, SqlTableSourceExpression table, SqlBooleanExpression on)
        {
            JoinExpression result = null;
            switch (joinType)
            {
                case JoinType.Inner:
                    result = new InnerJoinExpression(table, on);
                    break;
                case JoinType.Left:
                    result = new LeftJoinExpression(table, on);
                    break;
                case JoinType.Right:
                    result = new RightJoinExpression(table, on);
                    break;
                default:
                    throw new NotSupportedException();

            }

            Joins.Add(result);
        }
    }


    public abstract class JoinExpression : SqlExpression
    {

        protected JoinExpression(SqlTableSourceExpression table, SqlBooleanExpression on)
        {
            TableSource = table;
            On = on;
        }

        public virtual JoinType JoinType
        {
            get { throw new NotImplementedException(); }
        }

        public virtual SqlTableSourceExpression TableSource { get; set; }

        public SqlBooleanExpression On { get; set; }
    }


    public class InnerJoinExpression : JoinExpression
    {
        public override JoinType JoinType
        {
            get { return JoinType.Inner; }
        }

        public InnerJoinExpression(SqlTableSourceExpression table, SqlBooleanExpression on) : base(table, on)
        {
        }
    }

    public class LeftJoinExpression : JoinExpression
    {
        public override JoinType JoinType
        {
            get { return JoinType.Inner; }
        }

        public LeftJoinExpression(SqlTableSourceExpression table, SqlBooleanExpression on) : base(table, on)
        {
        }
    }

    public class RightJoinExpression : JoinExpression
    {
        public override JoinType JoinType
        {
            get { return JoinType.Inner; }
        }

        public RightJoinExpression(SqlTableSourceExpression table, SqlBooleanExpression on) : base(table, on)
        {
        }
    }



    public class SqlFieldExpression : SqlExpression
    {
        public List<Transformation> Transformations { get; set; }
    }

    public abstract class SqlSelectItem : SqlExpression
    {

    }

    public class SqlSelectField : SqlSelectItem
    {

    }


    public abstract class Transformation : SqlExpression
    {
        public virtual int Priority
        {
            get { return 0; }
        }

        public virtual void Apply()
        {

        }

        public virtual void Cease()
        {

        }
    }

    public abstract class FieldInSelectStatementTransformation : Transformation
    {
        protected SqlSelectExpression SelectExpression;
        protected SqlFieldExpression FieldExpression;

        protected FieldInSelectStatementTransformation(SqlSelectExpression selectExpression, SqlFieldExpression fieldExpression)
        {
            SelectExpression = selectExpression ?? throw new ArgumentNullException(nameof(selectExpression));
            FieldExpression = fieldExpression ?? throw new ArgumentNullException(nameof(fieldExpression));
        }

        public override void Apply()
        {
            SelectExpression.Transformations.Add(this);
        }

        public override void Cease()
        {
            SelectExpression.Transformations.Remove(this);
        }
    }
    public abstract class TableSourceTransformation : Transformation
    {
        protected SqlTableSourceExpression TableSourceExpression;

        protected TableSourceTransformation(SqlTableSourceExpression tableSourceExpression)
        {
            TableSourceExpression = tableSourceExpression ?? throw new ArgumentNullException(nameof(tableSourceExpression));
        }

        public override void Apply()
        {
            TableSourceExpression.Transformations.Add(this);
        }

        public override void Cease()
        {
            TableSourceExpression.Transformations.Remove(this);
        }
    }





    public class TableAliasTransformation : TableSourceTransformation
    {
        private readonly string _alias;

        public TableAliasTransformation(SqlTableSourceExpression tableSourceExpression, string alias) : base(tableSourceExpression)
        {
            _alias = alias;
        }

        public override void Apply()
        {
            base.Apply();
            TableSourceExpression.Alias = _alias;
        }

        public override void Cease()
        {
            base.Cease();
            TableSourceExpression.Alias = "";
        }
    }

    public class GroupByTransformation : FieldInSelectStatementTransformation
    {
        public GroupByTransformation(SqlSelectExpression selectExpression, SqlFieldExpression fieldExpression) : base(selectExpression, fieldExpression)
        {
        }

        public override void Apply()
        {
            base.Apply();
            SelectExpression.GroupBy.Add(FieldExpression);
        }

        public override void Cease()
        {
            base.Cease();
            SelectExpression.GroupBy.Remove(FieldExpression);
        }

        public override int Priority => (int)PriorityTransformation.GroupBy;
    }
    public class OrderByTransformation : FieldInSelectStatementTransformation
    {

        public OrderByTransformation(SqlSelectExpression selectExpression, SqlFieldExpression fieldExpression) : base(selectExpression, fieldExpression)
        {
        }

        public override void Apply()
        {
            base.Apply();
            SelectExpression.OrderBy.Add(FieldExpression);
        }

        public override void Cease()
        {
            base.Cease();
            SelectExpression.OrderBy.Remove(FieldExpression);
        }

        public override int Priority => (int)PriorityTransformation.OrderBy;
    }

    public enum PriorityTransformation
    {
        GroupBy = 0,
        OrderBy = 1
    }
}
