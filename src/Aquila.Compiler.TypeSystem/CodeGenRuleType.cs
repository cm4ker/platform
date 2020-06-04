using System;

namespace Aquila.Compiler.Aqua
{
    public enum CodeGenRuleType
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
        InForeignPropertyGetActionRule,


        /// <summary>
        /// Правила для генерации namespace
        /// </summary>
        NamespaceRule,

        /// <summary>
        /// Префикс для объектов DTO
        /// </summary>
        DtoPreffixRule,

        /// <summary>
        /// Постфикс для объектов DTO
        /// </summary>
        DtoPostfixRule
    }

    public class CodeGenRule
    {
        private readonly string _expression;

        public CodeGenRule(CodeGenRuleType type, string expression)
        {
            Type = type;
            _expression = expression;
        }

        public CodeGenRuleType Type { get; }

        public string GetExpression()
        {
            if (_expression is null) throw new NotImplementedException();
            return _expression;
        }
    }
}