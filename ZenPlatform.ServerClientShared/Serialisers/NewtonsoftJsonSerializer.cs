using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Compression;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Globalization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace ZenPlatform.Core.Serialisers
{
    public class NewtonsoftJsonSerializer : ISerializer
    {
        public readonly JsonSerializer Serializer;
        private JsonSerializerSettings _settings;

        public NewtonsoftJsonSerializer()
        {
            var convertors = new List<JsonConverter>
            {
                new SurrogateConverter(this)
            };

            _settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                Converters = convertors
            };
            Serializer = JsonSerializer.Create(_settings);
        }
        public T FromBytes<T>(byte[] input)
        {


            var data = Encoding.UTF8.GetString(input);
            return JsonConvert.DeserializeObject<T>(data, _settings);
        }

        public object FromBytes(byte[] input)
        {


            var data = Encoding.UTF8.GetString(input);
            return JsonConvert.DeserializeObject(data, _settings);
            /*
            using (MemoryStream memoryStream = new MemoryStream(input))
            {

                StreamReader streamReader = new StreamReader(memoryStream);
                return JsonConvert.DeserializeObject(Encoding.UTF8.GetBytes(data);streamReader.ReadToEnd(), settings);

            }
            */
        }

        public byte[] ToBytes<T>(T input)
        {
            
            /*
            using (var memoryStream = new MemoryStream())
            {

                using (StreamWriter streamWriter = new StreamWriter(memoryStream))
                {
                    streamWriter.Write();
                }
                */
                return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(input, _settings)); 
           // }

        }


        internal class SurrogateConverter : JsonConverter
        {
            private readonly NewtonsoftJsonSerializer _parent;
            /// <summary>
            /// TBD
            /// </summary>
            /// <param name="parent">TBD</param>
            public SurrogateConverter(NewtonsoftJsonSerializer parent)
            {
                _parent = parent;
            }
            /// <summary>
            ///     Determines whether this instance can convert the specified object type.
            /// </summary>
            /// <param name="objectType">Type of the object.</param>
            /// <returns><c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.</returns>
            public override bool CanConvert(Type objectType)
            {
                if (objectType == typeof(int) || objectType == typeof(float) || objectType == typeof(decimal))
                    return true;



                if (objectType == typeof(object))
                    return true;

                return false;
            }

            /// <summary>
            /// Reads the JSON representation of the object.
            /// </summary>
            /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
            /// <param name="objectType">Type of the object.</param>
            /// <param name="existingValue">The existing value of object being read.</param>
            /// <param name="serializer">The calling serializer.</param>
            /// <returns>The object value.</returns>
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                JsonSerializer serializer)
            {
                return DeserializeFromReader(reader, serializer, objectType);
            }

            private object DeserializeFromReader(JsonReader reader, JsonSerializer serializer, Type objectType)
            {
                var surrogate = serializer.Deserialize(reader);
                return TranslateSurrogate(surrogate, _parent, objectType);
            }

            private static object TranslateSurrogate(object deserializedValue, NewtonsoftJsonSerializer parent, Type type)
            {
                var j = deserializedValue as JObject;
                if (j != null)
                {
                    //The JObject represents a special akka.net wrapper for primitives (int,float,decimal) to preserve correct type when deserializing
                    if (j["$"] != null)
                    {
                        var value = j["$"].Value<string>();
                        return GetValue(value);
                    }

                    //The JObject is not of our concern, let Json.NET deserialize it.
                    return j.ToObject(type, parent.Serializer);
                }

                return deserializedValue;
            }

            private static object GetValue(string V)
            {
                var t = V.Substring(0, 1);
                var v = V.Substring(1);
                if (t == "I")
                    return int.Parse(v, NumberFormatInfo.InvariantInfo);
                if (t == "F")
                    return float.Parse(v, NumberFormatInfo.InvariantInfo);
                if (t == "M")
                    return decimal.Parse(v, NumberFormatInfo.InvariantInfo);

                throw new NotSupportedException();
            }

            /// <summary>
            /// Writes the JSON representation of the object.
            /// </summary>
            /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
            /// <param name="value">The value.</param>
            /// <param name="serializer">The calling serializer.</param>
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                if (value is int || value is decimal || value is float)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("$");
                    writer.WriteValue(GetString(value));
                    writer.WriteEndObject();
                }

            }

            private object GetString(object value)
            {
                if (value is int)
                    return "I" + ((int)value).ToString(NumberFormatInfo.InvariantInfo);
                if (value is float)
                    return "F" + ((float)value).ToString(NumberFormatInfo.InvariantInfo);
                if (value is decimal)
                    return "M" + ((decimal)value).ToString(NumberFormatInfo.InvariantInfo);
                throw new NotSupportedException();
            }
        }

    }
}
