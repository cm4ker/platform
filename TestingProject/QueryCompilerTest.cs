using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryCompiler;
using SqlPlusDbSync.Data.Database;

namespace SqlPlusDbSync.UnitTest
{
    [TestClass]
    public class QueryCompilerTest
    {
        private PlatformContext _context;
        private DBQueryFactory _queryFactory;

        public QueryCompilerTest()
        {
            _context = new PlatformContext("Data Source=(local);Initial Catalog=asna_apt_194;Integrated Security=True");
            _queryFactory = new DBQueryFactory(_context);
        }


        [TestMethod]
        public void CreateDatabaseTest()
        {
            using (var db =
                new PlatformContext("Data Source=(local);Initial Catalog=SJDGHFJSDHFLJSD;Integrated Security=True"))
            {

            }
        }


        [TestMethod]
        public void SetClauseTest()
        {
            var set = new DBSetVariableClause(new DBVariable(SQLVariables.CONTEXT_INFO), new DBHexConstant(0x123));

            Console.WriteLine(set.Compile());
            set = new DBSetVariableClause(new DBVariable(SQLVariables.CONTEXT_INFO), new DBHexConstant(null));
            Console.WriteLine(set.Compile());
        }
    }
}
