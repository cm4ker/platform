using System;
using System.Linq;
using ZenPlatform.Configuration.Data.Contracts.Entity;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.QueryBuilder.Common.Columns;
using ZenPlatform.QueryBuilder.DML.Select;

namespace ZenPlatform.EntityComponent.QueryBuilders
{
    public class SingleEntityQueryInjector : IQueryInjector
    {
        private readonly XCComponent _component;

        public SingleEntityQueryInjector(XCComponent component)
        {
            _component = component;
        }

        /// <inheritdoc />
        public SqlNode GetDataSourceFragment(string objectName)
        {
            if (_component.Types.FirstOrDefault(x => x.Name.ToLower() == objectName.ToLower()) is XCSingleEntity objType)
                return new TableNode(objType.RelTableName);

            throw new Exception("object not found");
        }

        /// <inheritdoc />
        public SqlNode GetColumnFragment(string objectName, string fieldName)
        {
            if (_component.Types.FirstOrDefault(x => x.Name.ToLower() == objectName.ToLower()) is XCSingleEntity objType)
            {
                var field = objType.Properties.FirstOrDefault(x => x.Name.ToLower() == fieldName.ToLower());

                //TODO: Если у нас сложное свойство, то нужно будет получить все колонки

                if (field == null)
                {
                    throw new Exception("");
                }

                return new ColumnNode(field.DatabaseColumnName);
            }

            throw new Exception("object not found");
        }
    }
}