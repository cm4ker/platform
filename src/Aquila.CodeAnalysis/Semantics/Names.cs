using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Aquila.CodeAnalysis.Semantics
{
    #region Name

    /// <summary>
    /// Used for names of methods and namespace components.
    /// </summary>
    [DebuggerNonUserCode]
    public struct Name : IEquatable<Name>, IEquatable<string>
    {
        public string Value
        {
            get { return value; }
        }

        private readonly string value;
        private readonly int hashCode;

        #region Special Names

        public static readonly Name[] EmptyNames = new Name[0];
        public static readonly Name EmptyBaseName = new Name("");
        public static readonly Name StaticClassName = new Name("static");
        public static readonly Name AutoloadName = new Name("__autoload");
        public static readonly Name ClrCtorName = new Name(".ctor");
        public static readonly Name ClrInvokeName = new Name("Invoke"); // delegate Invoke method
        public static readonly Name AppStaticName = new Name("AppStatic");
        public static readonly Name AppStaticAttributeName = new Name("AppStaticAttribute");
        public static readonly Name ExportName = new Name("Export");
        public static readonly Name ExportAttributeName = new Name("ExportAttribute");
        public static readonly Name DllImportAttributeName = new Name("DllImportAttribute");
        public static readonly Name DllImportName = new Name("DllImport");
        public static readonly Name OutAttributeName = new Name("OutAttribute");
        public static readonly Name OutName = new Name("Out");
        public static readonly Name DeclareHelperName = new Name("<Declare>");
        public static readonly Name LambdaFunctionName = new Name("<Lambda>");
        public static readonly Name ClosureFunctionName = new Name("{closure}");
        public static readonly Name AnonymousClassName = new Name("class@anonymous");

        #region SpecialMethodNames

        /// <summary>
        /// Contains special (or &quot;magic&quot;) method names.
        /// </summary>
        public static class SpecialMethodNames
        {
            /// <summary>Constructor.</summary>
            public static readonly Name Construct = new Name("__construct");

            /// <summary>Destructor.</summary>
            public static readonly Name Destruct = new Name("__destruct");

            /// <summary>Invoked when cloning instances.</summary>
            public static readonly Name Clone = new Name("__clone");

            /// <summary>Invoked when casting to string.</summary>
            public static readonly Name Tostring = new Name("__tostring");

            /// <summary>Invoked when an unknown field is read.</summary>
            public static readonly Name Get = new Name("__get");

            /// <summary>Invoked when an unknown field is written.</summary>
            public static readonly Name Set = new Name("__set");

            /// <summary>Invoked when an unknown method is called.</summary>
            public static readonly Name Call = new Name("__call");

            /// <summary>Invoked when an object is called like a function.</summary>
            public static readonly Name Invoke = new Name("__invoke");

            /// <summary>Invoked when an unknown method is called statically.</summary>
            public static readonly Name CallStatic = new Name("__callStatic");

            /// <summary>Invoked when object is being serialized.</summary>
            public static readonly Name Serialize = new Name("__serialize");
        };

        #endregion

        public bool IsConstructName
        {
            get { return this.Equals(SpecialMethodNames.Construct); }
        }

        public bool IsStaticClassName
        {
            get { return this.Equals(Name.StaticClassName); }
        }

        public bool IsReservedClassName
        {
            get { return IsStaticClassName; }
        }

        /// <summary>
        /// <c>true</c> if the name was generated for the 
        /// <see cref="AnonymousTypeDecl"/>, 
        /// <c>false</c> otherwise.
        /// </summary>
        public bool IsGenerated
        {
            get { return value.StartsWith(AnonymousClassName.Value); }
        }

        #endregion

        /// <summary>
        /// Creates a name. 
        /// </summary>
        /// <param name="value">The name shouldn't be <B>null</B>.</param>
        public Name(string value)
        {
            Debug.Assert(value != null);
            this.value = value;
            this.hashCode = StringComparer.OrdinalIgnoreCase.GetHashCode(value);
        }

        #region Utils

        /// <summary>
        /// Separator of class name and its static field in a form of <c>CLASS::MEMBER</c>.
        /// </summary>
        public const string ClassMemberSeparator = "::";

        /// <summary>
        /// Splits the <paramref name="value"/> into class name and member name if it is double-colon separated.
        /// </summary>
        /// <param name="value">Full name.</param>
        /// <param name="className">Will contain the class name fragment if the <paramref name="value"/> is in a form of <c>CLASS::MEMBER</c>. Otherwise <c>null</c>.</param>
        /// <param name="memberName">Will contain the member name fragment if the <paramref name="value"/> is in a form of <c>CLASS::MEMBER</c>. Otherwise it contains original <paramref name="value"/>.</param>
        /// <returns>True iff the <paramref name="value"/> is in a form of <c>CLASS::MEMBER</c>.</returns>
        public static bool IsClassMemberSyntax(string value, out string className, out string memberName)
        {
            Debug.Assert(value != null);
            //Debug.Assert(QualifiedName.Separator.ToString() == ":::" && !value.Contains(QualifiedName.Separator.ToString())); // be aware of deprecated namespace syntax

            int separator;
            if ((separator = value.IndexOf(':')) >= 0 && // value.Contains( ':' )
                (separator = System.Globalization.CultureInfo.InvariantCulture.CompareInfo.IndexOf(value,
                    ClassMemberSeparator, separator, value.Length - separator,
                    System.Globalization.CompareOptions.Ordinal)) > 0) // value.Contains( "::" )
            {
                className = value.Remove(separator);
                memberName = value.Substring(separator + ClassMemberSeparator.Length);
                return true;
            }
            else
            {
                className = null;
                memberName = value;
                return false;
            }
        }

        /// <summary>
        /// Determines if given <paramref name="value"/> is in a form of <c>CLASS::MEMBER</c>.
        /// </summary>
        /// <param name="value">Full name.</param>
        /// <returns>True iff the <paramref name="value"/> is in a form of <c>CLASS::MEMBER</c>.</returns>
        public static bool IsClassMemberSyntax(string value)
        {
            return value != null && value.Contains(ClassMemberSeparator);
        }

        #endregion

        #region Basic Overrides

        public override bool Equals(object obj)
        {
            return obj != null && obj.GetType() == typeof(Name) && Equals((Name)obj);
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }

        public override string ToString()
        {
            return this.value;
        }

        #endregion

        #region IEquatable<Name> Members

        public bool Equals(Name other)
        {
            return this.GetHashCode() == other.GetHashCode() && Equals(other.Value);
        }

        public static bool operator ==(Name name, Name other)
        {
            return name.Equals(other);
        }

        public static bool operator !=(Name name, Name other)
        {
            return !name.Equals(other);
        }

        #endregion

        #region IEquatable<string> Members

        public bool Equals(string other)
        {
            return string.Equals(value, other, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }

    #endregion

    #region VariableName

    /// <summary>
    /// Case-sensitive simple name in Unicode C normal form.
    /// Used for names of variables and constants.
    /// </summary>
    [DebuggerNonUserCode]
    public struct VariableName : IEquatable<VariableName>, IEquatable<string>
    {
        public string Value
        {
            get { return value; }
            set { this.value = value; }
        }

        private string value;

        #region Special Names

        public static readonly VariableName ThisVariableName = new VariableName("this");

        #region Autoglobals

        public const string EnvName = "_ENV";
        public const string ServerName = "_SERVER";
        public const string GlobalsName = "GLOBALS";
        public const string RequestName = "_REQUEST";
        public const string GetName = "_GET";
        public const string PostName = "_POST";
        public const string CookieName = "_COOKIE";
        public const string HttpRawPostDataName = "HTTP_RAW_POST_DATA";
        public const string FilesName = "_FILES";
        public const string SessionName = "_SESSION";

        #endregion

        public bool IsThisVariableName
        {
            get { return this == ThisVariableName; }
        }

        #region IsAutoGlobal

        /// <summary>
        /// Gets value indicting whether the name represents an auto-global variable.
        /// </summary>
        public bool IsAutoGlobal
        {
            get { return IsAutoGlobalVariableName(this.Value); }
        }

        /// <summary>
        /// Checks whether a specified name is the name of an auto-global variable.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Whether <paramref name="name"/> is auto-global.</returns>
        public static bool IsAutoGlobalVariableName(string name)
        {
            switch (name)
            {
                case GlobalsName:
                case ServerName:
                case EnvName:
                case CookieName:
                case HttpRawPostDataName:
                case FilesName:
                case RequestName:
                case GetName:
                case PostName:
                case SessionName:
                    return true;

                default:
                    return false;
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// Creates a name. 
        /// </summary>
        /// <param name="value">The name, cannot be <B>null</B> nor empty.</param>
        public VariableName(string value)
        {
            Debug.Assert(value != null);
            // TODO (missing from Mono): this.value = value.Normalize();

            this.value = value;
        }

        #region Basic Overrides

        public override bool Equals(object obj)
        {
            if (!(obj is VariableName)) return false;
            return Equals((VariableName)obj);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override string ToString()
        {
            return this.value;
        }

        #endregion

        #region IEquatable<VariableName> Members

        public bool Equals(VariableName other)
        {
            return this.value.Equals(other.value);
        }

        public static bool operator ==(VariableName name, VariableName other)
        {
            return name.Equals(other);
        }

        public static bool operator !=(VariableName name, VariableName other)
        {
            return !name.Equals(other);
        }

        #endregion

        #region IEquatable<string> Members

        public bool Equals(string other)
        {
            return value.Equals(other);
        }

        public static bool operator ==(VariableName name, string str)
        {
            return name.Equals(str);
        }

        public static bool operator !=(VariableName name, string str)
        {
            return !name.Equals(str);
        }

        #endregion
    }

    #endregion

    #region QualifiedName

    [DebuggerNonUserCode]
    public struct QualifiedName : IEquatable<QualifiedName>
    {
        #region Special names

        public static readonly QualifiedName Assert = new QualifiedName(new Name("assert"), Name.EmptyNames);
        public static readonly QualifiedName Error = new QualifiedName(new Name("<error>"), Name.EmptyNames);
        public static readonly QualifiedName Null = new QualifiedName(new Name("null"), Name.EmptyNames);
        public static readonly QualifiedName True = new QualifiedName(new Name("true"), Name.EmptyNames);
        public static readonly QualifiedName False = new QualifiedName(new Name("false"), Name.EmptyNames);
        public static readonly QualifiedName Array = new QualifiedName(new Name("array"), Name.EmptyNames);
        public static readonly QualifiedName Object = new QualifiedName(new Name("object"), Name.EmptyNames);
        public static readonly QualifiedName Int = new QualifiedName(new Name("int"), Name.EmptyNames);
        public static readonly QualifiedName String = new QualifiedName(new Name("string"), Name.EmptyNames);
        public static readonly QualifiedName Boolean = new QualifiedName(new Name("boolean"), Name.EmptyNames);
        public static readonly QualifiedName Bool = new QualifiedName(new Name("bool"), Name.EmptyNames);
        public static readonly QualifiedName Double = new QualifiedName(new Name("double"), Name.EmptyNames);
        public static readonly QualifiedName Float = new QualifiedName(new Name("float"), Name.EmptyNames);
        public static readonly QualifiedName Resource = new QualifiedName(new Name("resource"), Name.EmptyNames);
        public static readonly QualifiedName Callable = new QualifiedName(new Name("callable"), Name.EmptyNames);
        public static readonly QualifiedName Void = new QualifiedName(new Name("void"), Name.EmptyNames);
        public static readonly QualifiedName Iterable = new QualifiedName(new Name("iterable"), Name.EmptyNames);
        public static readonly QualifiedName This = new QualifiedName(new Name("This"), Name.EmptyNames);

        public bool IsSimpleName
        {
            get { return Namespaces.Length == 0; }
        }

        public bool IsReservedClassName
        {
            get { return this.IsSimpleName && this.name.IsReservedClassName; }
        }

        #endregion

        public const char Separator = '.';

        #region Properties

        /// <summary>
        /// The outer most namespace is the first in the array.
        /// </summary>
        public Name[] Namespaces
        {
            get { return namespaces; }
            set { namespaces = value; }
        }

        private Name[] namespaces;

        /// <summary>
        /// Base name. Contains the empty string for namespaces.
        /// </summary>
        public Name Name
        {
            get { return name; }
            set { name = value; }
        }

        private Name name;

        /// <summary>
        /// <c>True</c> if this represents fully qualified name (absolute namespace).
        /// </summary>
        public bool IsFullyQualifiedName
        {
            get { return isFullyQualifiedName; }
            internal set { isFullyQualifiedName = value; }
        }

        private bool isFullyQualifiedName;

        #endregion

        #region Construction

        internal QualifiedName(IList<string> names, bool hasBaseName, bool fullyQualified)
        {
            Debug.Assert(names != null && names.Count > 0);

            //
            if (hasBaseName)
            {
                name = new Name(names[names.Count - 1]);
                namespaces = new Name[names.Count - 1];
            }
            else
            {
                name = Name.EmptyBaseName;
                namespaces = new Name[names.Count];
            }

            for (int i = 0; i < namespaces.Length; i++)
                namespaces[i] = new Name(names[i]);

            //
            isFullyQualifiedName = fullyQualified;
        }

        public QualifiedName(Name name)
            : this(name, Name.EmptyNames, false)
        {
        }

        public QualifiedName(Name name, Name[] namespaces)
            : this(name, namespaces, false)
        {
        }

        public QualifiedName(Name name, Name[] namespaces, bool fullyQualified)
        {
            if (namespaces == null)
                throw new ArgumentNullException("namespaces");

            this.name = name;
            this.namespaces = namespaces;
            this.isFullyQualifiedName = fullyQualified;
        }

        internal QualifiedName(Name name, QualifiedName namespaceName)
        {
            Debug.Assert(namespaceName.name.Value == "");

            this.name = name;
            this.namespaces = namespaceName.Namespaces;
            this.isFullyQualifiedName = namespaceName.IsFullyQualifiedName;
        }

        internal QualifiedName(QualifiedName name, QualifiedName namespaceName)
        {
            Debug.Assert(namespaceName.name.Value == "");

            this.name = name.name;

            if (name.IsSimpleName)
            {
                this.namespaces = namespaceName.Namespaces;
            }
            else // used for nested types
            {
                this.namespaces = Enumerable.Concat(namespaceName.namespaces, name.namespaces).ToArray();
            }

            this.isFullyQualifiedName = namespaceName.IsFullyQualifiedName;
        }

        /// <summary>
        /// Make QualifiedName from the string like AAA\BBB\XXX
        /// </summary>
        /// <returns>Qualified name.</returns>
        public static QualifiedName Parse(string name, bool fullyQualified)
        {
            return Parse((name ?? string.Empty).AsSpan(), fullyQualified);
        }

        public static QualifiedName Parse(ReadOnlySpan<char> name, bool fullyQualified)
        {
            name = name.Trim();

            if (name.Length == 0)
            {
                return new QualifiedName(Name.EmptyBaseName);
            }

            // fully qualified
            if (name[0] == Separator)
            {
                name = name.Slice(1);
                fullyQualified = true;
            }

            // parse name
            Name[] namespaces;

            int lastNameStart = name.LastIndexOf(Separator) + 1;
            if (lastNameStart == 0)
            {
                // no namespaces
                namespaces = Name.EmptyNames;
            }
            else
            {
                var namespacesList = new List<Name>();

                int sep;
                while ((sep = name.IndexOf(Separator)) >= 0)
                {
                    if (sep > 0)
                    {
                        namespacesList.Add(new Name(name.Slice(0, sep).ToString()));
                    }

                    name = name.Slice(sep + 1);
                }

                namespaces = namespacesList.ToArray();
            }

            // create QualifiedName
            return new QualifiedName(new Name(name.ToString()), namespaces, fullyQualified);
        }

        /// <summary>
        /// Convert namespaces + name into list of strings.
        /// </summary>
        /// <returns>String List of namespaces (additionaly with <see cref="Name"/> component if it is not empty).</returns>
        internal List<string> ToStringList()
        {
            List<string> list = new List<string>(this.Namespaces.Select(x => x.Value));

            if (!string.IsNullOrEmpty(this.Name.Value))
                list.Add(this.Name.Value);

            return list;
        }

        /// <summary>
        /// Gets instance of <see cref="QualifiedName"/> with <see cref="QualifiedName.isFullyQualifiedName"/> set.
        /// </summary>
        public QualifiedName WithFullyQualified(bool fullyQualified)
        {
            if (fullyQualified == this.isFullyQualifiedName)
            {
                return this;
            }
            else
            {
                return new QualifiedName(this.name, this.namespaces, fullyQualified);
            }
        }

        #endregion

        #region Basic Overrides

        public override bool Equals(object obj)
        {
            return obj != null && obj.GetType() == typeof(QualifiedName) && this.Equals((QualifiedName)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = name.GetHashCode();
                for (int i = 0; i < namespaces.Length; i++)
                    result ^= namespaces[i].GetHashCode() << (i & 0x0f);

                return result;
            }
        }

        public string ToString(Name? memberName, bool instance)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < namespaces.Length; i++)
            {
                result.Append(namespaces[i]);
                result.Append(Separator);
            }

            result.Append(Name);
            if (memberName.HasValue)
            {
                result.Append(instance ? "->" : "::");
                result.Append(memberName.Value.ToString());
            }

            return result.ToString();
        }

        public override string ToString()
        {
            var ns = this.namespaces;
            if (ns == null || ns.Length == 0)
            {
                return this.Name.Value;
            }
            else
            {
                StringBuilder result = new StringBuilder(ns.Length * 8);
                for (int i = 0; i < ns.Length; i++)
                {
                    result.Append(ns[i]);
                    result.Append(Separator);
                }

                result.Append(this.Name.Value);
                return result.ToString();
            }
        }

        #endregion

        #region IEquatable<QualifiedName> Members

        public bool Equals(QualifiedName other)
        {
            if (!this.name.Equals(other.name) || this.namespaces.Length != other.namespaces.Length) return false;

            for (int i = 0; i < namespaces.Length; i++)
            {
                if (!this.namespaces[i].Equals(other.namespaces[i]))
                    return false;
            }

            return true;
        }

        public static bool operator ==(QualifiedName name, QualifiedName other)
        {
            return name.Equals(other);
        }

        public static bool operator !=(QualifiedName name, QualifiedName other)
        {
            return !name.Equals(other);
        }

        #endregion
    }

    #endregion
}