using System;
using System.Collections.Generic;
using Aquila.Compiler.Aqua;
using Aquila.Core.Contracts.Data;
using Aquila.Core.Contracts.Data.Entity;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.EntityComponent.Entity
{
    public class GeneratorRules : IEntityGenerator
    {
        public GeneratorRules(IComponent component)
        {
            Component = component;
        }

        protected IComponent Component { get; }

        /// <summary>
        /// Префикс объектов DTO, необходим для внутренних нужд класса
        /// </summary>
        public virtual string DtoPrefix { get; } = "Dto";

        public virtual string DtoPrivateFieldName { get; } = "_dto";

        public virtual string GetDtoClassName(IPType obj)
        {
            return $"{obj.Name}{DtoPrefix}";
        }

        public virtual string GetEntityClassName(IPType obj)
        {
            return "";

            // var preffix = obj.Parent.GetCodeRule(CodeGenRuleType.EntityClassPrefixRule).GetExpression();
            // var postfix = obj.Parent.GetCodeRule(CodeGenRuleType.EntityClassPostfixRule).GetExpression();
            //
            // return $"{preffix}{obj.Name}{postfix}";
        }

        //TODO: Необходимо реализовать все типы правил в базовом классе и выдавать Exception
        //в случае, если свойство не реализовано, а оно где-то вызвалось. 
        //Это явно укажет на то, что объект не может быть использован для такого сценария

        /// <summary>
        /// Получить правило именования постфикса для сущности
        /// </summary>
        /// <returns></returns>
        public virtual CodeGenRule GetEntityClassPostfixRule()
        {
            return new CodeGenRule(CodeGenRuleType.EntityClassPostfixRule, "");
        }


        /// <summary>
        /// Получить правило именования префикса для сущности
        /// </summary>
        /// <returns></returns>
        public virtual CodeGenRule GetEntityClassPrefixRule()
        {
            return new CodeGenRule(CodeGenRuleType.EntityClassPrefixRule, "");
        }

        public virtual CodeGenRule GetInForeignPropertySetActionRule()
        {
            return new CodeGenRule(CodeGenRuleType.InForeignPropertySetActionRule, null);
            //Obsolete
            throw new Exception(
                "This object type can't be a foreign property. You must exclude this prop type from object. Look at stack trace for object name.");
        }

        public virtual CodeGenRule GetInForeignPropertyGetActionRule()
        {
            return new CodeGenRule(CodeGenRuleType.InForeignPropertyGetActionRule, null);
            //Obsolete
            throw new Exception(
                "This object type can't be a foreign property. You must exclude this prop type from object. Look at stack trace for object name.");
        }

        public virtual CodeGenRule GetNamespaceRule()
        {
            return new CodeGenRule(CodeGenRuleType.NamespaceRule, "DefaultNamespace");
        }

        /// <summary>
        /// Получить от компонента готовую сгенерированную структуру с разбивкой по файлам
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public virtual Dictionary<string, string> GenerateSourceFiles()
        {
            throw new NotImplementedException();
        }
    }
}