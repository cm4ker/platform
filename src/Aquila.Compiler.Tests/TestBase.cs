using System.Collections.Generic;
using System.IO;
using Antlr4.Runtime;
using Aquila.Compiler.Cecil;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Preprocessor;
using Aquila.Language.Ast;
using Aquila.Language.Ast.Definitions;
using CompilationUnit = Aquila.Language.Ast.Definitions.CompilationUnit;

namespace Aquila.Compiler.Tests
{
    public abstract class TestBase
    {
        private ZLanguageVisitor _zlv;
        IAssemblyPlatform ap = new CecilAssemblyPlatform();

        public TestBase()
        {
            _zlv = new ZLanguageVisitor();
        }

        public int A(object i)
        {
            return (int) i + 1;
        }

        public string Transpile(string text)
        {
            var parser = Parse(text);
            var result = (CompilationUnit) _zlv.Visit(parser.entryPoint());

            var glob = new Root(null, new CompilationUnitList {result});

//            AstSymbolVisitor sv = new AstSymbolVisitor();
//            sv.Visit(glob);


            return null;
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


    // This is a collectible (unloadable) AssemblyLoadContext that loads the dependencies

    // of the plugin from the plugin's binary directory.
}