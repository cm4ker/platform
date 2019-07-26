using System.IO;
using Antlr4.Runtime;
using ZenPlatform.Compiler.AST;
using ZenPlatform.Compiler.Cecil;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Generation.NewGenerator;
using ZenPlatform.Compiler.Preprocessor;
using ZenPlatform.Compiler.Visitor;
using ZenPlatform.Language.Ast.Definitions;
using SyntaxNodeExtensions = Microsoft.CodeAnalysis.SyntaxNodeExtensions;

namespace ZenPlatform.Compiler.Tests
{
    public abstract class TestBase
    {
        private VRoslyn _r;
        private ZLanguageVisitor _zlv;
        IAssemblyPlatform ap = new CecilAssemblyPlatform();

        public TestBase()
        {
            _r = new VRoslyn(new CompilationOptions() {Mode = CompilationMode.Client});
            _zlv = new ZLanguageVisitor(ap.TypeSystem);
        }

        public int A(object i)
        {
            return (int) i + 1;
        }

        public string Transpile(string text)
        {
            var parser = Parse(text);
            var result = (CompilationUnit) _zlv.Visit(parser.entryPoint());

            var glob = new Root();
            glob.CompilationUnits.Add(result);

            AstSymbolVisitor sv = new AstSymbolVisitor();
            sv.Visit(glob);

            var t = _r.Visit(result);
            return SyntaxNodeExtensions.NormalizeWhitespace(t).ToFullString();
        }

        private ZSharpParser Parse(ITokenStream tokenStream)
        {
            ZSharpParser parser = new ZSharpParser(tokenStream);

            parser.AddErrorListener(new Listener());

            return parser;
        }

        private ZSharpParser Parse(string text)
        {
            using TextReader tr = new StringReader(text);
            return Parse(tr);
        }

        public ZSharpParser Parse(TextReader input)
        {
            return Parse(CreateInputStream(input));
        }

        private ITokenStream CreateInputStream(Stream input)
        {
            return PreProcessor.Do(new AntlrInputStream(input));
        }

        private ITokenStream CreateInputStream(TextReader reader)
        {
            return PreProcessor.Do(new AntlrInputStream(reader));
        }
    }
}