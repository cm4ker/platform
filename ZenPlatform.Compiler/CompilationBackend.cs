using System.IO;
using System.Threading;
using Antlr4.Runtime;
using Mono.Cecil;
using ZenPlatform.Compiler;
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
        public void Compile(Stream input, AssemblyDefinition assemblyDefinition)
        {
            var pTree = Parse(input);
            ZLanguageVisitor v = new ZLanguageVisitor();
            var module = v.VisitEntryPoint(pTree.entryPoint()) as Module;
            Generator g = new Generator(module, assemblyDefinition);
            g.Emit();
        }


        /// <summary>
        /// Скомпилировать поток символов в сборку
        /// </summary>
        /// <param name="input">Входящий поток символов</param>
        /// <param name="output">Поток для записи сборки</param>
        /// <param name="and">Имя сборки</param>
        /// <param name="moduleName">Имя модуля сборки</param>
        /// <param name="mk">Тип модуля сборки</param>
        public void Compile(Stream input, Stream output, AssemblyNameDefinition and, string moduleName, ModuleKind mk)
        {
            var assembly = AssemblyDefinition.CreateAssembly(and, moduleName, mk);

            Compile(input, assembly);

            assembly.Write(output);
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