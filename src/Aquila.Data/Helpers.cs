using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Npgsql;
using Aquila.QueryBuilder;

namespace Aquila.Data
{
    public static class DataReaderExtensions
    {
        /// <Summary>
        /// Map data from DataReader to list of objects
        /// </Summary>
        /// <typeparam name="T">Object</typeparam>
        /// <param name="dr">Data Reader</param>
        /// <returns>List of objects having data from data reader</returns>
        public static List<T> MapToList<T>(this DbDataReader dr) where T : new()
        {
            List<T> RetVal = null;
            var Entity = typeof(T);
            var PropDict = new Dictionary<string, PropertyInfo>();
            try
            {
                if (dr != null && dr.HasRows)
                {
                    RetVal = new List<T>();
                    var Props = Entity.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                    PropDict = Props.ToDictionary(p => p.Name.ToUpper(), p => p);
                    while (dr.Read())
                    {
                        T newObject = new T();
                        for (int Index = 0; Index < dr.FieldCount; Index++)
                        {
                            if (PropDict.ContainsKey(dr.GetName(Index).ToUpper()))
                            {
                                var Info = PropDict[dr.GetName(Index).ToUpper()];
                                if ((Info != null) && Info.CanWrite)
                                {
                                    var Val = dr.GetValue(Index);
                                    Info.SetValue(newObject, (Val == DBNull.Value) ? null : Val, null);
                                }
                            }
                        }

                        RetVal.Add(newObject);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return RetVal;
        }

        /// <Summary>
        /// Map data from DataReader to an object
        /// </Summary>
        /// <typeparam name="T">Object</typeparam>
        /// <param name="dr">Data Reader</param>
        /// <returns>Object having data from Data Reader</returns>
        public static T MapToSingle<T>(this DbDataReader dr) where T : new()
        {
            T RetVal = new T();
            var Entity = typeof(T);
            var PropDict = new Dictionary<string, PropertyInfo>();
            try
            {
                if (dr != null && dr.HasRows)
                {
                    var Props = Entity.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                    PropDict = Props.ToDictionary(p => p.Name.ToUpper(), p => p);
                    dr.Read();
                    for (int Index = 0; Index < dr.FieldCount; Index++)
                    {
                        if (PropDict.ContainsKey(dr.GetName(Index).ToUpper()))
                        {
                            var Info = PropDict[dr.GetName(Index).ToUpper()];
                            if ((Info != null) && Info.CanWrite)
                            {
                                var Val = dr.GetValue(Index);
                                Info.SetValue(RetVal, (Val == DBNull.Value) ? null : Val, null);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return RetVal;
        }
    }


    /// <summary>
    /// фабрика базы данных 
    /// </summary>
    public static class DatabaseFactory
    {
        /// <summary>
        /// Получить базу данных по её типу
        /// </summary>
        /// <param name="dbType">Тип базы данных</param>
        /// <param name="connectionString">Строка подключения</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">В случае если тип базы данных не поддерживается, значит выбрасывается этот тип исключения</exception>
        public static DbConnection Get(SqlDatabaseType dbType, string connectionString)
        {
            switch (dbType)
            {
                case SqlDatabaseType.Postgres: return new NpgsqlConnection(connectionString);
                case SqlDatabaseType.SqlServer: return new SqlConnection(connectionString);
                default: throw new NotSupportedException();
            }
        }
    }
}