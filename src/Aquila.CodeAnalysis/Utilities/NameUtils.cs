﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Aquila.CodeAnalysis.Semantics;
using Aquila.Syntax;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Symbols.Source;

namespace Aquila.Compiler.Utilities
{
    internal static class NameUtils
    {
        public static QualifiedName MakeQualifiedName(string name, string clrnamespace, bool fullyQualified)
        {
            if (string.IsNullOrEmpty(clrnamespace))
            {
                return new QualifiedName(new Name(name), Name.EmptyNames, fullyQualified);
            }

            // count name parts
            int ndots = 0;

            for (int i = 0; i < clrnamespace.Length; i++)
            {
                var ch = clrnamespace[i];
                if (ch == '.' || ch == QualifiedName.Separator)
                {
                    ndots++;
                }
            }

            // create name parts
            var names = new Name[ndots + 1];

            int lastDot = 0, n = 0;
            for (int i = 0; i < clrnamespace.Length; i++)
            {
                var ch = clrnamespace[i];
                if (ch == '.' || ch == QualifiedName.Separator)
                {
                    names[n++] = new Name(clrnamespace.Substring(lastDot, i - lastDot));
                    lastDot = i + 1;
                }
            }

            names[n++] = new Name(clrnamespace.Substring(lastDot, clrnamespace.Length - lastDot));
            Debug.Assert(n == names.Length);

            return new QualifiedName(new Name(name), names, fullyQualified);
        }

        /// <summary>
        /// Make QualifiedName from the string like AAA\BBB\XXX
        /// </summary>
        /// <returns>Qualified name.</returns>
        public static QualifiedName MakeQualifiedName(string name, bool fullyQualified)
        {
            if (string.IsNullOrEmpty(name))
                return new QualifiedName(Name.EmptyBaseName);

            // fully qualified
            if (name[0] == QualifiedName.Separator)
            {
                name = name.Substring(1);
                fullyQualified = true;
            }

            // parse name
            Name[] namespaces;

            int to = name.IndexOf(QualifiedName.Separator);
            if (to < 0)
            {
                namespaces = Name.EmptyNames;
            }
            else
            {
                int from = 0;
                List<Name> namespacesList = new List<Name>(4);
                do
                {
                    string part = name.Substring(from, to - from);
                    namespacesList.Add(new Name(part));
                    from = to + 1;
                } while ((to = name.IndexOf(QualifiedName.Separator, from)) > 0);

                name = name.Substring(from);
                namespaces = namespacesList.ToArray();
            }

            // create QualifiedName
            return new QualifiedName(new Name(name), namespaces, fullyQualified);
        }

        /// <summary>
        /// Gets CLR name using dot as a name separator.
        /// </summary>
        public static string ClrName(this QualifiedName qname)
        {
            if (qname.IsSimpleName) return qname.Name.Value;

            var ns = string.Join(".", qname.Namespaces);

            if (!string.IsNullOrEmpty(qname.Name.Value))
                ns += "." + qname.Name.Value;

            return ns;
        }

        /// <summary>
        /// Gets full CLR name including the namespace part.
        /// </summary>
        public static string GetFullName(this NamedTypeSymbol t)
        {
            return Microsoft.CodeAnalysis.MetadataHelpers.BuildQualifiedName(t.OriginalDefinition.NamespaceName,
                t.MetadataName);
        }

        /// <summary>
        /// Compares two arrays.
        /// </summary>
        public static bool NamesEquals(this Name[] names1, Name[] names2)
        {
            if (names1.Length != names2.Length)
                return false;

            for (int i = 0; i < names1.Length; i++)
                if (names1[i] != names2[i])
                    return false;

            return true;
        }

        /// <summary>
        /// Compares two qualified names.
        /// </summary>
        /// <remarks>
        /// The original comparison operator on <see cref="QualifiedName"/> fails when any of the comparands is not initialized.
        /// </remarks>
        public static bool NameEquals(this QualifiedName name1, QualifiedName name2)
        {
            bool name1Empty = name1.IsEmpty();
            bool name2Empty = name2.IsEmpty();
            if (name1Empty || name2Empty)
            {
                return name1Empty && name2Empty;
            }

            return name1 == name2;
        }

        /// <summary>
        /// Compares two variable names.
        /// </summary>
        /// <remarks>
        /// The original comparison operator on <see cref="QualifiedName"/> fails when any of the comparands is not initialized.
        /// </remarks>
        public static bool NameEquals(this VariableName name1, VariableName name2) => name1.Value == name2.Value;

        /// <summary>
        /// Gets value indicating whether given qualified name was not set.
        /// </summary>
        public static bool IsEmpty(this QualifiedName qname)
        {
            return (qname.Namespaces == null || qname.Namespaces.Length == 0) && string.IsNullOrEmpty(qname.Name.Value);
        }

