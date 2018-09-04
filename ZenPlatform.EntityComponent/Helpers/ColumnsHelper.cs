using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Editing;
using ZenPlatform.Configuration.Structure.Data.Types;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using ZenPlatform.Contracts;
using ZenPlatform.DataComponent;
using ZenPlatform.EntityComponent.Configuration;

namespace ZenPlatform.EntityComponent.Helpers
{
    /// <summary>
    /// Помошник колонок. С помощью него можно получить все необходимые колонки для сущности с типами
    /// </summary>
    public static class ColumnsHelper
    {
        public static List<DatabaseColumnDefinitionItem> GetByPropertyList(List<XCSingleEntityProperty> properties)
        {
            var result = new List<DatabaseColumnDefinitionItem>();
            //Генерируем свойства
            foreach (var prop in properties)
            {
                if (!prop.Types.Any())
                    throw new Exception("Property must have type!");

                if (prop.Types.Count == 1)
                {
                    var propType = prop.Types.First();
                    if (propType is XCPremitiveType)
                    {
                        
                    }
                    else if(propType is XCObjectTypeBase)
                    {
                    }
                }
            }

            return result;
        }
    }


    /// <summary>
    /// Элемент коллекции колонок базы данных для определённого свойства
    /// </summary>
    public class DatabaseColumnDefinitionItem
    {
        public DatabaseColumnDefinitionItem(XCObjectPropertyBase property, string databaseColumnName,
            XCPremitiveType type)
        {
            Property = property;
            DatabaseColumnName = databaseColumnName;
            Type = type;
        }

        /// <summary>
        /// Свойство объекта
        /// </summary>
        public XCObjectPropertyBase Property { get; set; }

        /// <summary>
        /// Имя колонки базы данных
        /// </summary>
        public string DatabaseColumnName { get; set; }

        /// <summary>
        /// Тип колонки
        /// </summary>
        public XCPremitiveType Type { get; set; }
    }
}