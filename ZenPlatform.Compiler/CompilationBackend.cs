using System.IO;
using Antlr4.Runtime;
using ZenPlatform.Compiler.AST;
using ZenPlatform.Compiler.AST.Definitions;
using ZenPlatform.Compiler.Generation;

namespace ZenPlatform.Compiler
{
    public class CompilationBackend
    {
        /// <summary>
        /// Скомпилировать поток символов и записать в сборку
        /// </summary>
        /// <param name="input"></param>
        /// <param name="assemblyDefinition"></param>
        public void Compile(Stream input)
        {
            var pTree = Parse(input);
//            ZLanguageVisitor v = new ZLanguageVisitor();
//            var module = v.VisitEntryPoint(pTree.entryPoint()) as CompilationUnit;
        }


        /// <summary>
        /// Распарсить исходный текст модуля
        /// </summary>
        /// <param name="input">Входящий поток символов</param>
        /// <returns></returns>
        private ZSharpParser Parse(Stream input)
        {
            AntlrInputStream inputStream = new AntlrInputStream(input);
            ZSharpLexer lexer = new ZSharpLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
            ZSharpParser parser = new ZSharpParser(commonTokenStream);

            parser.AddErrorListener(new Listener());

            return parser;
        }
    }
}