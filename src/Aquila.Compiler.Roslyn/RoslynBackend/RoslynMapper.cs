using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DblibOpCode = dnlib.DotNet.Emit.OpCode;
using DnlibOpCodes = dnlib.DotNet.Emit.OpCodes;
using DnlibTypeAttr = dnlib.DotNet.TypeAttributes;
using SreOpCode = System.Reflection.Emit.OpCode;
using SreOpCodes = System.Reflection.Emit.OpCodes;
using SreTypeAttr = System.Reflection.TypeAttributes;


namespace Aquila.Compiler.Roslyn.RoslynBackend
{
    public static class RoslynMapper
    {
        static RoslynMapper()
        {
            foreach (var sreField in typeof(SreOpCodes)
                .GetFields(BindingFlags.Static | BindingFlags.Public)
                .Where(f => f.FieldType == typeof(SreOpCode)))

            {
                var sre = (SreOpCode) sreField.GetValue(null);
                var cecilField = typeof(DnlibOpCodes).GetField(sreField.Name);
                if (cecilField == null)
                    continue;
                var cecil = (DblibOpCode) cecilField.GetValue(null);
                OpCodeDic[sre] = cecil;
            }

            var sreType = typeof(SreTypeAttr);
            var cecilType = typeof(DnlibTypeAttr);

            foreach (SreTypeAttr sEnumValue in sreType.GetEnumValues())
            {
                var sName = sreType.GetEnumName(sEnumValue);

                foreach (DnlibTypeAttr cEnumValue in cecilType.GetEnumValues())
                {
                    try
                    {
                        var cName = cecilType.GetEnumName(cEnumValue);
                        if (cName == sName)
                        {
                            TypeAttrDic.TryAdd(sEnumValue, cEnumValue);
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }

//            //Custom
//            TypeAttrDic.Add(SreTypeAttr.NotPublic, DnlibTypeAttr.NotPublic);
        }

        public static Dictionary<SreOpCode, DblibOpCode> OpCodeDic = new Dictionary<SreOpCode, DblibOpCode>();
        public static Dictionary<SreTypeAttr, DnlibTypeAttr> TypeAttrDic = new Dictionary<SreTypeAttr, DnlibTypeAttr>();

        static IEnumerable<Enum> GetFlags(Enum input)
        {
            foreach (Enum value in Enum.GetValues(input.GetType()))
                if (input.HasFlag(value))
                    yield return value;
        }

        public static DnlibTypeAttr Convert(this SreTypeAttr val)
        {
            DnlibTypeAttr result = 0;

            foreach (var @enum in GetFlags(val))
            {
                var flag = (SreTypeAttr) @enum;

                result |= TypeAttrDic[flag];
            }

            return result;
        }
    }
}