using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Aquila.Metadata
{
    public class SecPolicySubjectMetadata
    {
        public string Name { get; set; }
        public SecPermission Permission { get; set; }
    }

    public class SecPolicyMetadata
    {
        private static readonly IDeserializer Deserializer = new DeserializerBuilder()
            .WithTypeConverter(new YamlStringEnumConverter())
            .WithNamingConvention(NullNamingConvention.Instance)
            .Build();

        private static readonly ISerializer Serializer = new SerializerBuilder()
            .WithTypeConverter(new YamlStringEnumConverter())
            .WithNamingConvention(NullNamingConvention.Instance)
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitDefaults)
            .Build();

        public static SecPolicyMetadata FromYaml(string yaml)
        {
            return Deserializer.Deserialize<SecPolicyMetadata>(yaml);
        }

        public static string ToYaml(SecPolicyMetadata md)
        {
            return Serializer.Serialize(md);
        }

        public string Name { get; set; }

        public SecPolicyType Type { get; set; }

        public List<SecPolicySubjectMetadata> Subjects { get; set; }

        public List<SecPolicyCriterionMetadata> Criteria { get; set; }
    }

    /// <summary>
    /// This criteria need for select certain objects and extract permissions only on this objects
    /// </summary>
    public class SecPolicyCriterionMetadata
    {
        public SecPermission Permission { get; set; }

        public string Subject { get; set; }
        

        public string Query { get; set; }
    }

    [Flags]
    public enum SecPermission
    {
        None = 0,

        Create = 1 << 0,

        Read = 1 << 1,

        Update = 1 << 2,

        Delete = 1 << 3,
    }

    public enum SecPolicyType
    {
        Grant,
        Revoke
    }

    public class YamlStringEnumConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type) => type.IsEnum;

        public object ReadYaml(IParser parser, Type type)
        {
            var enumMembers = type.GetEnumNames().ToDictionary(x => x.ToLower(CultureInfo.InvariantCulture), x => x);
            var result = new StringBuilder();

            void HandleScalar(Scalar s)
            {
                var value = s.Value.ToLower(CultureInfo.InvariantCulture);

                if (enumMembers.TryGetValue(value, out var enumMember))
                {
                    result.Append(enumMember).Append(",");
                }
            }

            if (parser.TryConsume(out SequenceStart ss))
            {
                //we have flags value
                while (parser.TryConsume(out Scalar s))
                {
                    HandleScalar(s);
                }

                //required!
                parser.Consume<SequenceEnd>();
            }
            else
            {
                //scalar value
                HandleScalar(parser.Consume<Scalar>());
            }


            return Enum.Parse(type, result.ToString().TrimEnd(','));
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            var serialized = value.ToString() ?? string.Empty;
            var items = serialized.Split(',').Select(x => x.Trim()).ToArray();


            if (items.Length > 1)
                emitter.Emit(new SequenceStart(AnchorName.Empty, TagName.Empty, true, SequenceStyle.Flow));

            foreach (var item in items)
            {
                emitter.Emit(new Scalar(item.Trim()));
            }

            if (items.Length > 1)
                emitter.Emit(new SequenceEnd());
        }
    }
}