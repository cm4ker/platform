using System;
using Aquila.Core.Contracts.Configuration;
using Aquila.Core.Contracts.Data;
using Aquila.Shared.ParenChildCollection;

namespace Aquila.Core.Contracts.TypeSystem
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

        object ComponentImpl { get; set; }

        /// <summary>
        /// Throws exception if component not support feature
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetFeature<T>() where T : class
        {
            return ComponentImpl as T ?? throw new Exception("Component not support this feature");
        }

        public bool TryGetFeature<T>(out T feature) where T : class
        {
            feature = ComponentImpl as T;
            return feature != null;
        }


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