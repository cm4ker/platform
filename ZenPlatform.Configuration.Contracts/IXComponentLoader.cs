namespace ZenPlatform.Configuration.Contracts
{
    /// <summary>
    /// Загрузчик компонента
    /// Является мостиком между компонентом и платформой
    /// </summary>
    public interface IXComponentLoader
    {
        /// <summary>
        /// Инициализировать компомент
        /// При вызове этого метода будет инициализирована сущность <see>
        ///         <cref>DataComponentBase</cref>
        ///     </see>
        /// </summary>
        IDataComponent GetComponentImpl(IXCComponent component);

        /// <summary>
        /// Загрузить тип компонента
        /// </summary>
        /// <param name="com">Компонент</param>
        /// <param name="blob">Хранилище сериализованного объекта</param>
        /// <returns></returns>
        IXCObjectType LoadObject(IXCComponent com, IXCBlob blob);

        /// <summary>
        /// Сохранить обхект
        /// </summary>
        /// <param name="conf"></param>
        void SaveObject(IXCObjectType conf);

        /// <summary>
        /// Загрузить правила компонента, необходимо для RLS
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        IXCDataRule LoadRule(IXCDataRuleContent content);

        /// <summary>
        /// Сохранить правило 
        /// </summary>
        /// <param name="rule"></param>
        void SaveRule(IXCDataRule rule);
    }
}