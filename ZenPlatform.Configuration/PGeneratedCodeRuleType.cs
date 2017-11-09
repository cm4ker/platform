using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Configuration
{
    public enum PGeneratedCodeRuleType
    {
        /// <summary>
        /// Префикс класса
        /// </summary>
        EntityClassPrefixRule,

        /// <summary>
        /// Постфикс класса
        /// </summary>
        EntityClassPostfixRule,

        /// <summary>
        /// Правила генерации класса для чужих свойств
        /// </summary>
        InForeignPropertyTypeRule,

        /// <summary>
        /// Правила генерации колелкций для чужих свойств
        /// </summary>
        InForeignPropertyCollectionTypeRule,

        /// <summary>
        /// Правила генерации перед получением данных в чужих свойствах
        /// </summary>
        InForeignPropertyAfterSetActionsRule,

        /// <summary>
        /// Правила генерации после установки данных в чужих свойствах
        /// </summary>
        InForeignPropertyBeforeGetActionsRule,

        /// <summary>
        /// Правила генерации поля установки
        /// </summary>
        InForeignPropertySetActionRule,

        /// <summary>
        /// Правила генерации поля получения
        /// </summary>
        InForeignPropertyGetActionRule
    }

    public class PGeneratedCodeRule
    {
        private readonly string _expression;

        public PGeneratedCodeRule(PGeneratedCodeRuleType type, string expression)
        {
            Type = type;
            _expression = expression;
        }

        public PGeneratedCodeRuleType Type { get; }

        public string GetExpression()
        {
            if (_expression is null) throw new Exception("This GenerationCodeType not implemented");
            return _expression;
        }
    }
}
