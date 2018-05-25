//using System;
//using System.Collections.Generic;
//using System.Reflection;
//using ZenPlatform.Configuration.ConfigurationLoader.Contracts;
////using ZenPlatform.Configuration.Data;
//
//namespace ZenPlatform.Configuration
//{
//    /// <summary>
//    /// Корень конфигурации
//    /// </summary>
//    public class PRootConfiguration
//    {
//        public PRootConfiguration() : this(Guid.NewGuid())
//        {
//        }
//
//        public PRootConfiguration(Guid id)
//        {
//            Id = id;
//            Data = new PDataSectionConfiguration();
//            Languages = new List<PLanguage>();
//            Roles = new List<PRole>();
//        }
//
//        public string ProjectName { get; set; }
//
//        public PDataSectionConfiguration Data { get; }
//
//        public List<PLanguage> Languages { get; }
//
//        public List<PRole> Roles { get; set; }
//
//        public Guid Id { get; }
//    }
//
//    public class PRole
//    {
//        public PRole()
//        {
//            PlatformRules = new PPlatformRules();
//            ObjectRules = new List<IRule>();
//        }
//
//        public string RoleName { get; set; }
//
//        public PPlatformRules PlatformRules { get; }
//        public List<IRule> ObjectRules { get; set; }
//
//
//    }
//
//    public class PPlatformRules
//    {
//        //TODO: Перенести все свойства 
//    }
//
//    public abstract class PObjectRule : IRule
//    {
//        public PObjectRule(Guid objectId, PComponent component)
//        {
//            ObjectId = objectId;
//            ComponentOwner = component;
//        }
//
//        public Guid ObjectId { get; }
//        public PComponent ComponentOwner { get; }
//    }
//
//    /// <summary>
//    /// Язык, поддерживает регистрацию ресурсов, хранит перевод
//    /// </summary>
//    public class PLanguage
//    {
//        public PLanguage(string alias, string fullName)
//        {
//            Alias = alias;
//            FullName = fullName;
//
//            Resources = new Dictionary<string, string>();
//        }
//
//        public Dictionary<string, string> Resources { get; }
//
//        public string Alias { get; }
//        public string FullName { get; }
//    }
//
//    /// <summary>
//    /// Секция данных. Содержит в себе описания всех подключенных компонентов и зарегистрированных типов
//    /// </summary>
//    public class PDataSectionConfiguration
//    {
//        public PDataSectionConfiguration()
//        {
//            Components = new List<PComponent>();
//            Types = new List<IComponentType>();
//        }
//
//        public List<PComponent> Components { get; set; }
//        public List<IComponentType> Types { get; set; }
//
//    }
//}
