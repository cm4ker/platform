using System;

namespace ConfCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }


    /*
     CreateDataType -> Create MD -> Create derivative MD
     
     
     _type
     {
        Id,            
        ParentId,
        
        Name,
        
        IsRoot,
        IsDerivative,
        IsPrimitive,
        
        IsAbstract
        IsLink
     }
     
     _prop
     {
        Id,
        Name,
        OwnerTypeId     
     }
     
     _prop_types
     {
        PropId,
        TypeId,   
        
        Scale,
        Precision,
        Size                  
     }
          
     */


    public class MDType
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }


        public string Name { get; set; }
    }

    public class MDProp
    {
    }

    public class Configuration
    {
    }
}