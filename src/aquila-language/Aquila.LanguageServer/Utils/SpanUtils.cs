using Aquila.LanguageServer.Protocol;
using Microsoft.CodeAnalysis;

namespace Aquila.LanguageServer
{
    static class SpanUtils
    {
        public static Protocol.Range AsRange(this Microsoft.CodeAnalysis.Location location) =>
            AsRange(location.GetLineSpan());

        public static Protocol.Range AsRange(this FileLinePositionSpan span)
        {
            return new Protocol.Range(
                new Position(span.StartLinePosition.Line, span.StartLinePosition.Character),
                new Position(span.EndLinePosition.Line, span.EndLinePosition.Character));
        }


        public static OmniSharp.Extensions.LanguageServer.Protocol.Models.Range AsRange2(
            this Microsoft.CodeAnalysis.Location location) =>
            AsRange2(location.GetLineSpan());

        public static OmniSharp.Extensions.LanguageServer.Protocol.Models.Range AsRange2(this FileLinePositionSpan span)
        {
            return new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range(
                new OmniSharp.Extensions.LanguageServer.Protocol.Models.Position(span.StartLinePosition.Line,
                    span.StartLinePosition.Character),
                new OmniSharp.Extensions.LanguageServer.Protocol.Models.Position(span.EndLinePosition.Line,
                    span.EndLinePosition.Character));
        }
    }
}