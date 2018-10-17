namespace ZenPlatform.Tests.XmlSerializer.TestedObjects
{
    public class SimpleObject
    {
        
    }

    public class SimpleObjectWithFields
    {
        public string Field1 { get; set; }
    }

    public class SimpleObjectWithNastedObject
    {
        public SimpleObject Nasted { get; set; }
    }

    public class WithConstructors
    {
        public WithConstructors()
        {
            
        }

        public WithConstructors(string field1)
        {
            
        }
    }
    
    public class WithoutDefaultConstructor
    {
        public WithoutDefaultConstructor(string field1)
        {
            
        }
    }
}