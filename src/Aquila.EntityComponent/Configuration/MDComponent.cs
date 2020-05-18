using System.Collections.Generic;

namespace Aquila.EntityComponent.Configuration
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

    public class MDComponent
    {
    }
}