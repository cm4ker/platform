using System;
using System.Text.RegularExpressions;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;


namespace ZenPlatform.Core.Querying.Model
{
    public class QLangTypeBuilder
    {
        private readonly IXCRoot _conf;

        public QLangTypeBuilder(IXCRoot conf)
        {
            _conf = conf;
        }

        public IXCType String(int size)
        {
            return new XCString {Size = size};
        }

        public IXCType Numeric(int scale, int precision)
        {
            return new XCNumeric {Scale = scale, Precision = precision};
        }

        public IXCType Date()
        {
            return new XCDateTime();
        }

        public IXCType Parse(string typeName)
        {
            if (typeName.Contains("."))
            {
                var parts = typeName.Split(".");

                return _conf.Data.GetComponentByName(parts[0]).GetTypeByName(parts[1]);
            }

            Regex r = new Regex(@"(\b[^()]+)(\((.*)\))?$");

            var match = r.Match(typeName);
            string[] args = null;

            if (match.Success)
            {
                if (match.Groups.Count == 1)
                {
                    typeName = match.Groups[1].Value;
                }
                else
                {
                    typeName = match.Groups[1].Value;
                    args = match.Groups[3].Value.Split(",");
                }
            }

            return typeName switch
            {
                "string" => String(int.Parse(args[0])),
                "numeric" => Numeric(int.Parse(args[0]), int.Parse(args[1])),
                "datetime" => Date(),
                _ => throw new Exception($"Type def: {typeName} can't be resolved")
            };
        }
    }
}