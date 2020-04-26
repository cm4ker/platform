namespace ZenPlatform.ServerRPC
{
    class Program
    {
        static void Main(string[] args)
        {
            Test.Run();
            
            // DbConnection con = new SqlConnection();
            //
            // con.ConnectionString =
            //     "Data source=(LocalDb)\\MSSQLLocalDB; Initial catalog=testdb; Integrated Security= true;"; // MultipleActiveResultSets=True";
            //
            // con.Open();
            //
            // var cmd = con.CreateCommand();
            // cmd.CommandText = "select name, object_id, uses_ansi_nulls from sys.tables";
            // var reader1 = cmd.ExecuteReader().GetBufferedData().GetDataReader();
            //
            // reader1.ToConsole();

            /*
             var query = Q"FROM Invoice SELECT Link = Invoice";
 
             var reader = query.Execute();
 
             while(reader.Read())
             {
                 //1. Map (Invoice alias -> cpecific column)
                 //2. Possible data type (Get real type from the DTO and wrap)
 
                  HERE I HAVE InvoiceLink type
                        V
                 var invoice = reader.Invoice;
 
                 |*
                 var breader = reader.Buffer();
 
                 while(breader.Read())
                 {
                   InvoiceLink v_0 = InvoiceManager.Get((Guid)breader["Fld_001"]);
 
                   if((int)breader["Fld_007_Type"] == 1)
                   {
 
                   }
                 }
                 
                 *|
             }
              */
        }
    }
}