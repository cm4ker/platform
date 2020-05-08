using System.Collections.Generic;
using System.Data.Common;

namespace ZenPlatform.ServerRPC
{
    public static class DbReaderExtensions
    {
        public static void ToConsole(this DbDataReader reader)
        {
            List<List<string>> myData = new List<List<string>>(); //create list of list

            int fCount =
                reader.FieldCount; //i assume every row in the reader has the same number of field of the first row

            var header = new List<string>();
            for (int i = 0; i < fCount; i++)
            {
                header.Add(reader.GetName(i));
            }

            myData.Add(header);

            while (reader.Read())
            {
                //create list for the row
                List<string> myRow = new List<string>();
                myData.Add(myRow); //add the row to the list of rows
                for (int i = 0; i < fCount; i++)
                {
                    myRow.Add(reader[i].ToString()); //fill the row with field data
                }
            }

            string[,] arrValues = new string[myData.Count, fCount]; //create the array for the print class

            //go through the list and convert to an array
            //this could probably be improved 
            for (int i = 0; i < myData.Count; i++)
            {
                List<string> myRow = myData[i]; //get the list for the row
                for (int j = 0; j < fCount; j++)
                {
                    arrValues[i, j] = myRow[j]; //read the field
                }
            }

            ArrayPrinter.PrintToConsole(arrValues);
        }
    }
}