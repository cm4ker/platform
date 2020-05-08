using System;
using System.Collections.Generic;
using System.Linq;

namespace Aquila.QueryBuilder.Queries
{
   public class DBMigrationQueryBuilder
    {
        public DBMigrationQueryBuilder()
        {

        }

        /// <summary>
        /// Создает инструкции SQL на изменение таблицы. Переимменование таблиц не поддержиается. 
        /// Не найденые столбцы в oldTable будут добавлены.
        /// Не найденые столбцы в newTable будут удалены.
        /// У одинаковых столбцов будет изменен тип, если необходимо.
        /// Поиск столбцов осуществляется по имени.
        /// </summary>
        /// <param name="oldTable">Исходная таблица</param>
        /// <param name="newTable">Новая таблица</param>
        /// <returns></returns>
        IQueryable BuildChangeTable(DBTable oldTable, DBTable newTable)
        {

            if (oldTable.Name != newTable.Name) throw new NotSupportedException("Renaming tables not supported ");

            DBMultiQuery query = new DBMultiQuery();

            var toCreateList =
                from newField in newTable.Fields
                join oldField in oldTable.Fields
                    on newField.Name equals oldField.Name
                    into outer
                from field in outer.DefaultIfEmpty()
                where field is null
                select newField;

            DBAlterTableQuery alterTableQuery = new DBAlterTableQuery();
            alterTableQuery.AddColumns(toCreateList);
            query.AddQuery(alterTableQuery);

            var toDropList =
                from oldField in oldTable.Fields
                join newField in newTable.Fields
                    on oldField.Name equals newField.Name
                    into outer
                from field in outer.DefaultIfEmpty()
                where field is null
                select oldField;

            alterTableQuery = new DBAlterTableQuery();
            alterTableQuery.DropColumns(toDropList);
            query.AddQuery(alterTableQuery);

            var toAlterList =
                from newField in oldTable.Fields
                join oldField in newTable.Fields
                    on newField.Name equals oldTable.Name
                where !newField.Schema.Equals(oldField.Schema)
                select newField;

            alterTableQuery = new DBAlterTableQuery();
            alterTableQuery.AlterColumns(toAlterList);
            query.AddQuery(alterTableQuery);

            return query;
        }

        IQueryable BuildCreateTable(DBTable table)
        {
            return new DBCreateTableQuery(table);
        }

        
    }
}
