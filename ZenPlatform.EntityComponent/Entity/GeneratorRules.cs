using System;
using System.Collections.Generic;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Configuration.Contracts.Data.Entity;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;

namespace ZenPlatform.EntityComponent.Entity
{
    public class GeneratorRules : IEntityGenerator
    {
        public GeneratorRules(IXCComponent component)
        {
            Component = component;
        }

        protected IXCComponent Component { get; }

        /// <summary>
        /// Префикс объектов DTO, необходим для внутренних нужд класса
        /// </summary>
        public virtual string DtoPrefix { get; } = "Dto";

        public virtual string DtoPrivateFieldName { get; } = "_dto";

        public virtual string GetDtoClassName(IXCObjectType obj)
        {
            return $"{obj.Name}{DtoPrefix}";
        }

        public virtual string GetEntityClassName(IXCObjectType obj)
        {
            var preffix = obj.Parent.GetCodeRule(CodeGenRuleType.EntityClassPrefixRule).GetExpression();
            var postfix = obj.Parent.GetCodeRule(CodeGenRuleType.EntityClassPostfixRule).GetExpression();

            return $"{preffix}{obj.Name}{postfix}";
        }

        public virtual string GetMultiDataStorageClassName(IXCObjectProperty property)
        {
            return $"MultiDataStorage_{property.DatabaseColumnName}";
        }

        public virtual string GetMultiDataStoragePrivateFieldName(IXCObjectProperty property)
        {
            return $"_mds{property.DatabaseColumnName}";
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

        public string GetDtoClassName(object obj)
        {
            return GetDtoClassName(obj as XCObjectTypeBase);
        }

        public string GetEntityClassName(object obj)
        {
            return GetEntityClassName(obj as XCObjectTypeBase);
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