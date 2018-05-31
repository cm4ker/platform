using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration;

namespace ZenPlatform.Core.Entity
{
    public abstract class EntityGeneratorBase
    {
        protected EntityGeneratorBase(XCComponent component)
        {
            Component = component;
        }


        protected XCComponent Component { get; }

        /// <summary>
        /// Префикс объектов DTO, необходим для внутренних нужд класса
        /// </summary>
        public virtual string DtoPrefix { get; } = "Dto";

        public virtual string DtoPrivateFieldName { get; } = "_dto";


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
        public virtual Dictionary<string, string> GenerateFilesFromComponent()
        {
            throw new NotImplementedException();
        }
    }

    /*
     * Нужно организовать такой интерфейс для другого билдера, чтобы он мог сделать следующее
     * В свойстве, когда PObjectType = другой компанент, сгенерировать правильную инстуркцию по получению и по сохранению
     * Например.
     * 
     * Есть документ Invoice
     * У него есть свойство NomenclatureTable у этого свойства организована следующая настройка IsOneToMany = True
     * Для Dto объекта Invoice никаких свойств реализовано не будет.
     * Для Dto NomenclatureTableRow будет добавлено следующее поле: 
     * 
     * public [InvoiceKeyType] InvoiceRef { get; set; }
     * 
     * Причём, если у нас будут ссылаться несолько объектов на 1у таблицу, необходимо сгенерировать следующую структуру для Dto:
     * 
     * public [КонсолидирующийТипДанныхДляКлюча] Ref { get; set; }
     * public int RefType { get; set; } // Тип объекта, которому принадлежит данная строка (см **1)
     * 
     * А вот для объекта Entity будет реалзовано примерно следующее:
     * 
     * public EntityCollection<NomenclatureTableRow> NomenclatureTable 
     * {
     *     get
     *     {
     *          return Session.Data.NomenclatureTable.GetListByDocumentId(_dto.Key);
     *          //return Session.Data.NomenclatureTable.GetListByDocumentId(_dto.Key, Environment.GetTypeNumber(GetType())); -- В случае, если много документов ссылаются на 1 табличную часть
     *          
     *     }
     * }
     * 
     * Из примера выше следующие тезисы:
     *      1) Коллекции объектов могут быть только для чтения.
     *      2) Коллекции должны быть типа EntityCollection (под вопросом, но обёртка должна быть)
     *      3) Должен загенериться какой-то метод аля GetListByDocumentId(object key)
     *      4) Не понятна ситуация, когда одна табличная часть может быть связана с несколькими объектами, что в этом случае должно происходить? (Под **1 - попытка представить
     *      как это будет выглядеть в БД)
     *      
     *      
     */

    public class PublicMethodDefinition
    {
    }
}