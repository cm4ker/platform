using System;
using System.Text.RegularExpressions;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.TypeSystem;
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

        public IType String(int size)
        {
            var spec = _conf.TypeManager.String.GetSpec();
            spec.Size = size;
            return spec;
        }

        public IType Numeric(int scale, int precision)
        {
            var spec = _conf.TypeManager.Numeric.GetSpec();
            spec.Scale = scale;
            spec.Precision = precision;
            return spec;
        }

        public IType Date()
        {
            return _conf.TypeManager.DateTime;
        }

        public IType Parse(string typeName)
        {
            if (typeName.Contains("."))
            {
                var parts = typeName.Split(".");

                return _conf.TypeManager.FindComponentByName(parts[0]).FindTypeByName(parts[1]);
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