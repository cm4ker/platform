using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration.Data.Contracts.Entity;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.QueryBuilder.Common.Columns;
using ZenPlatform.QueryBuilder.DML.Select;

namespace ZenPlatform.EntityComponent.QueryBuilders
{
    public class SingleEntityQueryInjector 
    {
        private readonly XCComponent _component;

        public SingleEntityQueryInjector(XCComponent component)
        {
            _component = component;
        }

        /// <inheritdoc />
        public SqlFragment GetDataSourceFragment(DataQueryConstructorContext context)
        {
//            if (_component.Types.FirstOrDefault(x => x.Name.ToLower() == context.ObjectName.ToLower()) is XCSingleEntity objType)
//                return new SqlFragment(new TableNode(objType.RelTableName), null);

            throw new Exception("object not found");
        }

        /// <inheritdoc />
        public SqlFragment GetColumnFragment(DataQueryConstructorContext context)
        {
//            if (_component.Types.FirstOrDefault(x => x.Name.ToLower() == context.ObjectName.ToLower()) is XCSingleEntity objType)
//            {
//                var field = objType.Properties.FirstOrDefault(x => x.Name.ToLower() == context.FieldName.ToLower());
//
//                //TODO: Если у нас сложное свойство, то нужно будет получить все колонки
//
//                if (field == null)
//                {
//                    throw new Exception("");
//                }
//
//                return new SqlFragment(new ColumnNode(field.DatabaseColumnName), null);
//            }

            throw new Exception("object not found");
        }
    }
}