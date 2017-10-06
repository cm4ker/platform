using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryCompiler;
using QueryCompiler.Queries;
using SqlPlusDbSync.Data.Database;
using SqlPlusDbSync.Platform;
using SqlPlusDbSync.Platform.Configuration;
using SqlPlusDbSync.Platform.EntityObject;

namespace SqlPlusDbSync.UnitTest
{
    [TestClass]
    public class MainTests
    {
        private AsnaDatabaseContext _context;
        private DBQueryCompiler _queryCompiler;
        private Core _core;

        public MainTests()
        {
            _context = new AsnaDatabaseContext("Data Source=(local);Initial Catalog=asna_apt_194;Integrated Security=True");
            _queryCompiler = new DBQueryCompiler(_context);
            _core = new Core(_context);
        }

        [TestMethod]
        public void SetIdentityTesting()
        {
            DBQueryCompiler qc = new DBQueryCompiler(_context);
            var batch = new DBBatch();

            batch.AddQuery(qc.CreateSetIdentity("Hdr", true));
            batch.AddQuery(qc.CreateSetIdentity("Hdr", false));

            Console.WriteLine(batch.Compile());

        }

        [TestMethod]
        public void TestRulesLoading()
        {
            Core c = new Core(_context);
        }

        [TestMethod]
        public void CaseClauseTase()
        {
            var log = new DBLogicalClause();
            log.Where(DBClause.CreateParameter("@test", SqlDbType.Char), CompareType.Equals,
                DBClause.CreateConstant(10), false);
            DBCaseClause c = new DBCaseClause(log, DBClause.CreateConstant(1), DBClause.CreateConstant(2));
            Console.WriteLine(c.Compile());
        }

        [TestMethod]
        public void SelectTest()
        {
            DBTable table = _queryCompiler.CreateTable("Hdr", "H");
            DBTable table2 = _queryCompiler.CreateTable("Str", "S");


            table.FillFieldsFromSchema();
            table2.FillFieldsFromSchema();

            //(new DbSumCompileTransformation()).Apply(field);
            //(new DbSelectAliasTransformation("test")).Apply(field);

            var query = new DBSelectQuery();
            query.From(table)
                 .Join(table2)
                 .On(table.GetField("HDR_UNICODE"), CompareType.Equals, table2.GetField("HDR_UNICODE"));

            query.SelectAllFieldsFromSourceTables();



            var param = DBClause.CreateParameter("Hdr_Unicode", SqlDbType.UniqueIdentifier);
            query.Where(table.GetField("Prm_Unicode"), CompareType.Equals, DBClause.CreateConstant(11))
                .AndOr(table.GetField("Hdr_unicode"), CompareType.Equals, param)
                .And(table2.GetField("Hdr_unicode"), CompareType.Equals, param)
                .Or(table2.GetField("Str_Qnt"), CompareType.GreatThen, table2.GetField("Str_BoxQnt"));

            var query2 = new DBSelectQuery();
            var sub = query.GetAsSubQuery();

            sub.SetAliase("Subselect");
            query2.From(sub);
            query2.SelectAllFieldsFromSourceTables();

            var first = query2.Fields.First();
            query2.OrderBy(first);
            query2.Top(100);
            query2.Into("#tmp_table");

            Console.WriteLine(query2.Compile());

        }

        [TestMethod]
        public void GroupByTest()
        {
            DBSelectQuery query = new DBSelectQuery();
            DBTable table = _queryCompiler.CreateTable("Hdr", "H");
            table.FillFieldsFromSchema();
            query.From(table);
            query.SelectAllFieldsFromSourceTables();
            query.GroupBy(table.Fields.First());
            Console.Write(query.Compile());
        }