        /// <summary>
        /// Gets value indicating whether given name was not set.
        /// </summary>
        public static bool IsEmpty(this VariableName name) => string.IsNullOrEmpty(name.Value);

        /// <summary>
        /// Gets value indicating whether given name was not set.
        /// </summary>
        public static bool IsValid(this VariableName name) => !IsEmpty(name);

        /// <summary>
        /// Gets variable name without leading <c>$</c>.
        /// </summary>
        /// <param name="varname">String in form of <c>$varname</c> or <c>$GLOBALS['varname']</c> or <c>'varname'</c></param>
        /// <returns>Variable name without leading <c>$</c> or <c>null</c>.</returns>
        public static string AsVarName(string varname)
        {
            if (varname != null && varname.Length != 0)
            {
                if (varname[0] == '$')
                {
                    // $varname
                    varname = varname.Substring(1);
                }

                if (varname.Length != 0)
                {
                    var lbrace = varname.IndexOf('[');
                    if (lbrace >= 0)
                    {
                        if (varname.StartsWith("GLOBALS", StringComparison.OrdinalIgnoreCase) ||
                            varname.StartsWith("_GLOBALS", StringComparison.OrdinalIgnoreCase))
                        {
                            // GLOBALS['varname']
                            // _GLOBALS['varname']
                            var rbrace = varname.IndexOf(']', lbrace + 1);
                            if (rbrace >= 0)
                            {
                                varname = varname.Substring(lbrace + 1, rbrace - lbrace - 2).Trim();
                                if (varname.Length > 2 &&
                                    varname[0] == varname[varname.Length - 1] &&
                                    (varname[0] == '\'' || varname[0] == '"'))
                                {
                                    return varname.Substring(1, varname.Length - 2);
                                }
                            }
                        }
                    }
                    else
                    {
                        return varname;
                    }
                }
            }

            //
            return null;
        }

        public static bool StringsEqual(this string str1, string str2, bool ignoreCase) => string.Equals(str1, str2,
            ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

        /// <summary>
        /// Special Aquila type and function names.
        /// </summary>
        public struct SpecialNames
        {
            public static QualifiedName ArrayAccess
            {
                get { return new QualifiedName(new Name("ArrayAccess")); }
            }

            public static QualifiedName Iterator
            {
                get { return new QualifiedName(new Name("Iterator")); }
            }

            public static QualifiedName Traversable
            {
                get { return new QualifiedName(new Name("Traversable")); }
            }

            public static QualifiedName Closure
            {
                get { return new QualifiedName(new Name("Closure")); }
            }

            public static QualifiedName Exception
            {
                get { return new QualifiedName(new Name("Exception")); }
            }

            public static QualifiedName stdClass
            {
                get { return new QualifiedName(new Name("stdClass")); }
            }

            public static QualifiedName System => new QualifiedName(new Name("System"));

            public static QualifiedName System_Object =>
                new QualifiedName(new Name("Object"), new[] { new Name("System") });

            public static QualifiedName System_DateTime =>
                new QualifiedName(new Name("DateTime"), new[] { new Name("System") });

            public static Name offsetGet
            {
                get { return new Name("offsetGet"); }
            }

            public static Name offsetSet
            {
                get { return new Name("offsetSet"); }
            }

            public static Name current
            {
                get { return new Name("current"); }
            }

            /// <summary>Special <c>shell_exec</c> function name.</summary>
            public static QualifiedName shell_exec
            {
                get { return new QualifiedName(new Name("shell_exec")); }
            }

            /// <summary>Special <c>is_null</c> function name.</summary>
            public static QualifiedName is_null
            {
                get { return new QualifiedName(new Name("is_null")); }
            }

            /// <summary>Special <c>assert</c> function name.</summary>
            public static QualifiedName assert
            {
                get { return new QualifiedName(new Name("assert")); }
            }

            /// <summary>Special <c>dirname</c> function name.</summary>
            public static QualifiedName dirname
            {
                get { return new QualifiedName(new Name("dirname")); }
            }

            /// <summary>Special <c>basename</c> function name.</summary>
            public static QualifiedName basename
            {
                get { return new QualifiedName(new Name("basename")); }
            }

            /// <summary><c>get_parent_class</c> function name.</summary>
            public static QualifiedName get_parent_class => new QualifiedName(new Name("get_parent_class"));

            /// <summary><c>method_exists</c> function name.</summary>
            public static QualifiedName method_exists => new QualifiedName(new Name("method_exists"));

            public static QualifiedName ini_get = new QualifiedName(new Name("ini_get"));

            public static QualifiedName extension_loaded = new QualifiedName(new Name("extension_loaded"));

            /// <summary><c>define</c> function name.</summary>
            public static QualifiedName define = new QualifiedName(new Name("define"));

            /// <summary><c>ord</c> function name.</summary>
            public static QualifiedName ord = new QualifiedName(new Name("ord"));
        }
    }
}