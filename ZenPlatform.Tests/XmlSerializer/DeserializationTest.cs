using SharpDX.Win32;
using Xunit;
using ZenPlatform.Tests.XmlSerializer.TestedObjects;
using ZenPlatform.XmlSerializer;

namespace ZenPlatform.Tests.XmlSerializer
{
    public class DeserializationTest
    {
        private Serializer _serializer = new Serializer();


        [Fact]
        public void SimpleDeserialize()
        {
            var xml =
                @"<?xml version=""1.0"" encoding=""utf-16""?><SimpleObjectWithNastedObject><ThisIsStringField>Field string</ThisIsStringField><IntValue>275</IntValue><ThisIsString>This is simple string;</ThisIsString></SimpleObjectWithNastedObject>";

            var o = _serializer.Deserialize<SimpleObjectWithNastedObject>(xml);
            Assert.NotNull(o);
            Assert.Equal("Field string", o.ThisIsStringField);
            Assert.Equal(275, o.IntValue);
            Assert.Equal("This is simple string;", o.ThisIsString);
        }

        [Fact]
        public void NastedTest()
        {
            var xml =
                @"<?xml version=""1.0"" encoding=""utf-16""?><DeserializeNestedObject><Nasted><SimpleObject /></Nasted></DeserializeNestedObject>";

            var o = _serializer.Deserialize<DeserializeNestedObject>(xml);
            Assert.NotNull(o);
            Assert.NotNull(o.Nasted);
        }

        [Fact]
        public void CollectionTest()
        {
            var xml =
                @"<?xml version=""1.0"" encoding=""utf-16""?><DeserializeCollection><Collection><String>First</String><String>Second</String><String>Third</String></Collection></DeserializeCollection>";

            var o = _serializer.Deserialize<DeserializeCollection>(xml);
            Assert.NotNull(o);
            Assert.NotNull(o.Collection);
            Assert.Equal(3, o.Collection.Count);
        }
    }
}