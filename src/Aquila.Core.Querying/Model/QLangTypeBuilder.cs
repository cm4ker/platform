using System;
using System.Text.RegularExpressions;
using Aquila.Configuration.Contracts;
using Aquila.Configuration.Contracts.TypeSystem;
using Aquila.Configuration.Structure;
using Aquila.Configuration.Structure.Data.Types.Primitive;


namespace Aquila.Core.Querying.Model
{
    public class QLangTypeBuilder
    {
        private readonly ITypeManager _conf;

        public QLangTypeBuilder(ITypeManager conf)
        {
            _conf = conf;
        }

        public IPType String(int size)
        {
            var spec = _conf.String.GetSpec();
            spec.Size = size;
            return spec;
        }

        public IPType Numeric(int scale, int precision)
        {
            var spec = _conf.Numeric.GetSpec();
            spec.Scale = scale;
            spec.Precision = precision;
            return spec;
        }

        public IPType Date()
        {
            return _conf.DateTime;
        }

        public IPType Parse(string typeName)
        {
            if (typeName.Contains("."))
            {
                var parts = typeName.Split(".");

                return _conf.FindComponentByName(parts[0]).FindTypeByName(parts[1]);
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