using System;

namespace SqlPlusDbSync.QueryCompiler.Queryes
{
    public class SingleResultSelectQuery : SelectQueryBase, ISelectItem
    {
        public string Alias { get; set; }
        public ITable Owner
        {
            get { return null; }
            set { }
        }

        public override string Compile()
        {
            if (string.IsNullOrEmpty(Alias))
                throw new ArgumentNullException("Alias");

            return base.Compile();
        }
    }
}