using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Text;
using QueryCompiler;
using QueryCompiler.Queries;

namespace SqlPlusDbSync.Platform
{
    public static class PlatformHelper
    {
        private const string IdentityParameterConst = "@identity";
        private const string InsertActionConst = "insert";
        private const string DeleteActionConst = "delete";
        private const string UpdateActionConst = "update";
        private const string SObjectNameConst = "$sobject";
        private const string ObjectVersionConst = "$version";
        private const string ObjectKeyConst = "$keyField";
        private const string RegisterObjectConst = "$register";
        private const string OwnerObjectConst = "$owner";

        private const string AfterLoadConst = "AfterLoad";
        private const string BeforeSaveConst = "BeforeSave";
        private const string BeforeDeleteConst = "BeforeDelete";

        public static string IdentityParameter = IdentityParameterConst;
        public static string InsertAction = InsertActionConst;
        public static string DeleteAction = DeleteActionConst;
        public static string UpdateAction = UpdateActionConst;


        #region Events
        public static string AfterLoad = AfterLoadConst;
        public static string BeforeSave = BeforeSaveConst;
        public static string BeforeDelete = BeforeDeleteConst;
        #endregion


        public static string IdServeceField = "$id";
        public static string ActionServeceField = "$action";
        public static string ObjectKeyField = ObjectKeyConst;
        public static string SObjectNameField = SObjectNameConst;
        public static string ObjectVersionField = ObjectVersionConst;
        public static string RegisterObjectField = RegisterObjectConst;
        public const string OwnerObjectField = OwnerObjectConst;




        public static DBTable GetDBTable(this TableType sType, DBQueryCompiler qc)
        {
            TableType to = sType;

            var table = qc.CreateTable(to.Table.Name, to.GetFullName());

            foreach (var field in to.Fields)
            {
                table.DeclareField(field.Name);
            }

            return table;
        }

        public static DBTable GetVersionTable(DBQueryCompiler qc)
        {
            DBTable table = qc.CreateTable("__Versions");

            table.DeclareField("TableName");
            table.DeclareField("Id");
            table.DeclareField("Version");
            table.DeclareField("PointId");

            return table;
        }

        public static DBTable GetMetadataTable(DBQueryCompiler qc)
        {
            DBTable table = qc.CreateTable("__Metadata");

            table.DeclareField("Id");
            table.DeclareField("Metadata");

            return table;
        }

        public static SaveAction ParseSaveAction(string str)
        {
            switch (str)
            {
                case InsertActionConst: return SaveAction.Insert;
                case DeleteActionConst: return SaveAction.Delete;
                case UpdateActionConst: return SaveAction.Update;
                default: throw new NotSupportedException();
            }
        }

