using ZenPlatform.QueryBuilder.Interfaces;

namespace ZenPlatform.QueryBuilder
{
    public class DBTableField : DBField
    {
        public DBTableField(DBTable owner, string name) : base(owner, name)
        {
            base.CompileExpression = "[{OwnerName}].[{Name}]";
        }

        public override string Compile(bool recompile = false)
        {
            var ownerName = (string.IsNullOrEmpty((Owner as DBTable).Alias))
                ? (Owner as DBTable).Name
                : (Owner as DBTable).Alias;

            return
                $"[{ownerName}].[{Name}]"; //StandartCompilers.SimpleCompiler(base.CompileExpression, new { OwnerName = ownerName, Name });
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
            return $"[{(Owner as IDBAliasedFieldContainer).Alias}].[{Name}]";
        }
    }
}