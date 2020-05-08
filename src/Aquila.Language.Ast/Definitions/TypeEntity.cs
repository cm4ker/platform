using System;
using Aquila.Compiler.Contracts.Symbols;
using Aquila.Language.Ast.AST;

namespace Aquila.Language.Ast.Definitions
{
    /// <summary>
    /// Тип сущности
    /// </summary>
    public abstract partial class TypeEntity
    {
        public SymbolType SymbolType => SymbolType.Type;


        public virtual string GetNamespace()
        {
            var parent = FirstParent<NamespaceDeclaration>();

            if (parent == null)
                return "";

            var ns = parent.GetNamespace();

            if (!string.IsNullOrEmpty(ns))
                return string.Join(".", ns, parent.Name);

            return parent.Name;
        }
    }

    public partial class NamespaceDeclaration
    {
        public void AddEntity(TypeEntity type)
        {
            this.Entityes.Add(type);
        }


        public string GetNamespace()
        {
            return this.FirstParent<NamespaceDeclaration>()?.Name ?? "";
        }
    }
}