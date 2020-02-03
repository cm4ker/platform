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

    public class MDComponent : IMDComponent
    {
        public List<string> EntityReferences { get; set; }
    }
}