using System.Collections.Generic;

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

        public string ThisIsStringField = "Field string";
    }

    public class ObjectWithCollection
    {
        public ObjectWithCollection()
        {
            Collection = new List<string>();
            Collection.Add("First");
            Collection.Add("Second");
            Collection.Add("Third");
        }

        public List<string> Collection { get; set; }
    }

    public class ObjectWithDictionary
    {
        public ObjectWithDictionary()
        {
            Dict = new Dictionary<string, int>();
            Dict.Add("First", 1);
            Dict.Add("Second", 2);
            Dict.Add("Third", 3);
        }

        public Dictionary<string, int> Dict { get; set; }
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