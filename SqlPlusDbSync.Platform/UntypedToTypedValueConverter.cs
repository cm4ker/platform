using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SqlPlusDbSync.Platform
{
    public class UntypedToTypedValueConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException("This converter should only be applied directly via ItemConverterType, not added to JsonSerializer.Converters");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;
            var value = serializer.Deserialize(reader, objectType);
            if (value is TypeWrapper)
            {
                return ((TypeWrapper)value).ObjectValue;
            }
            return value;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (serializer.TypeNameHandling == TypeNameHandling.None)
            {
                Console.WriteLine("ObjectItemConverter used when serializer.TypeNameHandling == TypeNameHandling.None");
                serializer.Serialize(writer, value);
            }
            // Handle a couple of simple primitive cases where a type wrapper is not needed
            else if (value is string)
            {
                writer.WriteValue((string)value);
            }
            else if (value is bool)
            {
                writer.WriteValue((bool)value);
            }
            else
            {
                var contract = serializer.ContractResolver.ResolveContract(value.GetType());
                if (contract is JsonPrimitiveContract)
                {
                    var wrapper = TypeWrapper.CreateWrapper(value);
                    serializer.Serialize(writer, wrapper, typeof(object));
                }
                else
                {
                    serializer.Serialize(writer, value);
                }
            }
        }
    }
}