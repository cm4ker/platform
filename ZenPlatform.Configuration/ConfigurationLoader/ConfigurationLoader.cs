using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration;
using ZenPlatform.Configuration.Data;

namespace ZenPlatform.Configuration.ConfigurationLoader
{
    public class ConfigurationLoader
    {
        private DirectoryInfo _directory;
        private FileInfo _pathToProject;


        public ConfigurationLoader(string pathToProjectToProjectXmlFile)
        {
            _pathToProject = new FileInfo(pathToProjectToProjectXmlFile);
            _directory = _pathToProject.Directory;
        }

        public PRootConfiguration Load()
        {
            //Шаг 1: загружаем корневой файл проекта
            using (var tr = new StreamReader(_pathToProject.FullName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(XmlConfRoot));
                var xmlConf = (XmlConfRoot)serializer.Deserialize(tr);

                var conf = new PRootConfiguration(xmlConf.ProjectId);
                conf.ProjectName = xmlConf.ProjectName;

                //Шаг : загружаем языки
                foreach (var language in xmlConf.Languages)
                {
                    conf.Languages.Add(new PLanguage(language.Alias, language.Name));
                }

                //Шаг : загружаем присоединённые компоненты данных
                LoadComponents(xmlConf, conf);

                //Шаг : загружаем типы данных
                foreach (var file in xmlConf.Data.IncludedFiles)
                {
                    var component = conf.Data.Components.First(x => x.Id == file.ComponentId);
                    var pObj = component.Loader.LoadComponentType(Path.Combine(_directory.Name, file.Path), component);
                    conf.Data.Types.Add(pObj);
                }

                LoadRoles(xmlConf, conf);

                //Шаг : Загружаем все свойства типов данных
                foreach (var file in xmlConf.Data.IncludedFiles)
                {
                    var component = conf.Data.Components.First(x => x.Id == file.ComponentId);
                    component.Loader.LoadComponentTypeDependencies(Path.Combine(_directory.Name, file.Path), conf.Data.Types);
                }



                return conf;
            }
        }

        private void LoadComponents(XmlConfRoot xmlConf, PRootConfiguration conf)
        {
            foreach (var xmlConfComponent in xmlConf.Data.Components)
            {
                var componentPath = Path.Combine(_directory.ToString(), xmlConfComponent.File.Path);
                FileInfo fi = new FileInfo(componentPath);

                var comAssembly = Assembly.LoadFile(fi.FullName);

                var loader = comAssembly.GetTypes()
                   .FirstOrDefault(x => x.IsPublic && !x.IsAbstract && x.GetInterfaces().Contains(typeof(IComponenConfigurationtLoader)));

                if (loader != null)
                {
                    //Если компонент реализует компонент для доступа к данным и загрузчик конфигурации, в таком случае мы можем выполнить загрузку
                    var loaderInstance = (IComponenConfigurationtLoader)Activator.CreateInstance(loader);

                    var pComponent = new PComponent(xmlConfComponent.Id, loaderInstance);
                    pComponent.Name = xmlConfComponent.Name;
                    pComponent.ComponentPath = xmlConfComponent.File.Path;

                    //Необходимо зарегистрировать конфигурацию
                    pComponent.Configuration = conf;

                    conf.Data.Components.Add(pComponent);
                }
                else
                {
                    throw new InvalidComponentException();
                }
            }
        }

        private void LoadRoles(XmlConfRoot xmlConf, PRootConfiguration conf)
        {
            //Шаг : Загружаем роли
            foreach (var file in xmlConf.Roles.IncludedFiles)
            {
                using (var roleFile = new StreamReader(Path.Combine(_directory.Name, file.Path)))
                {
                    var ser = new XmlSerializer(typeof(XmlConfRole));
                    var roleInstance = (XmlConfRole)ser.Deserialize(roleFile);

                    var prole = new PRole();
                    prole.RoleName = roleInstance.Name;

                    //TODO: заполнить правила платформы (PlatformRules) 

                    foreach (var orule in roleInstance.DataRules)
                    {
                        var pobject = conf.Data.Types.First(x => x.Id == orule.ObjectId);

                        prole.ObjectRules.Add(pobject.OwnerComponent.Loader.LoadComponentRole(orule.Content));
                    }

                    conf.Roles.Add(prole);
                }
            }
        }
    }
}

public class InvalidComponentException : Exception
{
}

