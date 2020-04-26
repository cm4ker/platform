using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.Core.Querying.Model
{
    /// <summary>
    /// Представляет параметр
    /// </summary>
    public partial class QParameter : QExpression
    {
        private List<IPType> _types = new List<IPType>();

        public void AddType(IPType type)
        {
            if (!_types.Contains(type)) _types.Add(type);
        }

        public override IEnumerable<IPType> GetExpressionType()
        {
            return _types;
        }
    }
}