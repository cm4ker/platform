using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryCompiler;
using SqlPlusDbSync.Data.Database;

namespace SqlPlusDbSync.UnitTest
{
    [TestClass]
    public class QueryCompilerTest
    {
        private AsnaDatabaseContext _context;
        private DBQueryCompiler _queryCompiler;

        public QueryCompilerTest()
        {
            _context = new AsnaDatabaseContext("Data Source=(local);Initial Catalog=asna_apt_194;Integrated Security=True");
            _queryCompiler = new DBQueryCompiler(_context);
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
