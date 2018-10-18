using System.IO;
using Xunit;
using ZenPlatform.Tests.XmlSerializer.TestedObjects;
using ZenPlatform.XmlSerializer;

namespace ZenPlatform.Tests.XmlSerializer
{
    public class SerializationTest
    {
        private Serializer _serializer = new Serializer();

        [Fact]
        public void SimpleObject()
        {
            var conf = SerializerConfiguration.Create()
                .Ignore<SimpleObjectWithNastedObject>(x => x.Nasted);

            var expected =
                @"<?xml version=""1.0"" encoding=""utf-16""?><SimpleObjectWithNastedObject><ThisIsStringField>Field string</ThisIsStringField><IntValue>275</IntValue><ThisIsString>This is simple string;</ThisIsString></SimpleObjectWithNastedObject>";

            var instance = new SimpleObjectWithNastedObject();
            using (var tw = new StringWriter())
            {
                _serializer.Serialize(instance, tw, conf);
                Assert.Equal(expected, tw.ToString());
            }
        }

        [Fact]
        public void CollectionTesting()
        {
            var conf = SerializerConfiguration.Create();

            var expected =
                @"<?xml version=""1.0"" encoding=""utf-16""?><ObjectWithCollection><Collection><String>First</String><String>Second</String><String>Third</String></Collection></ObjectWithCollection>";

            var instance = new ObjectWithCollection();
            using (var tw = new StringWriter())
            {
                {
                    _serializer.Serialize(instance, tw, conf);
                    Assert.Equal(expected, tw.ToString());
                }
            }
        }

        [Fact]
        public void DictTesting()
        {
            var conf = SerializerConfiguration.Create();

            var expected =
                @"<?xml version=""1.0"" encoding=""utf-16""?><ObjectWithDictionary><Dict><KeyValuePair><Key>First</Key><Value>1</Value></KeyValuePair><KeyValuePair><Key>Second</Key><Value>2</Value></KeyValuePair><KeyValuePair><Key>Third</Key><Value>3</Value></KeyValuePair></Dict></ObjectWithDictionary>";

            var instance = new ObjectWithDictionary();
            using (var tw = new StringWriter())
            {
                {
                    _serializer.Serialize(instance, tw, conf);
                    Assert.Equal(expected, tw.ToString());
                }
            }
        }
    }
}