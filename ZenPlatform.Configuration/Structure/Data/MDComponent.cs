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
using IComponent = ZenPlatform.Configuration.Contracts.TypeSystem.IComponent;
using MDType = ZenPlatform.Configuration.Structure.Data.Types.MDType;

namespace ZenPlatform.Configuration.Structure.Data
{
    public class ComponentModel : IMetaData<MDComponent>
    {
        public ITypeManager _tm;

        private IComponent _component;
        private MDComponent _metadata;

        public Guid ComponentId => _component.Info.ComponentId;

        public void OnLoad(ILoader loader, MDComponent settings)
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

            _component = c;
            loader.TypeManager.Register(c);
        }

        public IMDItem OnStore(IXCSaver saver)
        {
            var asm = _component.ComponentAssembly;

            var refModule = asm.Modules.FirstOrDefault() ?? throw new Exception("Module not found");

            ModuleDefMD module = ModuleDefMD.Load(refModule);

            using (var ms = new MemoryStream())
            {
                module.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);
                saver.SaveBytes(refModule.Name, ms.ToArray());

                _metadata.AssemblyReference = refModule.Name;
            }

            return _metadata;
        }
    }

    /*
       - Root 
       - ComAsmRef (DllPath: C:\test\a.dll, MDComponent:C:\test\Com\Entity.xml) 
       ---------- Component level
            IDataComponent.Loader(ILoader, MDPath);
                        
            IDataComponent.ListMD() : List<object> -- Список метаданных;
            IDataComponent.Create() : object
            IDataComponent.Delete(object);
     */

    public class MDEntityComponent : MDComponent
    {
        public List<string> EntityReferences { get; set; }
    }

    /*
    
     CatalogStructure
     \
        Root.xml
        
        Entity.xml
        Document.xml
        Reference.xml
        
        Entity
            etc files...
        Document
            etc files...
        Reference
            etc files...
        
        AccumulateRegister
            Main.xml
        
        packages
            Entity.dll
            Document.dll
            Reference.dll
            AccumulateRegister.dll
        
     <Project>
        <Name>Test</Name>
        <Id>SOME_GUID</Id>
        
        <ComRef Name="Entity" Version="1.0.0.0" Entry="./Entity.xml" />
        <ComRef Name="Document" Version="1.0.0.0" Entry="./Document.xml" />
        <ComRef Name="Reference" Version="1.0.0.0" Entry="./Reference.xml" />
        
        <ComRef Name="AccumulateRegister" Version="1.0.0.0" />
     </Project>
     
     
     
     */


    public class ComponentRef : IComponentRef
    {
        public string Name { get; set; }
        public string Entry { get; set; }
    }

    public class MDComponent : IMDItem
    {
        public string AssemblyReference { get; set; }
        //
        // public List<string> EntityReferences { get; set; }

        public MDComponent()
        {
            // EntityReferences = new List<string>();
        }
    }
}