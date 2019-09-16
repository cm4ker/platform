using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using ZenPlatform.Shared;

namespace ZenPlatform.DataComponent.Helpers
{
    /// <summary>
    /// Помошник для свойства элемента комопнента.
    /// Умеет по свойству получить все имена колонок базы данных\свойств которые должны быть.
    /// Это нужно для стандартизации наименования колонок, в частности мы повторяем код в EntityGenerator, EntityManager и EntityMigrator
    /// Когда мы хоим создать, выбрать или изменить, нам нужны имена колонок и их характеристики в более явном виде
    /// </summary>
    public static class ColumnsHelper
    {
        public static List<DatabaseColumnDefinitionItem> GetColumnsFromProperty(this XCObjectPropertyBase prop)
        {
            var result = new List<DatabaseColumnDefinitionItem>();

            //Генерируем по следующему алгоритму
            prop.DatabaseColumnName = $"fld{prop.Id}";

            var hasObjectGuid = false;
            var hasObjectInt = false;
            foreach (var type in prop.Types)
            {
                if (type is XCPrimitiveType ptype)
                {
                    result.Add(new DatabaseColumnDefinitionItem(prop, $"{prop.DatabaseColumnName}_{type.Name}", ptype));
                }
                else if (type is XCObjectTypeBase obj)
                {
                    //Необхоидимо проверить, является ли тип принадлежащим к зависимому компоненту
                    if (obj.Parent.ComponentImpl.DatabaseObjectsGenerator.HasForeignColumn)
                    {
                        if (!hasObjectGuid)
                        {
                            result.Add(new DatabaseColumnDefinitionItem(prop, $"{prop.DatabaseColumnName}_Ref",
                                new XCGuid()));
                            hasObjectGuid = true;
                        }
                        else if (!hasObjectInt)
                        {
                            result.Add(new DatabaseColumnDefinitionItem(prop, $"{prop.DatabaseColumnName}_Type",
                                new XCGuid()));
                            hasObjectInt = true;
                        }
                    }
                }
            }

            return result;
        }
    }

    /// <summary>
    /// Описание колонки
    /// </summary>
    public class DatabaseColumnDefinitionItem
    {
        public DatabaseColumnDefinitionItem(XCObjectPropertyBase property, string databaseColumnName,
            XCPrimitiveType type)
        {
            Property = property;
            DatabaseColumnName = databaseColumnName;
            Type = type;
        }

        //К какому свойству колонка относится
        public XCObjectPropertyBase Property { get; set; }

        /// <summary>
        /// Реальное имя колонки с префиксами и постфиксами
        /// </summary>
        public string DatabaseColumnName { get; set; }


        /// <summary>
        /// Тип колонки в базе данных
        /// </summary>
        public XCPrimitiveType Type { get; set; }
    }
}