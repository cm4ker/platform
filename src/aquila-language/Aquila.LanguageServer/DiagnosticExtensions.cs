using System;
using System.Collections.Generic;
using System.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Diagnostic = Microsoft.CodeAnalysis.Diagnostic;
using DiagnosticSeverity = Microsoft.CodeAnalysis.DiagnosticSeverity;


namespace Aquila.LanguageServer;

public static class DiagnosticExtensions
{
    public static IEnumerable<OmniSharp.Extensions.LanguageServer.Protocol.Models.Diagnostic> ToDiagnostics(
        this IEnumerable<Diagnostic> source)
        => source.Select(diagnostic => CreateDiagnostic(diagnostic));

    private static OmniSharp.Extensions.LanguageServer.Protocol.Models.Diagnostic CreateDiagnostic(
        Diagnostic diagnostic)
    {
        var (descCode, codeDescription) = GetDiagnosticDocumentation(diagnostic);
        return new OmniSharp.Extensions.LanguageServer.Protocol.Models.Diagnostic()
        {
            Severity = ToDiagnosticSeverity(diagnostic.Severity),
            Code = descCode ?? diagnostic.Id,
            Message = "Test", //diagnostic.GetMessage(),
            Source = "Aquila",
            Range = diagnostic.Location.AsRange2(),
            CodeDescription = codeDescription
        };
    }

    private static (string?, CodeDescription?) GetDiagnosticDocumentation(Diagnostic diagnostic)
    {
        if (diagnostic.Location.SourceTree?.FilePath != null)
        {
            // This shuffling of the Code to Uri gives us the message formatting
            // that is desired where the documentation link is displayed as the text
            // of the link. Otherwise the code is displayed rather than the Uri
            //
            // Default message format:
            //   Declared parameter must be referenced within the document scope. bicep core(no-unused-params) [2,7]
            //
            // Desired format:
            //   Declared parameter must be referenced within the document scope. bicep core(https://aka.ms/bicep/linter/no-unused-params) [2,7]

            var path = PathUtils.NormalizePath(diagnostic.Location.SourceTree?.FilePath);

            return new(path, new CodeDescription() { Href = new Uri(path) });
        }

        // no additional documentation
        return new(null, null);
    }

    private static OmniSharp.Extensions.LanguageServer.Protocol.Models.DiagnosticSeverity ToDiagnosticSeverity(
        DiagnosticSeverity level)
        => level switch
        {
            DiagnosticSeverity.Info => OmniSharp.Extensions.LanguageServer.Protocol.Models.DiagnosticSeverity
                .Information,
            DiagnosticSeverity.Warning => OmniSharp.Extensions.LanguageServer.Protocol.Models.DiagnosticSeverity
                .Warning,
            DiagnosticSeverity.Error => OmniSharp.Extensions.LanguageServer.Protocol.Models.DiagnosticSeverity.Error,
            _ => throw new ArgumentException($"Unrecognized level {level}"),
        };

    // private static Container<DiagnosticTag>? ToDiagnosticTags(DiagnosticLabel? label) => label switch
    // {
    //     null => null,
    //     DiagnosticLabel.Unnecessary => new Container<DiagnosticTag>(DiagnosticTag.Unnecessary),
    //     DiagnosticLabel.Deprecated => new Container<DiagnosticTag>(DiagnosticTag.Deprecated),
    //     _ => throw new ArgumentException($"Unrecognized label {label}"),
    // };
}