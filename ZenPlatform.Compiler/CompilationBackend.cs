using System;
using System.IO;
using Antlr4.Runtime;
using ZenPlatform.Compiler.AST;
using ZenPlatform.Compiler.AST.Definitions;
using ZenPlatform.Compiler.Cecil;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Generation;
using ZenPlatform.Compiler.Generation.NewGenerator;
using ZenPlatform.Compiler.Preprocessor;
using ZenPlatform.Compiler.Sre;
using ZenPlatform.Compiler.Visitor;
using ZenPlatform.Language.Ast.AST.Definitions;

namespace ZenPlatform.Compiler
{
    public interface ICompilationBackend
    {
        /// <summary>
        /// Скомпилировать поток символов и записать в сборку
        /// </summary>
        /// <param name="input"></param>
        /// <param name="assemblyDefinition"></param>
        IAssemblyBuilder Compile(Stream input);

        /// <summary>
        /// Скомпилировать поток символов и записать в сборку
        /// </summary>
        /// <param name="input"></param>
        /// <param name="assemblyDefinition"></param>
        IAssemblyBuilder Compile(TextReader input);
    }

    public class CompilationBackend : ICompilationBackend
    {
        /// <summary>
        /// Скомпилировать поток символов и записать в сборку
        /// </summary>
        /// <param name="input"></param>
        /// <param name="assemblyDefinition"></param>
        public IAssemblyBuilder Compile(Stream input)
        {
            return CompileTree(Parse(CreateInputStream(input)));
        }

        /// <summary>
        /// Скомпилировать поток символов и записать в сборку
        /// </summary>
        /// <param name="input"></param>
        /// <param name="assemblyDefinition"></param>
        public IAssemblyBuilder Compile(TextReader input)
        {
            return CompileTree(Parse(CreateInputStream(input)));
        }

        private IAssemblyBuilder CompileTree(ZSharpParser pTree)
        {
            IAssemblyPlatform ap = new SreAssemblyPlatform();

            var ab = ap.AsmFactory.Create(ap.TypeSystem, "Debug", new Version(1, 0));


            ZLanguageVisitor v = new ZLanguageVisitor(ap.TypeSystem);
            var module = v.VisitEntryPoint(pTree.entryPoint()) as CompilationUnit ?? throw new Exception();

            var glob = new Root();
            glob.CompilationUnits.Add(module);

            //Gen
            //Перед генерацией необходимо подготовить дерево символов
            AstSymbolVisitor sv = new AstSymbolVisitor();
            sv.Visit(glob);

            AstCreateMultitype cm = new AstCreateMultitype(ab);
            glob.Accept(cm);
            cm.Bake();

            var prm = new GeneratorParameters(module, ab, CompilationMode.Client);

            Generator g = new Generator(prm);

            return ab;
        }

        private ITokenStream CreateInputStream(Stream input)
        {
            return PreProcessor.Do(new AntlrInputStream(input));
        }

        private ITokenStream CreateInputStream(TextReader reader)
        {
            return PreProcessor.Do(new AntlrInputStream(reader));
        }


        /// <summary>
        /// Распарсить исходный текст модуля
        /// </summary>
        /// <param name="tokenStream"></param>
        /// <returns></returns>
        private ZSharpParser Parse(ITokenStream tokenStream)
        {
            ZSharpParser parser = new ZSharpParser(tokenStream);

            parser.AddErrorListener(new Listener());

            return parser;
        }
    }
}