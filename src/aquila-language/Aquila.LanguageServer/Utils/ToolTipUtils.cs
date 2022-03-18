using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis.Shared.Extensions;

namespace Aquila.LanguageServer
{
    internal static class ToolTipUtils
    {
        public static SourceSymbolSearcher.SymbolStat FindDefinition(AquilaCompilation compilation, string filepath,
            int line, int character)
        {
            var tree = compilation.SyntaxTrees.FirstOrDefault(t => t.FilePath == filepath);
            if (tree == null)
            {
                return null;
            }

            // Get the position in the file
            int position = tree.GetOffset(new LinePosition(line, character));
            if (position == -1)
            {
                return null;
            }

            var token = tree.GetTouchingTokenAsync(position, CancellationToken.None);

            // Find the bound node corresponding to the text position
            SourceSymbolSearcher.SymbolStat searchResult = null;
            // foreach (var routine in compilation.GetUserDeclaredRoutinesInFile(tree))
            // {
            //     // Consider only routines containing the position being searched (<Main> has span [0..0])
            //     if (routine.IsGlobalScope || routine.GetSpan().Contains(position))
            //     {
            //         // Search parameters at first
            //         searchResult = SourceSymbolSearcher.SearchParameters(routine, position);
            //         if (searchResult != null)
            //         {
            //             break;
            //         }
            //
            //         // Search the routine body
            //         searchResult = SourceSymbolSearcher.SearchCFG(compilation, routine.ControlFlowGraph, position);
            //         if (searchResult != null)
            //         {
            //             break;
            //         }
            //     }
            // }

            return searchResult;
        }

        public static IEnumerable<Protocol.Location> ObtainDefinition(AquilaCompilation compilation, string filepath,
            int line, int character)
        {
            var result = FindDefinition(compilation, filepath, line, character);
            if (result != null && result.Symbol != null)
            {
                ImmutableArray<Location> location;

                try
                {
                    location = result.Symbol.Locations;
                }
                catch
                {
                    yield break; // location not impl.
                }

                foreach (var loc in location)
                {
                    var pos = loc.GetLineSpan();
                    if (pos.IsValid)
                    {
                        yield return new Protocol.Location
                        {
                            Uri = new Uri(pos.Path).AbsoluteUri,
                            Range = pos.AsRange(),
                        };
                    }
                }
            }
        }

        /// <summary>
        /// Returns the text of a tooltip corresponding to the given position in the code.
        /// </summary>
        public static ToolTipInfo ObtainToolTip(AquilaCompilation compilation, string filepath, int line, int character)
        {
            return FormulateToolTip(FindDefinition(compilation, filepath, line, character));
        }

        static string FormulateTypeHint(ITypeSymbol type)
        {
            if (type == null)
            {
                return null;
            }

            return type.SpecialType switch
            {
                SpecialType.System_Int32 => "int",
                SpecialType.System_Int64 => "int",
                SpecialType.System_Double => "float",
                SpecialType.System_Boolean => "boolean",
                SpecialType.System_String => "string",
                SpecialType.System_Object => "object",
                _ => type.MetadataName switch
                {
                    _ => type.Name,
                }
            };
        }