        public static void ExecNonQueryBatch(this SqlConnection conn, DBBatch batch)
        {
            if (batch.Parameters.Count > 2000)
            {
                using (var cmd = conn.CreateCommand())
                {
                    var tran = conn.BeginTransaction(IsolationLevel.Snapshot);
                    cmd.Transaction = tran;

                    foreach (var query in batch.Queryes)
                    {
                        if (query is IDataChangeQuery)
                            foreach (var param in (query as IDataChangeQuery).Parameters)
                            {
                                cmd.Parameters.Add(param.SqlParameter);
                            }

                        cmd.CommandText = query.Compile();
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }
                    tran.Commit();
                }
            }
            else
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = batch.Compile();
                    foreach (var parameter in batch.Parameters)
                    {
                        cmd.Parameters.Add(parameter.SqlParameter);
                    }
                    var tran = conn.BeginTransaction(IsolationLevel.Snapshot);
                    cmd.Transaction = tran;
                    var fetched = cmd.ExecuteNonQuery();
                    tran.Commit();
                }
            }
        }
        public static TypeWrapper GetContainer(object obj)
        {
            var type = obj.GetType();
            var genericType = typeof(TypeWrapper<>).MakeGenericType(type);
            return Activator.CreateInstance(genericType, obj) as TypeWrapper;
        }
        public static object GetObject(object obj)
        {
            var tw = obj as TypeWrapper;
            if (tw is null) return null;

            return tw.ObjectValue;
        }

        public static object GetPObjectValue(this object obj, string name)
        {
            var dynObj = obj as PObject;
            if (dynObj == null) return null;

            if (dynObj.Properties.TryGetValue(name.ToLower(), out var item))
                return GetObject(item);
            return null;
        }

        public static PObject GetDynObject(this object obj, string name)
        {
            if (obj is null) return null;
            return obj.GetPObjectValue(name.ToLower()) as PObject;
        }

        public static List<PObject> GetDynObjectCollection(this object obj, string name)
        {
            if (obj is null) return null;
            return obj.GetPObjectValue(name.ToLower()) as List<PObject>;
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }


        public static byte[] GetInitialVersion()
        {
            return new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        }



        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int memcmp(byte[] b1, byte[] b2, long count);

        public static int ByteArrayCompare(byte[] b1, byte[] b2)
        {
            if (b1 is null || b2 is null) return 0;
            // Validate buffers are the same length.
            // This also ensures that the count does not exceed the length of either buffer.  
            if (b1.Length == b2.Length)
                return memcmp(b1, b2, b1.Length);
            else return 0;
        }

        public static Type GetClrType(SqlDbType sqlType, bool isNullable = false)
        {
            if (isNullable)
            {
                switch (sqlType)
                {
                    case SqlDbType.BigInt:
                        return typeof(long?);

                    case SqlDbType.Binary:
                    case SqlDbType.Image:
                    case SqlDbType.Timestamp:
                    case SqlDbType.VarBinary:
                        return typeof(byte[]);

                    case SqlDbType.Bit:
                        return typeof(bool?);

                    case SqlDbType.Char:
                    case SqlDbType.NChar:
                    case SqlDbType.NText:
                    case SqlDbType.NVarChar:
                    case SqlDbType.Text:
                    case SqlDbType.VarChar:
                    case SqlDbType.Xml:
                        return typeof(string);

                    case SqlDbType.DateTime:
                    case SqlDbType.SmallDateTime:
                    case SqlDbType.Date:
                    case SqlDbType.Time:
                    case SqlDbType.DateTime2:
                        return typeof(DateTime?);

                    case SqlDbType.Decimal:
                    case SqlDbType.Money:
                    case SqlDbType.SmallMoney:
                        return typeof(decimal?);

                    case SqlDbType.Float:
                        return typeof(double?);

                    case SqlDbType.Int:
                        return typeof(int?);

                    case SqlDbType.Real:
                        return typeof(float?);

                    case SqlDbType.UniqueIdentifier:
                        return typeof(Guid?);

                    case SqlDbType.SmallInt:
                        return typeof(short?);

                    case SqlDbType.TinyInt:
                        return typeof(byte?);

                    case SqlDbType.Variant:
                    case SqlDbType.Udt:
                        return typeof(object);

                    case SqlDbType.Structured:
                        return typeof(DataTable);

                    case SqlDbType.DateTimeOffset:
                        return typeof(DateTimeOffset?);

                    default:
                        throw new ArgumentOutOfRangeException("sqlType");
                }
            }
            else
            {
                switch (sqlType)
                {
                    case SqlDbType.BigInt:
                        return typeof(long);

                    case SqlDbType.Binary:
                    case SqlDbType.Image:
                    case SqlDbType.Timestamp:
                    case SqlDbType.VarBinary:
                        return typeof(byte[]);

                    case SqlDbType.Bit:
                        return typeof(bool);

                    case SqlDbType.Char:
                    case SqlDbType.NChar:
                    case SqlDbType.NText:
                    case SqlDbType.NVarChar:
                    case SqlDbType.Text:
                    case SqlDbType.VarChar:
                    case SqlDbType.Xml:
                        return typeof(string);

                    case SqlDbType.DateTime:
                    case SqlDbType.SmallDateTime:
                    case SqlDbType.Date:
                    case SqlDbType.Time:
                    case SqlDbType.DateTime2:
                        return typeof(DateTime);

                    case SqlDbType.Decimal:
                    case SqlDbType.Money:
                    case SqlDbType.SmallMoney:
                        return typeof(decimal);

                    case SqlDbType.Float:
                        return typeof(double);

                    case SqlDbType.Int:
                        return typeof(int);

                    case SqlDbType.Real:
                        return typeof(float);

                    case SqlDbType.UniqueIdentifier:
                        return typeof(Guid);

                    case SqlDbType.SmallInt:
                        return typeof(short);

                    case SqlDbType.TinyInt:
                        return typeof(byte);

                    case SqlDbType.Variant:
                    case SqlDbType.Udt:
                        return typeof(object);

                    case SqlDbType.Structured:
                        return typeof(DataTable);

                    case SqlDbType.DateTimeOffset:
                        return typeof(DateTimeOffset);

                    default:
                        throw new ArgumentOutOfRangeException("sqlType");
                }
            }
        }
    }


}