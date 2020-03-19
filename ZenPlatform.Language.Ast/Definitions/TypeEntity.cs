using System;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST;

namespace ZenPlatform.Language.Ast.Definitions
{
    /// <summary>
    /// Тип сущности
    /// </summary>
    public abstract partial class TypeEntity
    {
        public SymbolType SymbolType => SymbolType.Type;


        public string GetNamespace()
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