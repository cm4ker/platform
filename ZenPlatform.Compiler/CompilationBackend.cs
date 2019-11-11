using System;
using System.Collections.Generic;
using System.IO;
using Antlr4.Runtime;
using Microsoft.CodeAnalysis;
using ZenPlatform.Compiler.AST;
using ZenPlatform.Compiler.Cecil;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Generation;
using ZenPlatform.Compiler.Preprocessor;
using ZenPlatform.Compiler.Visitor;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.QueryBuilder;


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
        private readonly XCRoot _root;

        public CompilationBackend(XCRoot root)
        {
            _root = root;
        }
        
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
            IAssemblyPlatform ap = new CecilAssemblyPlatform();

            var ab = ap.AsmFactory.Create(ap.TypeSystem, "Debug", new Version(1, 0));

            ZLanguageVisitor v = new ZLanguageVisitor();
            var module = v.VisitEntryPoint(pTree.entryPoint()) as CompilationUnit ?? throw new Exception();

            module.PrintPretty("", true);

            AstScopeRegister.Apply(module);

            var prm = new GeneratorParameters(new List<CompilationUnit> {module}, ab, CompilationMode.Client,
                SqlDatabaseType.SqlServer, _root);

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


    public static class ParserHelper
    {
        public static TypeBody ParseModuleBody(string text)
        {
            using (var s = new StringReader(text))
            {
                var p = Parse(CreateInputStream(s));

                var v = new ZLanguageVisitor();

                return (TypeBody) v.VisitModuleBody(p.moduleBody());
            }
        }

        public static TypeBody ParseTypeBody(string text)
        {
            using (var s = new StringReader(text))
            {
                var p = Parse(CreateInputStream(s));

                var v = new ZLanguageVisitor();

                return (TypeBody) v.VisitTypeBody(p.typeBody());
            }
        }

        public static Block ParseInstructionBlock(string text)
        {
            throw new NotImplementedException();
        }

        private static ITokenStream CreateInputStream(Stream input)
        {
            return PreProcessor.Do(new AntlrInputStream(input));
        }

        private static ITokenStream CreateInputStream(TextReader reader)
        {
            return PreProcessor.Do(new AntlrInputStream(reader));
        }


        public static ZSharpParser Parse(ITokenStream tokenStream)
        {
            ZSharpParser parser = new ZSharpParser(tokenStream);

            parser.AddErrorListener(new Listener());

            return parser;
        }
    }
}