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
    }
}