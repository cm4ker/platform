using System;
using System.IO;
using System.Xml.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration;

namespace ZenPlatform.Builder
{
    public class CodeBuilder
    {
        private const string folderName = "bin";

        public void Generate(XCComponent component)
        {
            //TODO : Необходимо сделать единую точку входа для генерации файлов
            /*
             * Для этого необходимо: 
             *      1) Создать проект
             *      2) Добавить в созданный проект все сгенерированные файлы
             *      3) Скомпилировать проект, назвать его "CompiledConfiguration"
             *      4) Создать проект UserProject, поместить туда всю логику.
             *      
             * !!!Для версии 0.0.0.1 просто генерируем файлы и ложим их в определённую папку, скажем 'Generated'
             */

            var generator = component.ComponentImpl.Generator;


            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);
            var filesForDelete = Directory.GetFiles(folderName, "*.cs");
            foreach (var file in filesForDelete)
            {
                File.Delete(file);
            }

            var files = generator.GenerateFilesFromComponent();

            foreach (var file in files)
            {
                using (var fs = File.Create(Path.Combine(folderName, file.Key)))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(file.Value);
                    }
                }
            }

            var task = new AddFilesToProject()
            {
                ProjectPath =
                    @"C:\Users\n.zenkov\Source\Repos\ThePlatform\SqlPlusDbSync\SqlPlusDbSync\ZenPlatform.ReadyForDeployProject\ZenPlatform.ReadyForDeployProject.csproj",
                ProjectDir =
                    @"C:\Users\n.zenkov\Source\Repos\ThePlatform\SqlPlusDbSync\SqlPlusDbSync\ZenPlatform.ReadyForDeployProject\",
                FilePath =
                    @"C:\Users\n.zenkov\Source\Repos\ThePlatform\SqlPlusDbSync\SqlPlusDbSync\ZenPlatform.ReadyForDeployProject\Test",
                BinPath =
                    @"C:\Users\n.zenkov\Source\Repos\ThePlatform\SqlPlusDbSync\SqlPlusDbSync\ZenPlatform.ReadyForDeployProject\Bin",
                HooksPath =
                    @"C:\Users\n.zenkov\Source\Repos\ThePlatform\SqlPlusDbSync\SqlPlusDbSync\ZenPlatform.ReadyForDeployProject\Test"
            };

            task.Execute();
        }
    }

    public class AddFilesToProject : Task
    {
        [Required] public string ProjectPath { get; set; }

        [Required] public string FilePath { get; set; }

        [Required] public string BinPath { get; set; }

        [Required] public string HooksPath { get; set; }

        [Required] public string ProjectDir { get; set; }


        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>
        /// true if the task successfully executed; otherwise, false.
        /// </returns>
        public override bool Execute()
        {
            try
            {
                var binRelative = BinPath.Replace(ProjectDir + "\\", "");
                var hooksRelative = HooksPath.Replace(ProjectDir + "\\", "");
                var fileRelative = FilePath.Replace(ProjectDir + "\\", "");
                XDocument document = XDocument.Load(ProjectPath);
                XNamespace ns = "http://schemas.microsoft.com/developer/msbuild/2003";

                bool foundBin = false;
                bool foundHooks = false;
                bool foundFile = false;
                XElement itemGroup = null;
                foreach (XElement el in document.Descendants(ns + "ItemGroup"))
                {
                    foreach (XElement item in el.Descendants(ns + "Compile"))
                    {
                        itemGroup = el;
                        if (item.Attribute("Include").Value.Contains(binRelative))
                        {
                            foundBin = true;
                            Log.LogMessage(MessageImportance.Low, "FoundBin: {0}", foundBin);
                        }
                        else if (item.Attribute("Include").Value.Contains(hooksRelative))
                        {
                            foundHooks = true;
                            Log.LogMessage(MessageImportance.Low, "FoundHooks: {0}", foundHooks);
                        }
                        else if (item.Attribute("Include").Value.Contains(fileRelative))
                        {
                            foundFile = true;
                            Log.LogMessage(MessageImportance.Low, "FoundFile: {0}", foundFile);
                        }
                    }
                }

                if (!foundBin)
                {
                    XElement item = new XElement(ns + "Compile");
                    item.SetAttributeValue("Include", binRelative);
                    if (itemGroup != null) itemGroup.Add(item);
                }

                if (!foundHooks)
                {
                    XElement item = new XElement(ns + "Compile");
                    item.SetAttributeValue("Include", hooksRelative);
                    if (itemGroup != null) itemGroup.Add(item);
                }

                if (!foundFile)
                {
                    XElement item = new XElement(ns + "Compile");
                    item.SetAttributeValue("Include", fileRelative);
                    if (itemGroup != null) itemGroup.Add(item);
                }

                if (!foundBin || !foundHooks || !foundFile)
                {
                    document.Save(ProjectPath);
                }
            }
            catch (Exception e)
            {
                Log.LogErrorFromException(e);
            }

            return !Log.HasLoggedErrors;
        }
    }
}