using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Composition.Hosting.Core;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Host.Mef;
using ZenPlatform.Configuration.Data;
using ZenPlatform.Core.Entity;

namespace ZenPlatform.CSharpCodeBuilder
{
    public class CodeBuilder
    {
        private const string folderName = "generated";

        public void Generate(EntityGeneratorBase generator, PComponent component)
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

            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);
            var filesForDelete = Directory.GetFiles(folderName, "*.cs");
            foreach (var file in filesForDelete)
            {
                File.Delete(file);
            }

            var files = generator.GenerateFilesFromComponent(component);

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

        }
    }
}
