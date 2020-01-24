using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading;
using System.Xml.Serialization;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Exceptions;
using ZenPlatform.Configuration.Structure.Data.Types;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Shared.ParenChildCollection;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Configuration.Contracts.Store;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.Configuration.Structure.Data
{
    public class MDComponent : IMDComponent, IMDItem, IMetaData<MDComponent>
    {
        public Guid ComponentId { get; set; }

        public string AssemblyReference { get; set; }

        public List<string> EntityReferences { get; set; }

        public MDComponent()
        {
            EntityReferences = new List<string>();
        }

        public void Initialize(ILoader loader, IMDComponent settings)
        {
            //load assembly
            var bytes = loader.LoadBytes(settings.AssemblyReference);

            var module = ModuleDefMD.Load(bytes);

            var alreadyLoaded = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(x => x.FullName == module.Assembly.FullName);

            var c = loader.TypeManager.Component();

            if (alreadyLoaded != null)
                c.ComponentAssembly = alreadyLoaded;
            else
                c.ComponentAssembly = Assembly.Load(bytes);

            // load entitys
            foreach (var reference in settings.EntityReferences)
            {
                c.ComponentImpl.Loader.LoadObject(c, loader, reference);
            }

            ComponentId = c.Info.ComponentId;
        }

        public void Initialize(ILoader loader, MDComponent settings)
        {
            throw new NotImplementedException();
        }

        public IMDItem Store(IXCSaver saver)
        {
            var asm = saver.TypeManager.FindComponent(ComponentId).ComponentAssembly;

            var refelectionModule = asm.Modules.FirstOrDefault();
            ModuleDefMD module = ModuleDefMD.Load(refelectionModule);

            using (var ms = new MemoryStream())
            {
                module.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);
                saver.SaveBytes(refelectionModule.Name, ms.ToArray());
                AssemblyReference = refelectionModule.Name;
            }

            return this;
        }
    }
}