        [TestMethod]
        public void UpdateTest()
        {
            DBTable table = _queryCompiler.CreateTable("Hdr", "H");
            DBTable table2 = _queryCompiler.CreateTable("Str", "S");

            var field = table.DeclareField("Hdr_Unicode");
            var field8 = table.DeclareField("Hdr_idn");
            var field3 = table.DeclareField("Hdr_Sum01");
            var field4 = table.DeclareField("Hdr_Sum02");
            var field5 = table.DeclareField("Hdr_Sum03");
            var field6 = table.DeclareField("Hdr_Sum04");
            var field7 = table.DeclareField("Hdr_Sum05");

            var field2 = table2.DeclareField("Hdr_Unicode");

            var query = new DBUpdateQuery();

            query.AddFrom(table);
            query.AddJoin(table2, JoinType.Inner).On(field, CompareType.Equals, field2);
            query.AddField(field);
            query.AddField(field8);
            query.AddField(field3);
            query.AddField(field4);
            query.AddField(field5);
            query.AddField(field6);
            query.AddField(field7);

            query.Where(field, CompareType.Equals, new DBConstant("test"));

            Console.WriteLine("=================== UPDATE ===================");
            Console.WriteLine(query.Compile());
            foreach (var parameter in query.Parameters)
            {
                Console.WriteLine("Parameter name: {0}, Parameter type: {1}, Parameter value {2}", parameter.SqlParameter.ParameterName, parameter.SqlParameter.DbType, parameter.SqlParameter.Value);
            }
            Console.WriteLine("=================== END UPDATE ===================");
        }

        [TestMethod]
        public void DeleteTest()
        {
            DBTable table = _queryCompiler.CreateTable("Hdr", "H");
            DBTable table2 = _queryCompiler.CreateTable("Str", "S");

            var field = table.DeclareField("Hdr_Unicode");
            var field2 = table2.DeclareField("Hdr_Unicode");

            var query = new DBDeleteQuery();

            query.AddFrom(table);
            query.AddJoin(table2, JoinType.Inner).On(field, CompareType.Equals, field2);

            query.DeleteTable = table;

            query.Where(field, CompareType.Equals, field2);

            Console.WriteLine("=================== DELETE ===================");
            Console.WriteLine(query.Compile());
            Console.WriteLine("=================== END DELETE ===================");
        }

        [TestMethod]
        public void InsertTest()
        {
            DBTable table = _queryCompiler.CreateTable("Hdr", "H");
            DBTable table2 = _queryCompiler.CreateTable("Str", "S");

            var field = table.DeclareField("Hdr_Unicode");
            var field2 = table2.DeclareField("Hdr_Unicode");

            var query = new DBInsertQuery();

            query.AddField(field);

            Console.WriteLine("=================== INSERT ===================");
            Console.WriteLine(query.Compile());
            Console.WriteLine("=================== END INSERT ===================");
        }

        [TestMethod]
        public void SchemaTest()
        {
        }

        [TestMethod]
        public void CoreQueryTest()
        {
            var core = new Core(_context);
            var result = core.GetChangedIdFromVersion(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x02, 0x94, 0x2A, 0xC0 },
                Guid.Parse("99E82819-A9B4-4085-B02C-3A5F5D9DB3C5"));
        }

        [TestMethod]
        public void IdentySelectGraphTest()
        {
            var core = new Core(_context);
            var sobj = core.SupportedObjects.First(x => x.Name.Contains("ДокументЧек"));
            var oc = new SObjectQueryProcessor(_context);
            var query = oc.GetSelectGraphQuery(sobj);
            Console.WriteLine(query.Compile());
        }

