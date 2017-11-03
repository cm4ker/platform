using QueryCompiler.Schema;


namespace SqlPlusDbSync.Data.Database
{
    public class PlatformContext : DBContext
    {
        public PlatformContext(string connectionString) : base(connectionString, false)
        {

        }
    }
}
