using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CecilOpCode = Mono.Cecil.Cil.OpCode;
using CecilOpCodes = Mono.Cecil.Cil.OpCodes;
using CecilTypeAttr = Mono.Cecil.TypeAttributes;
using SreOpCode = System.Reflection.Emit.OpCode;
using SreOpCodes = System.Reflection.Emit.OpCodes;
using SreTypeAttr = System.Reflection.TypeAttributes;


namespace ZenPlatform.Compiler.Cecil
{
    public static class SreMapper
    {
        static SreMapper()
        {
            foreach (var sreField in typeof(SreOpCodes)
                .GetFields(BindingFlags.Static | BindingFlags.Public)
                .Where(f => f.FieldType == typeof(SreOpCode)))

            {
                var sre = (SreOpCode) sreField.GetValue(null);
                var cecilField = typeof(CecilOpCodes).GetField(sreField.Name);
                if (cecilField == null)
                    continue;
                var cecil = (CecilOpCode) cecilField.GetValue(null);
                OpCodeDic[sre] = cecil;
            }

            var sreType = typeof(SreTypeAttr);
            var cecilType = typeof(CecilTypeAttr);

            foreach (SreTypeAttr sEnumValue in sreType.GetEnumValues())
            {
                var sName = sreType.GetEnumName(sEnumValue);

                foreach (CecilTypeAttr cEnumValue in cecilType.GetEnumValues())
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

            //Custom
            TypeAttrDic.Add(SreTypeAttr.NotPublic, CecilTypeAttr.NotPublic);
        }

        public static Dictionary<SreOpCode, CecilOpCode> OpCodeDic = new Dictionary<SreOpCode, CecilOpCode>();
        public static Dictionary<SreTypeAttr, CecilTypeAttr> TypeAttrDic = new Dictionary<SreTypeAttr, CecilTypeAttr>();

        static IEnumerable<Enum> GetFlags(Enum input)
        {
            foreach (Enum value in Enum.GetValues(input.GetType()))
                if (input.HasFlag(value))
                    yield return value;
        }

        public static CecilTypeAttr Convert(this SreTypeAttr val)
        {
            CecilTypeAttr result = 0;

            foreach (var @enum in GetFlags(val))
            {
                var flag = (TypeAttributes) @enum;

                result |= TypeAttrDic[flag];
            }

            return result;
        }
    }
}