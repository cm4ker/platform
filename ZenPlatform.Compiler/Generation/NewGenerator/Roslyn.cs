using System.Buffers.Text;
using System.Collections.Generic;
using System.IO.Compression;
using System.Net.WebSockets;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Host.Mef;
using Mono.CompilerServices.SymbolWriter;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Infrastructure;

namespace ZenPlatform.Compiler.Generation.NewGenerator
{
    public class CompilationOptions
    {
        public CompilationMode Mode { get; set; }
    }
}