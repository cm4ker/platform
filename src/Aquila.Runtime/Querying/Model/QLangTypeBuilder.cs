using System;
using System.Reflection;
using System.Text.RegularExpressions;


namespace Aquila.Core.Querying.Model
{
    public class QLangTypeBuilder
    {
        public QLangTypeBuilder()
        {
        }

        public TypeInfo String(int size)
        {
            throw new NotImplementedException();
        }

        public TypeInfo Numeric(int scale, int precision)
        {
            throw new NotImplementedException();
        }

        public TypeInfo Date()
        {
            throw new NotImplementedException();
        }

        public TypeInfo Int()
        {
            throw new NotImplementedException();
        }

        public TypeInfo Parse(string typeName)
        {
            if (typeName.Contains("."))
            {
                var parts = typeName.Split(".");

                throw new NotImplementedException();
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