        /// <summary>
        /// Creates the text of a tooltip.
        /// </summary>
        private static ToolTipInfo FormulateToolTip(SourceSymbolSearcher.SymbolStat searchResult)
        {
            if (searchResult == null || (searchResult.Symbol == null && searchResult.BoundExpression == null))
            {
                return null;
            }

            var ctx = searchResult.TypeCtx;
            var expression = searchResult.BoundExpression;
            var symbol = searchResult.Symbol;
            if (symbol is IErrorMethodSymbol errSymbol)
            {
                if (errSymbol.ErrorKind == ErrorMethodKind.Missing)
                {
                    return null;
                }

                symbol = errSymbol.OriginalSymbols.LastOrDefault();
            }

            var result = new StringBuilder(32);
            var kind = string.Empty;

            if (expression is BoundVariableRef varref && varref.Name.IsDirect)
            {
                var name = varref.Name.NameValue;

                if (symbol is IParameterSymbol)
                {
                    kind = "parameter";
                }
                else
                {
                    kind = "variable";
                }

                result.Append("$" + name);
            }
            else if (expression is BoundGlobalConst)
            {
                kind = "global constant";
                result.Append(((BoundGlobalConst)expression).Name);
            }

            else if (expression is BoundFieldRef fld)
            {
                return null;
            }

            else
            {
                return null;
            }

            // : type
            if (ctx != null)
            {
            }

            // constant value
            if (expression != null && expression.ConstantValue.HasValue)
            {
                //  = <value>
                var value = expression.ConstantValue.Value;
                string valueStr;
                if (value == null) valueStr = "NULL";
                else if (value is int) valueStr = ((int)value).ToString();
                else if (value is string)
                    valueStr = "\"" + ((string)value).Replace("\"", "\\\"").Replace("\n", "\\n") + "\"";
                else if (value is long) valueStr = ((long)value).ToString();
                else if (value is double) valueStr = ((double)value).ToString(CultureInfo.InvariantCulture);
                else if (value is bool) valueStr = (bool)value ? "TRUE" : "FALSE";
                else valueStr = value.ToString();

                if (valueStr != null)
                {
                    result.Append(" = ");
                    result.Append(valueStr);
                }
            }

            string description;

            try
            {
                description = XmlDocumentationToMarkdown(symbol?.GetDocumentationCommentXml());
            }
            catch
            {
                description = null;
            }

            // description
            return new ToolTipInfo(kind + "\n" + result.ToString(), description);
        }

        static string TrimLines(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            var result = new StringBuilder(text.Length);
            string line;
            using (var lines = new StringReader(text))
            {
                while ((line = lines.ReadLine()) != null)
                {
                    result.AppendLine(line.Trim());
                }
            }

            return result.ToString();
        }

        static string XmlDocumentationToMarkdown(string xmldoc)
        {
            if (string.IsNullOrWhiteSpace(xmldoc))
            {
                return string.Empty;
            }

            // trim the lines (may be misinterpreted as code block in markdown)
            xmldoc = TrimLines(xmldoc);

            // 
            var result = new StringBuilder(xmldoc.Length);
            var settings = new XmlReaderSettings
            {
                IgnoreComments = true,
                IgnoreProcessingInstructions = true,
                IgnoreWhitespace = true,
                ValidationType = ValidationType.None
            };

            using (var xml = XmlReader.Create(new StringReader("<summary>" + xmldoc + "</summary>"), settings))
            {
                bool skipped = false;
                while (skipped || xml.Read())
                {
                    skipped = false;

                    switch (xml.NodeType)
                    {
                        case XmlNodeType.Element:
                            // <summary>
                            switch (xml.Name.ToLowerInvariant())
                            {
                                case "summary":
                                    break; // OK
                                case "remarks":
                                    result.Append("\n\n**Remarks:**\n");
                                    break;
                                case "para":
                                case "p":
                                case "br":
                                    result.Append("\n\n");
                                    break; // continue
                                case "returns":
                                    result.Append("\n\n**Returns:**\n");
                                    break;
                                case "c":
                                    result.AppendFormat("**{0}**", xml.ReadInnerXml());
                                    skipped = true;
                                    break;
                                case "paramref":
                                    if (xml.HasAttributes)
                                        result.AppendFormat("**${0}**", xml.GetAttribute("name"));
                                    break;
                                case "code":
                                    result.AppendFormat("```\n{0}```\n", xml.ReadInnerXml());
                                    skipped = true;
                                    break;
                                case "see":
                                    if (xml.HasAttributes)
                                        result.AppendFormat("**{0}**",
                                            xml.GetAttribute("langword") ?? CrefToString(xml.GetAttribute("cref")));
                                    break;
                                case "a":
                                    if (xml.HasAttributes)
                                    {
                                        result.AppendFormat("({1})[{0}]", xml.GetAttribute("href"), xml.ReadInnerXml());
                                        skipped = true;
                                    }

                                    break;
                                // TODO: table, thead, tr, td
                                default:
                                    xml.Skip();
                                    skipped = true; // do not call Read()!
                                    break;
                            }

                            break;

                        case XmlNodeType.Text:
                            result.Append(xml.Value.Replace("*", "\\*"));
                            break;
                    }
                }
            }

            //
            return result.ToString().Trim();
        }

        static string CrefToString(string cref)
        {
            if (string.IsNullOrEmpty(cref))
            {
                return string.Empty;
            }

            int trim = Math.Max(cref.LastIndexOf(':'), cref.LastIndexOf('.'));
            return trim > 0
                ? cref.Substring(trim + 1)
                : cref;
        }
    }
}