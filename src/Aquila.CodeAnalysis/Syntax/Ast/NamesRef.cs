﻿// Copyright(c) DEVSENSE s.r.o.
// All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the License); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS
// OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY
// IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABILITY OR NON-INFRINGEMENT.
//
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

using System;
using System.Diagnostics;
using Aquila.Syntax.Ast;
using Aquila.Syntax.Text;
using Microsoft.CodeAnalysis.Text;

namespace Aquila.Syntax.Syntax
{
    #region VariableNameRef

    /// <summary>
    /// Represents a variable name and its position within AST.
    /// </summary>
    [DebuggerDisplay("{_span,nq} {_name}")]
    public struct VariableNameRef
    {
        private readonly TextSpan _span;
        private readonly VariableName _name;

        /// <summary>
        /// Position of the name.
        /// </summary>
        public TextSpan Span => _span;

        /// <summary>
        /// Variable name.
        /// </summary>
        public VariableName Name => _name;

        public VariableNameRef(TextSpan span, string name)
            : this(span, new VariableName(name))
        {
        }

        public VariableNameRef(TextSpan span, VariableName name)
        {
            _span = span;
            _name = name;
        }

        public override string ToString() => _name.ToString();

        public static implicit operator VariableName(VariableNameRef self) => self.Name;
    }

    #endregion

    #region NameRef

    /// <summary>
    /// Represents a variable name and its position.
    /// </summary>
    [DebuggerDisplay("{_span,nq} {_name}")]
    public struct NameRef
    {
        private readonly TextSpan _span;
        private readonly Name _name;

        /// <summary>
        /// Position of the name.
        /// </summary>
        public TextSpan Span => _span;

        /// <summary>
        /// Variable name.
        /// </summary>
        public Name Name => _name;

        /// <summary>
        /// Gets value indicating the name is not empty.
        /// </summary>
        public bool HasValue => !string.IsNullOrEmpty(_name.Value);

        /// <summary>
        /// An empty name.
        /// </summary>
        public static NameRef Invalid => new NameRef(TextSpan.FromBounds(0, 0), Name.EmptyBaseName);

        public NameRef(TextSpan span, string name)
            : this(span, new Name(name))
        {
        }

        public NameRef(TextSpan span, Name name)
        {
            _span = span;
            _name = name;
        }

        /// <summary>
        /// Gets <see cref="Name"/> as <see cref="string"/>.
        /// </summary>
        public override string ToString() => _name.ToString();

        public static implicit operator Name(NameRef self) => self.Name;
    }

    #endregion

    #region QualifiedNameRef

    /// <summary>
    /// Represents a qualified name and its position.
    /// </summary>
    [DebuggerDisplay("{_span,nq} {_name}")]
    public struct QualifiedNameRef
    {
        private readonly TextSpan _span;
        private readonly QualifiedName _name;

        /// <summary>
        /// Position of the qualified name.
        /// </summary>
        public TextSpan Span => _span;

        /// <summary>
        /// Qualified name.
        /// </summary>
        public QualifiedName QualifiedName => _name;

        /// <summary>
        /// Gets value indicating the qualified name is not empty.
        /// </summary>
        public bool HasValue => !string.IsNullOrEmpty(_name.Name.Value) ||
                                (_name.Namespaces != null && _name.Namespaces.Length != 0);

        // internal static QualifiedNameRef FromTypeRef(TypeRef tref)
        // {
        //     // if (tref == null)
        //     // {
        //     //     return QualifiedNameRef.Invalid;
        //     // }
        //     // var qname = tref.QualifiedName;
        //     // if (qname.HasValue)
        //     //     return new QualifiedNameRef(tref.Span, qname.Value);
        //     // else
        //     throw new ArgumentException();
        // }

        /// <summary>
        /// Empty name.
        /// </summary>
        public static QualifiedNameRef Invalid =>
            new QualifiedNameRef(TextSpan.FromBounds(0, 0), Syntax.Name.EmptyBaseName, Syntax.Name.EmptyNames);

        public QualifiedNameRef(TextSpan span, Name name, Name[] namespaces, bool fullyQualified = false)
            : this(span, new QualifiedName(name, namespaces, fullyQualified))
        {
        }

        public QualifiedNameRef(TextSpan span, QualifiedName name)
        {
            _span = span;
            _name = name;
        }

        /// <summary>
        /// Gets <see cref="QualifiedName"/> as <see cref="string"/>.
        /// </summary>
        public override string ToString() => _name.ToString();

        public static implicit operator QualifiedName(QualifiedNameRef self) => self.QualifiedName;
    }

    #endregion
}