        //[TestMethod]
        public void MappingTest()
        {
            EntityGenerator eg = new EntityGenerator();
            eg.Generate(_core);

            var sw = new Stopwatch();

            Mapper mapper = new Mapper();

            var proc = new SObjectQueryProcessor(_context);
            var sobject = _core.SupportedObjects.First(x => x.Name.Contains("ПриходОстатки28"));

            SObjectEntityManager mgr = new SObjectEntityManager(_context, sobject);

            var query = proc.GetSingleSelectQuery(sobject);
            query.Parameters[PlatformHelper.IdentityParameter].SetValue(Guid.Parse("B23AA8BD-1DD3-44B7-8E7C-FAF1BC0CE693"));

            var asm = Assembly.Load("EntityAssembly");
            var type = asm.GetTypes().FirstOrDefault(x => x.Name == "ПриходОстатки28");

            sw.Start();
            for (int i = 0; i < 100; i++)
            {
                mgr.Load(Guid.Parse("B23AA8BD-1DD3-44B7-8E7C-FAF1BC0CE693"));
            }
            sw.Stop();
            Console.WriteLine($"Full reflection loading: {sw.Elapsed}ms");

            DTOObject dtoObject = Activator.CreateInstance(type) as DTOObject;

            sw.Reset();
            using (var cmd = _context.CreateCommand(query.Compile()))
            {
                foreach (var param in query.Parameters)
                {
                    cmd.Parameters.Add(param.SqlParameter);
                }
                sw.Start();
                for (int i = 0; i < 100; i++)
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        //mgr.Load(Guid.Parse("B23AA8BD-1DD3-44B7-8E7C-FAF1BC0CE693"));
                        mapper.Map(dtoObject, sobject, reader);
                    }
                }
                sw.Stop();
            }

            Console.WriteLine($"Reflection class mapping: {sw.Elapsed}ms");
            sw.Reset();

            proc = new SObjectQueryProcessor(_context);
            sw.Start();
            for (int i = 0; i < 100; i++)
            {
                // _core.GetObject(sobject, Guid.Parse("B23AA8BD-1DD3-44B7-8E7C-FAF1BC0CE693"), proc);
            }
            sw.Stop();

            Console.WriteLine($"Platform dynamic object: {sw.Elapsed}ms");
        }

        [TestMethod]
        public void MarkObjectTest()
        {
            EntityGenerator eg = new EntityGenerator();
            eg.Generate(_core);

            var sw = new Stopwatch();

            var sobject = _core.SupportedObjects.First(x => x.Name.Contains("ПриходОстатки28"));
            SObjectEntityManager mgr = new SObjectEntityManager(_context, sobject);
            var obj = mgr.Load(Guid.Parse("B23AA8BD-1DD3-44B7-8E7C-FAF1BC0CE693")) as DTOObject;
            sw.Start();
            mgr.MarkObject(obj, obj);
            sw.Stop();
            Console.WriteLine($"Marking object complete: {sw.Elapsed}ms");
        }

        [TestMethod]
        public void EntitySaveQueryGenerateAndExecute()
        {
            var sobject = _core.SupportedObjects.First(x => x.Name.Contains("ПриходОстатки28"));
            SObjectEntityManager mgr = new SObjectEntityManager(_context, sobject);
            var obj = mgr.Load(Guid.Parse("B23AA8BD-1DD3-44B7-8E7C-FAF1BC0CE693")) as DTOObject;
            mgr.Save(obj);
        }

        [TestMethod]
        public void EntityDeleteQueryGenerateAndExecute()
        {
            var sobject = _core.SupportedObjects.First(x => x.Name.Contains("ПриходОстатки28"));
            SObjectEntityManager mgr = new SObjectEntityManager(_context, sobject);
            var obj = mgr.Load(Guid.Parse("B23AA8BD-1DD3-44B7-8E7C-FAF1BC0CE693")) as DTOObject;
            SObjectQueryProcessor proc = new SObjectQueryProcessor(_context);
            var batch = proc.GetDeleteQuery(obj, sobject);
        }

        [TestMethod]
        public void EntityPackageGenerationSql()
        {
            var sobject = _core.SupportedObjects.First(x => x.Name.Contains("ПриходОстатки28"));
            SObjectQueryProcessor op = new SObjectQueryProcessor(_context);
            var query = op.GetPackageQuery(sobject);
            Console.WriteLine(query.Compile());
        }

        [TestMethod]
        public void EntityGetPackageFromXML()
        {
            var sobject = _core.SupportedObjects.First(x => x.Name.Contains("ПриходАптека"));
            SObjectQueryProcessor op = new SObjectQueryProcessor(_context);

            var xml = @"
<Object><SUSR_ID>2</SUSR_ID><Skd_UniCode>52</Skd_UniCode><Prm_UniCode>1</Prm_UniCode><Hdr_Obj2Type>3</Hdr_Obj2Type><Hdr_Obj2Code>5833</Hdr_Obj2Code><Hdr_ObjDopCode1>0</Hdr_ObjDopCode1><Hdr_OperCode>2</Hdr_OperCode><Hdr_Oper2Code>0</Hdr_Oper2Code><Hdr_NumDcm>3395</Hdr_NumDcm><Hdr_Date>2017-09-05T00:00:00</Hdr_Date><Hdr_IntDate>2017-09-05T16:18:32</Hdr_IntDate><Hdr_CalcYN>0</Hdr_CalcYN><DcmState>-6</DcmState><Hdr_ExchType1>1</Hdr_ExchType1><Hdr_ExchRate1>0.0000</Hdr_ExchRate1><Hdr_Value0_07>102</Hdr_Value0_07><Hdr_Value0_08>1</Hdr_Value0_08><Hdr_Value0_09>0</Hdr_Value0_09><Hdr_Value2_01>0.00</Hdr_Value2_01><Hdr_Value2_03>0.00</Hdr_Value2_03><Hdr_Value2_05>0.00</Hdr_Value2_05><Hdr_Value2_06>0.00</Hdr_Value2_06><Hdr_Value2_07>0.00</Hdr_Value2_07><Hdr_Value2_08>0.00</Hdr_Value2_08><Hdr_Value2_09>0.00</Hdr_Value2_09><Hdr_Value2_10>0.00</Hdr_Value2_10><Hdr_Value4_04>0.0000</Hdr_Value4_04><Hdr_Value4_06>0.0000</Hdr_Value4_06><Hdr_Value4_07>0.0000</Hdr_Value4_07><Hdr_Value4_08>0.0000</Hdr_Value4_08><Hdr_Value4_09>0.0000</Hdr_Value4_09><Hdr_Value4_10>0.0000</Hdr_Value4_10><Hdr_Date1>2017-09-05T00:00:00</Hdr_Date1><Hdr_BoolFld01YN>0</Hdr_BoolFld01YN><Hdr_BoolFld02YN>0</Hdr_BoolFld02YN><Hdr_BoolFld03YN>0</Hdr_BoolFld03YN><Hdr_UsrChFlag>0</Hdr_UsrChFlag><Hdr_UniCode>06770419-0E19-4520-9A27-E20FFC6B73AE</Hdr_UniCode><Hdr_IDN>76684</Hdr_IDN><ModifyDateTime>2017-09-05T16:18:32</ModifyDateTime></Object>
";
            var query = op.GetSelectFromXml(sobject, xml);
            Console.WriteLine(query.Compile());
        }

        //[TestMethod]
        public void TestPerformance()
        {
            var skdList = new List<Guid>
            {
                Guid.Parse("640C21AB-F038-4FB4-BBB3-1C396BE45C95"),
                Guid.Parse("99E82819-A9B4-4085-B02C-3A5F5D9DB3C5"),
                Guid.Parse("E2AC2090-DD9A-402E-B81A-611EE7D85F31"),
                Guid.Parse("77AB8A28-5B9E-43B9-830B-9B907550E9FA"),
                Guid.Parse("1552223C-E481-4BA5-BC6B-FC956F803045")
            };

            var objects = _core.GetChangedEntityes(PlatformHelper.GetInitialVersion(), skdList);
        }

        [TestMethod]
        public void EntityPackageGenerationSqlWithUnion()
        {
            var sobject = _core.SupportedObjects.First(x => x.Name.Contains("ПриходОстатки28"));
            SObjectQueryProcessor op = new SObjectQueryProcessor(_context);
            var query = op.GetPackageQuery(sobject);
            Console.WriteLine(query.Compile());
        }

        /// <summary>
        /// Temporary depricated
        /// </summary>
        [TestMethod]
        public void GetOwnerObject()
        {
            //var core = new Core(_context, true);
            //core.LoadRulesFromFile("rules.dbe");
            //var sobj = core.SupportedObjects.First(x => x.Name.Contains("МежскладскаяПередача2"));
            //var proc = new SObjectProcessor(_context);
            //var dynobj = core.GetObject(sobj, Guid.Parse("B8E328F7-FFA9-4A8A-8760-5F2E32D7DD8D"), proc);
            //Console.WriteLine(dynobj[PlatformHelper.OwnerObjectField]);
        }

    }
}
