using System;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.EntityComponent.Configuration;

namespace ZenPlatform.EntityComponent
{
    public static class NamingConventions
    {
        public static string GetNamespace(this IXCObjectType se)
        {
            return se.Parent.GetCodeRule(CodeGenRuleType.NamespaceRule).GetExpression();
        }

        public static string GetDtoName(this IXCObjectType se)
        {
            var component = se.Parent;
            return
                $"{component.GetCodeRuleExpression(CodeGenRuleType.DtoPreffixRule)}{se.Name}{component.GetCodeRuleExpression(CodeGenRuleType.DtoPostfixRule)}";
        }

        public static string GetObjectName(this IXCObjectType se)
        {
            return se.Name;
        }

        public static string GetLinkName(this IXCObjectType se)
        {
            return se.GetLink().Name;
        }

        public static string GetManagerName(this IXCObjectType se)
        {
            return $"{se.Name}Manager";
        }
    }
}