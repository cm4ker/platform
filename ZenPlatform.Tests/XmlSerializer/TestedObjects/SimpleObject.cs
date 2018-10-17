namespace ZenPlatform.Tests.XmlSerializer.TestedObjects
{
    public class SimpleObject
    {
    }

    public class SimpleObjectWithFields
    {
        public string Field1 { get; set; } = "value";
    }

    public class SimpleObjectWithNastedObject
    {
        public SimpleObjectWithNastedObject()
        {
            Nasted = new SimpleObject();
        }

        public SimpleObject Nasted { get; set; }

        public int IntValue { get; set; } = 275;
        public string ThisIsString { get; set; } = "This is simple string;";
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