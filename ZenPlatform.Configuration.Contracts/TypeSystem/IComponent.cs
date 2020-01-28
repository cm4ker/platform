using System;
using System.Reflection;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Contracts.TypeSystem
{
    public interface ITypeManagerProvider
    {
        ITypeManager TypeManager { get; }
    }

    public interface IComponent : ITypeManagerProvider, IChildItem<IProject>
    {
        public Guid Id { get; }

        /// <summary>
        /// Информация о компоненте
        /// </summary>
        IXCComponentInformation Info { get; set; }
        
        string Name { get; }
        
        IDataComponent ComponentImpl { get; set; }

        IMDComponent Metadata { get; set; }

        /// <summary>
        /// Зарегистрировать правило для генерации кода
        /// Это действие иммутабельно. В последствии нельзя отменить регистрацию.
        /// Нельзя создавать два одинаковых правила генерации кода с одним типом. Это приведёт к ошибке
        /// </summary>
        /// <param name="rule"></param>
        void RegisterCodeRule(CodeGenRule rule);

        /// <summary>
        ///  Получить правило генерации кода по его типу.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        CodeGenRule GetCodeRule(CodeGenRuleType type);

        string GetCodeRuleExpression(CodeGenRuleType type);
    }
}