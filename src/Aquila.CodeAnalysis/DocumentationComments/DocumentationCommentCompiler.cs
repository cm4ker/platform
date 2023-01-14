using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis.Symbols;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Symbols.Source;
using Aquila.CodeAnalysis.Symbols.Synthesized;
using Aquila.Compiler.Utilities;
using SourceFieldSymbol = Aquila.CodeAnalysis.Symbols.SourceFieldSymbol;

namespace Aquila.CodeAnalysis.DocumentationComments
{
    internal class DocumentationCommentCompiler
    {
        internal static void WriteDocumentationCommentXml(AquilaCompilation compilation, string assemblyName,
            Stream xmlDocStream, DiagnosticBag xmlDiagnostics, CancellationToken cancellationToken)
        {
            if (xmlDocStream != null)
            {
                new DocumentationCommentCompiler(xmlDocStream, xmlDiagnostics, cancellationToken)
                    .WriteCompilation(compilation, assemblyName)
                    .Dispose();
            }
        }

        readonly StreamWriter _writer;
        readonly DiagnosticBag _xmlDiagnostics;
        readonly CancellationToken _cancellationToken;

        readonly Dictionary<string, object> _written = new Dictionary<string, object>(512);

        bool AddToWritten(string id, object symbol)
        {
            var count = _written.Count;

            _written[id] = symbol;

            return count != _written.Count; // item has been added
        }

        private DocumentationCommentCompiler(Stream xmlDocStream, DiagnosticBag xmlDiagnostics,
            CancellationToken cancellationToken)
        {
            _writer = new StreamWriter(xmlDocStream, Encoding.UTF8);
            _xmlDiagnostics = xmlDiagnostics;
            _cancellationToken = cancellationToken;
        }

        static string XmlEncode(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            int len = text.Length;
            var encodedText = new StringBuilder(len + 8);

            for (int i = 0; i < len; ++i)
            {
                char ch = text[i];
                switch (ch)
                {
                    case '<':
                        encodedText.Append("&lt;");
                        break;
                    case '>':
                        encodedText.Append("&gt;");
                        break;
                    case '&':
                        encodedText.Append("&amp;");
                        break;
                    default:
                        encodedText.Append(ch);
                        break;
                }
            }

            return encodedText.ToString();
        }

        DocumentationCommentCompiler WriteCompilation(AquilaCompilation compilation, string assemblyName)
        {
            // //Aquila uses document writer for the manifest
            //
            // var manifest = new AquilaProjectManifest()
            // {
            //     ProjectName = assemblyName,
            //     RuntimeVersion = compilation.AquilaCorLibrary.Identity.Version.ToString()
            // };
            //
            // XmlSerializer s = new XmlSerializer(typeof(AquilaProjectManifest));
            //
            // s.Serialize(_writer, manifest);
            //
            // //
            return this;
        }

        void Dispose()
        {
            _writer.Flush();
            _writer.Dispose();
        }

        static void WriteSummary(TextWriter output, string summary)
        {
            if (string.IsNullOrWhiteSpace(summary)) return;

            summary = summary.Trim();

            if (summary.Length == 0) return;

            output.WriteLine("<summary>");
            output.Write(XmlEncode(summary));
            output.WriteLine();
            output.WriteLine("</summary>");
        }

        static void WriteParam(TextWriter output, string pname, string pdesc, string type = null)
        {
            if (string.IsNullOrWhiteSpace(pdesc) && string.IsNullOrEmpty(type))
            {
                return;
            }

            //

            output.Write("<param name=\"");
            output.Write(XmlEncode(pname));
            output.Write('\"');

            if (!string.IsNullOrEmpty(type))
            {
                // THIS IS A CUSTOM XML NOTATION, NOT SUPPORTED BY MS IDE

                // type="int|string|bool" 
                output.Write(" type=\"");
                output.Write(XmlEncode(type));
                output.Write('\"');
            }

            output.Write('>');
            output.Write(XmlEncode(pdesc));
            output.WriteLine("</param>");
        }

        static void WriteException(TextWriter output, string[] types, string pdesc)
        {
            if (string.IsNullOrWhiteSpace(pdesc) || types == null)
            {
                return;
            }

            //
            for (int i = 0; i < types.Length; i++)
            {
                var t = types[i];

                output.Write("<exception cref=\"");
                output.Write(XmlEncode(QualifiedName.Parse(t, true)
                    .ClrName())); // TODO: CommentIdResolver.GetId(..)    // TODO: translate correctly using current naming context
                output.Write('\"');
                output.Write('>');
                output.Write(XmlEncode(pdesc));
                output.WriteLine("</exception>");
            }
        }

        public static void WriteMethod(TextWriter output, SourceMethodSymbolBase method)
        {
            Contract.ThrowIfNull(output);
            Contract.ThrowIfNull(method);
        }

        void WriteMethod(string id, SourceMethodSymbolBase method)
        {
            if (!AddToWritten(id, method))
            {
                // already written
                return;
            }

            _writer.WriteLine($"<member name=\"{id}\">");

            WriteMethod(_writer, method);

            _writer.WriteLine("</member>");
        }

        void WriteMethod(SourceMethodSymbolBase method)
        {
            if (method.IsGlobalScope) return; // global code have no XML annotation

            WriteMethod(CommentIdResolver.GetId(method), method);
        }
    }
}