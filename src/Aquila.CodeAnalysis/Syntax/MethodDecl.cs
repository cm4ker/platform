﻿using System.Collections.Immutable;
using System.Data;
using System.Linq;
using Aquila.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Syntax
{
    public partial class FuncDecl
    {
        private MemberModifiers? _memberModifers;

        public MemberModifiers GetModifiers()
        {
            if (_memberModifers == null)
            {
                _memberModifers = AstUtils.GetModifiers(Modifiers);
            }

            return _memberModifers.Value;
        }


        public bool IsGlobal => Parent is CompilationUnitSyntax;
    }

    public partial class CompilationUnitSyntax
    {
        public string ModuleName => this.Module?.Name.GetUnqualifiedName().Identifier.Text ?? "main";

        private ImmutableArray<FuncDecl> _funcs;
        private ImmutableArray<TypeDecl> _types;

        public ImmutableArray<FuncDecl> Functions =>
            (_funcs.IsDefaultOrEmpty) ? _funcs = this.Members.OfType<FuncDecl>().ToImmutableArray() : _funcs;

        public ImmutableArray<TypeDecl> Types =>
            (_types.IsDefaultOrEmpty) ? _types = this.Members.OfType<TypeDecl>().ToImmutableArray() : _types;
    }
}