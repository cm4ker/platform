using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Aquila.Configuration.Contracts.TypeSystem;

namespace Aquila.Core.Querying.Model
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