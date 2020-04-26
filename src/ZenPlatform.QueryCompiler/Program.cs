namespace ZenPlatform.QueryBuilder
{
    class Program
    {
        //static void Main(string[] args)
        //{
        //    Console.WriteLine("Hello worlds");

        //    DbConnection conn = new SqlConnection("Data Source=(local);Initial Catalog=asna_apt_194;Integrated Security=True");
        //    conn.Open();

        //    SelectTest();
        //    UpdateTest();
        //    DeleteTest();
        //    SchemaTest();
        //    InsertTest();

        //    Console.ReadKey();
        //    conn.Close();
        //}

        //public static void SelectTest()
        //{
        //    DBTable table = new DBTable("Hdr", "H");
        //    DBTable table2 = new DBTable("Str", "S");

        //    var field = table.DeclareField("Hdr_idn");


        //    var field2 = table2.DeclareField("Str_idn");

        //    //(new DbSumCompileTransformation()).Apply(field);
        //    //(new DbSelectAliasTransformation("test")).Apply(field);

        //    var compiled = "";

        //    var query = new DBSelectQuery();
        //    query.AddFrom(table);
        //    query.AddJoin(table2).On(field, CompareType.Equals, field2);
        //    query.SelectAllFieldsFromSourceTables();
        //    query.Where(field, CompareType.Equals, field2);


        //    var query2 = new DBSelectQuery();
        //    var sub = query.GetAsSubQuery();
        //    sub.SetAliase("Subselect");
        //    query2.AddFrom(sub);
        //    query2.SelectAllFieldsFromSourceTables();


        //    //compiled = query.Compile();

        //    Console.WriteLine("=================== SELECT ===================");
        //    Console.WriteLine(query2.Compile());
        //    Console.WriteLine("=================== END SELECT ===================");


        //}

        //public static void UpdateTest()
        //{
        //    DBTable table = new DBTable("Hdr", "H");
        //    DBTable table2 = new DBTable("Str", "S");

        //    var field = table.DeclareField("Hdr_Unicode");
        //    var field8 = table.DeclareField("Hdr_idn");
        //    var field3 = table.DeclareField("Hdr_Sum01");
        //    var field4 = table.DeclareField("Hdr_Sum02");
        //    var field5 = table.DeclareField("Hdr_Sum03");
        //    var field6 = table.DeclareField("Hdr_Sum04");
        //    var field7 = table.DeclareField("Hdr_Sum05");

        //    var field2 = table2.DeclareField("Hdr_Unicode");

        //    var query = new DBUpdateQuery();

        //    query.AddFrom(table);
        //    query.AddJoin(table2, JoinType.Inner).On(field, CompareType.Equals, field2);
        //    query.AddField(field);
        //    query.AddField(field8);
        //    query.AddField(field3);
        //    query.AddField(field4);
        //    query.AddField(field5);
        //    query.AddField(field6);
        //    query.AddField(field7);

        //    query.Where(field, CompareType.Equals, new DBConstant("test"));

        //    Console.WriteLine("=================== UPDATE ===================");
        //    Console.WriteLine(query.Compile());
        //    foreach (var parameter in query.Parameters)
        //    {
        //        Console.WriteLine("Parameter name: {0}, Parameter type: {1}, Parameter value {2}", parameter.SqlParameter.ParameterName, parameter.SqlParameter.DbType, parameter.SqlParameter.Value);
        //    }
        //    Console.WriteLine("=================== END UPDATE ===================");
        //}
        //public static void DeleteTest()
        //{
        //    DBTable table = new DBTable("Hdr", "H");
        //    DBTable table2 = new DBTable("Str", "S");

        //    var field = table.DeclareField("Hdr_Unicode");
        //    var field2 = table2.DeclareField("Hdr_Unicode");

        //    var query = new DBDeleteQuery();

        //    query.AddFrom(table);
        //    query.AddJoin(table2, JoinType.Left).On(field, CompareType.Equals, field2);

        //    query.DeleteTable = table;

        //    query.Where(field, CompareType.Equals, field2);

        //    Console.WriteLine("=================== DELETE ===================");
        //    Console.WriteLine(query.Compile());
        //    Console.WriteLine("=================== END DELETE ===================");
        //}
        //public static void InsertTest()
        //{
        //    DBTable table = new DBTable("Hdr", "H");
        //    DBTable table2 = new DBTable("Str", "S");

        //    var field = table.DeclareField("Hdr_Unicode");
        //    var field2 = table2.DeclareField("Hdr_Unicode");

        //    var query = new DBInsertQuery();

        //    query.AddField(field);

        //    Console.WriteLine("=================== INSERT ===================");
        //    Console.WriteLine(query.Compile());
        //    Console.WriteLine("=================== END INSERT ===================");
        //}
        //public static void SchemaTest()
        //{
        //}
    }
}
