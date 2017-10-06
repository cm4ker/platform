using QueryCompiler.Interfaces;
using QueryCompiler.Schema;

namespace QueryCompiler
{
    public class DBTableField : DBField
    {
        public DBTableField(DBTable owner, string name) : base(owner, name)
        {
            base.CompileExpression = "[{OwnerName}].[{Name}]";
            Schema = owner.SchemaFields.Find(x => x.ColumnName == name);
        }

        public DBTableField(DBTable owner, string name, DBFieldSchema schema) : base(owner, name)
        {
            base.CompileExpression = "[{OwnerName}].[{Name}]";
            Schema = schema;
        }

        public override string Compile(bool recompile = false)
        {
            var ownerName = (string.IsNullOrEmpty((Owner as DBTable).Alias))
                ? (Owner as DBTable).Name
                : (Owner as DBTable).Alias;
            return StandartCompilers.SimpleCompiler(base.CompileExpression, new { OwnerName = ownerName, Name });
        }
    }

    public class DBClauseField : DBField
    {
        public DBClauseField(IDBAliasedFieldContainer owner, string name) : base(owner, name)
        {
            base.CompileExpression = "[{OwnerName}].[{Name}]";
        }

        public override string Compile(bool recompile = false)
        {
            return StandartCompilers.SimpleCompiler(base.CompileExpression, new { OwnerName = Owner.Alias, Name });
        }
    }
}