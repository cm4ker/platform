using System;
using System.Text.RegularExpressions;
using Aquila.Compiler.Aqua.TypeSystem;
using Aquila.Configuration.Structure;
using Aquila.Core.Contracts.TypeSystem;


namespace Aquila.Core.Querying.Model
{
    public class QLangTypeBuilder
    {
        private readonly TypeManager _conf;

        public QLangTypeBuilder(TypeManager conf)
        {
            _conf = conf;
        }

        public PType String(int size)
        {
            var spec = _conf.String.GetSpec();
            spec.SetSize(size);
            return spec;
        }

        public PType Numeric(int scale, int precision)
        {
            var spec = _conf.Numeric.GetSpec();
            spec.SetScale(scale);
            spec.SetPrecision(precision);
            return spec;
        }

        public PType Date()
        {
            return _conf.DateTime;
        }

        public PType Int()
        {
            return _conf.Int;
        }

        public PType Parse(string typeName)
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
                "int" => Int(),
                _ => throw new Exception($"Type def: {typeName} can't be resolved")
            };
        }
    }
}