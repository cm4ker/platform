//using System.Collections.Generic;
//using Aquila.Configuration.Data.Types.Complex;
//using Aquila.Core;
//
//namespace Aquila.DataComponent.Migrations
//{
//    /// <summary>
//    /// Комопнент предназначенный для миграции сущностей
//    /// </summary>
//    public abstract class DataComponentMigrationBase
//    {
//        private List<DataComponentObjectMigrationBase> _migrators;
//
//        protected DataComponentMigrationBase(SystemSession session)
//        {
//            Session = session;
//            _migrators = new List<DataComponentObjectMigrationBase>();
//        }
//
//        public SystemSession Session { get; }
//
//        public void AddMigrator(DataComponentObjectMigrationBase migrator)
//        {
//            _migrators.Add(migrator);
//        }
//
//        public void ApplyMigration(PObjectType oldObject, PObjectType newObject)
//        {
//            //TODO: Найти первый подходящий мигратор для объекта и сделать миграцию... 
//        }
//    }
//
//
//    public abstract class DataComponentObjectMigrationBase
//    {
//    }
//
//    /// <summary>
//    /// Компонент, который отвечает за миграцию конкретного объекта. Здесь заложена логика его мигрирования
//    /// У объекта есть несколько состояний в которых он может находится
//    /// 1) Создан - Объект только лишь создан и не содержит в себе никаких данных
//    /// 2) Обработан - Объект выполнил подготовку к мигрированию (сгенерировал скрипты и так далее)
//    /// 3) Выполнен - Объект мигрировал
//    /// </summary>
//    public abstract class DataComponentObjectMigrationBase<T>
//        where T : PObjectType
//    {
//        protected DataComponentObjectMigrationBase(SystemSession session, T newObjectType, T oldObjectType)
//        {
//            Session = session;
//            NewObjectType = newObjectType;
//            OldObjectType = oldObjectType;
//        }
//
//        public abstract void Prepare();
//        public abstract void Migrate();
//
//        public ObjectMigrationStatus MigrationStatus { get; set; }
//        public MigrationChangesType MigrationChangesType { get; set; }
//
//        protected T NewObjectType { get; }
//
//        protected T OldObjectType { get; }
//
//        protected SystemSession Session { get; }
//    }
//
//    public enum MigrationChangesType
//    {
//        //Then status = Created
//        None,
//        Success,
//        Warning,
//        Breaking,
//        Error
//    }
//
//    public enum ObjectMigrationStatus
//    {
//        /// <summary>
//        /// Миграция создана и готова для дальнейших манипуляций
//        /// </summary>
//        Created,
//
//        /// <summary>
//        /// Миграция подготовлена. Созданы все условия для миграции. Стало понятно какие изменения будут
//        /// внесены в базу данных. Какой уровень вмешательства будет в базу.
//        /// </summary>
//        Prepared,
//
//        /// <summary>
//        /// Миграция полностью завершила свою работу
//        /// </summary>
//        Complited
//    }
//    /*
//     *API для реализации
//     *
//     * Тезисы к реализации:
//     * 1) Про мигрирование должен знать каждый компонент, это однозначно.
//     * 2) Вышестоящие компоненты (Конфигурация, Базовая часть платформы, и так далее... ) должны возможность
//     * получить какую-то обратную связь.
//     * 3) Все интерфейсы, необходимые для реализации не являются обязательными, они запечатаны в компоненте и
//     * лишь помогают ориентироваться. Т.е. внутри компонента может быть сколь угодно сложный механизм отслеживания
//     * измеений
//     *
//     * public abstract class
//     *
//     *
//     */
//
//    /*
//     * Миграции -  очень комплексная вещь.
//     * Для того, чтобы сделать миграцию, необходимо иметь два операнда: исходная конфигурация и обновлённая конфигурация.
//     *
//     * После этого необходимо сравнивать два объекта PObjectType. Для каждого компонента PObjectType разворачивается в описательную часть компонента (для
//     * компонента типа "Документ" будет использоваться сущность PDocumentObjectType в которой существуюет расширенное описание объекта)
//     *
//     * Результатом сравнения двух конфигураций будет список действий, который необходимо выполнить, чтобы база пришла в целостное,
//     * обновлённое состояние, которое соответсвует конфигурации
//     *
//     * Пример:
//     * Исходный документ        Обновлённый документ
//     * ПриходнаяНакладная       ПриходнаяНакладная
//     *      - Номер                  - Номер
//     *      - Дата                   - Контрагент
//     *      - Контрагент             - Дата
//     *                               - Склад
//     *
//     *
//     * Из примера видно что добавился склад
//     * Поэтому необходимо сделать
//     *
//     * AddFieldAction(FiledName, FieldType, etc...)
//     *
//     * Либо при изменении должно зарегистрироваться следующее
//     *
//     * ChangeFieldAction(Value, Old, New)
//     *
//     * Для удаления будет следующая штука
//     *
//     * DeleteFieldAction(Value)
//     *
//     * И такие экшены будут реализовыны для каждого компонента
//     *
//     */
//}

