// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System;
using System.Globalization;
using Aquila.CodeAnalysis.Errors;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis
{
    internal sealed class XmlSyntaxDiagnosticInfo : SyntaxDiagnosticInfo
    {
        static XmlSyntaxDiagnosticInfo()
        {
            ObjectBinder.RegisterTypeReader(typeof(XmlSyntaxDiagnosticInfo), r => new XmlSyntaxDiagnosticInfo(r));
        }

        private readonly XmlParseErrorCode _xmlErrorCode;

        internal XmlSyntaxDiagnosticInfo(XmlParseErrorCode code, params object[] args)
            : this(0, 0, code, args)
        {
        }

        internal XmlSyntaxDiagnosticInfo(int offset, int width, XmlParseErrorCode code, params object[] args)
            : base(offset, width, ErrorCode.WRN_XMLParseError, args)
        {
            _xmlErrorCode = code;
        }

        #region Serialization

        protected override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteUInt32((uint)_xmlErrorCode);
        }

        private XmlSyntaxDiagnosticInfo(ObjectReader reader)
            : base(reader)
        {
            _xmlErrorCode = (XmlParseErrorCode)reader.ReadUInt32();
        }

        #endregion

        public override string GetMessage(IFormatProvider formatProvider = null)
        {
            throw new NotImplementedException();
        }
    }
}