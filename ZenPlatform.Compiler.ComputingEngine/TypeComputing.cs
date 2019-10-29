using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualBasic.CompilerServices;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Language.Ast.Definitions;

namespace ZenPlatform.Compiler.ComputingEngine
{
    /// <summary>
    /// Вычислитель типов. Производится на основании текущего контекста сборки
    /// </summary>
    /// <param name="assembly"></param>
    public class TypeComputing
    {
        private Dictionary<string, IType> _cached;

        private readonly IAssembly _assembly;
        private readonly SystemTypeBindings _stb;

        public TypeComputing(IAssembly assembly)
        {
            _assembly = assembly;
            _stb = assembly.TypeSystem.GetSystemBindings();
            _cached = new Dictionary<string, IType>();
        }

        public IType Compute(TypeSyntax typeSyntax)
        {
            if (typeSyntax is SingleTypeSyntax stn)
            {
                if (_cached.TryGetValue(stn.TypeName, out var type))
                {
                    return type;
                }
            }

            else if (typeSyntax is PrimitiveTypeSyntax ptn)
            {
                return ptn.Kind switch
                {
                    TypeNodeKind.Boolean => _stb.Boolean,
                    TypeNodeKind.Int => _stb.Int,
                    TypeNodeKind.Char => _stb.Char,
                    TypeNodeKind.Double => _stb.Double,
                    TypeNodeKind.String => _stb.String,
                };
            }

            else if (typeSyntax is ArrayTypeSyntax atn)
            {
                return Compute(atn.ElementType).MakeArrayType();
            }

            else if (typeSyntax is UnionTypeSyntax utn)
            {
                throw new NotImplementedException();
            }

            return null;
        }

        public void Register(TypeSyntax typeSyntax, IType type)
        {
            if (typeSyntax is SingleTypeSyntax stn)
                if (!_cached.ContainsKey(stn.TypeName))
                {
                    _cached.Add(stn.TypeName, type);
                }
        }
    }
}