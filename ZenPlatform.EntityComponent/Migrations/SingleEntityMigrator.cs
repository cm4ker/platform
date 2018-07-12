using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Data.Contracts.Entity;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.QueryBuilder.DDL.CreateTable;
using ZenPlatform.Shared;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.EntityComponent.Migrations
{
    //TODO: Пока конфигурация не хранится в базе данных, делать какие-то телодвижения в миграциях бессмысленно, ибо нам неоткуда взять дифф

    public class SingleEntityMigrator : IEntityMigrator
    {
        private readonly IXCConfigurationSettingsStore _store;


        /// <summary>
        /// Создать новую миграцию для сущности
        /// </summary>
        /// <param name="store">хранилище настроек</param>
        /// <param name="old">Старая конфигурация</param>
        /// <param name="actual">Новая конфигурация</param>
        public SingleEntityMigrator(IXCConfigurationSettingsStore store)
        {
            _store = store;
        }

        public IList<Node> GetScript(XCSingleEntity old, XCSingleEntity actual)
        {
            var result = new List<Node>();

            if (old == null)
            {
                /*
                 * Создаём новую сущность
                 * Для этого необходимо проделать следующие действия:
                 *  1) Присвоить имена новым таблицам и колонкам
                 *  2) Сгенерировать скрипт создания таблиц
                 */

                //TODO: Проверить, присваиваются ли идентификаторы для XCTypeBase.Id автоматически. Если нет, то этим должна заниматься конфигурация
                actual.RelTableName = $"ent{actual.Id}";

                var createTable =
                    new CreateTableQueryNode(actual.RelTableName, (t) => { });

                foreach (var prop in actual.Properties)
                {
                    //Генерируем по следующему алгоритму
                    prop.DatabaseColumnName = $"fld{prop.Id}";

                    var hasObjectGuid = false;
                    var hasObjectInt = false;
                    foreach (var type in prop.Types)
                    {
                        if (type is XCPremitiveType ptype)
                        {
                            ItemSwitch<XCPremitiveType>.Switch(ptype)
                                .CaseIs<XCBoolean>(i => createTable.WithColumn($"{prop.DatabaseColumnName}_{type.Name}", x => x.Boolean()))
                                .CaseIs<XCString>(i => createTable.WithColumn($"{prop.DatabaseColumnName}_{type.Name}", x => x.Varchar(i.ColumnSize)))
                                .CaseIs<XCDateTime>(i => createTable.WithColumn($"{prop.DatabaseColumnName}_{type.Name}", x => x.DateTime()))
                                .CaseIs<XCGuid>(i => createTable.WithColumn($"{prop.DatabaseColumnName}_{type.Name}", x => x.Guid()))
                                .CaseIs<XCNumeric>(i => createTable.WithColumn($"{prop.DatabaseColumnName}_{type.Name}", x => x.Numeric(i.Scale, i.Precision)))
                                .CaseIs<XCBinary>(i => createTable.WithColumn($"{prop.DatabaseColumnName}_{type.Name}", x => x.Varbinary(i.ColumnSize)));
                        }
                        else if (type is XCObjectTypeBase obj)
                        {
                            //TODO: по задумке дочерний компонент должен самостоятельно инкапсулировать поля для ссылки на него в чужие объекты
                            /*
                             * Опишим следующую ситуацию:
                             *
                             * У нас есть табличная часть. Она поставляется, как свойство в компонент.
                             * Но она не генерирует поле в основной таблице.
                             */

                            //Необхоидимо проверить, является ли тип принадлежащим к зависимому компоненту
                            if (obj.Parent.ComponentImpl.DatabaseObjectsGenerator.HasForeignColumn)
                            {
                                if (!hasObjectGuid)
                                {
                                    createTable.WithColumn($"{prop.DatabaseColumnName}_Ref", x => x.Guid());
                                    hasObjectGuid = true;
                                }
                                else if (!hasObjectInt)
                                {
                                    createTable.WithColumn($"{prop.DatabaseColumnName}_Type", x => x.Int());
                                    hasObjectInt = true;
                                }
                            }
                            //Если компонент приатаченный, в таком случае на свойство необходимо также запустить миграцию
                            if (actual.Parent.AttachedComponents.Contains(obj.Parent))
                            {
                                obj.Parent.ComponentImpl.Migrator.GetScript(null, obj);
                            }
                        }
                    }
                }

                result.Add(createTable);
            }

            if (actual == null)
            {
                result.Add(new DropTableQueryNode(old.RelTableName));
            }

            return result;
        }

        public IList<Node> GetScript(XCObjectTypeBase old, XCObjectTypeBase actual)
        {
            return GetScript((XCSingleEntity)old, (XCSingleEntity)actual);
        }
    }
}