namespace Aquila.Metadata
{
    /*
    1) Compiler don't know about the metadata
    2) This process entirely passed to the component for this compiler must know about INTERFACE
    3) Component parse MD and create Type (or Types for it)
    4) After this compiler go create Properties and Methods
    5) Metadata contains Symbolic link to the Type (ex: InvoiceLink)
        Here need some explanation:
            All components 'sign' protocol \ communication
            Postfix 'Link' - persisted into the database object
            Postfix 'Object' - not persisted into the database object

    */

    /*
     TODO: Is DotNetYAML wrap the parsed structure into the graph? -NO
          
     TODO: Create metadata language based on yaml 
     
     */

    /// <summary>
    /// Metadata base class
    /// </summary>
    public abstract class MetadataBase
    {
    }

    /*
     DB internal metadata completely full (current schema) for compare and migrate
     DB table (SchemaTable) for entry relational to the tables (Key-Value store)
     
     Now we have base compiler infrastructure for compilation language.
     The next steps for make platform working:
     
     1) Create simple metadata for Entity
     2) Create database infrastructure for migrating
     3) Add into language the synthesize information about Entity and provide info about classes
     3) Add into language the querying feature ( for it we need 1 & 2 )
     4) Create simple metadata for Web service
     5) Add into language new feature WebServices 
     
     0) CodeAnalyses run diagnostics on code if no errors then we can compile
     1) Deployer reads the local conf
     2) Deployer reads the remote conf
     3) Compare & decide run migration
     4) Questioning about migration (warnings, remove objects data etc)
     5) Migrate
     6) Run the code Analyses and compile the code (can optimize it if we can connection to the endpoint)
     
          
     Compiler load SchemaTable -> it give a shot for compile queries (targeted solution). 
     
     Queries can be 2 types:
     1) Compiled
     2) Interpreted
     
     Reflection in platform 
      
     
     
     
     */
}