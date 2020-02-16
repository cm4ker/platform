using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Xunit;
using Xunit.Sdk;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Visitor;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.AST;
using ZenPlatform.Language.Ast.Definitions;

namespace ZenPlatform.Compiler.Tests
{
    public class AstWalkerTest
    {
        [Fact]
        public void ClassTableTest()
        {
            var cu = new CompilationUnit(null, new List<UsingBase>(), new List<TypeEntity>(),
                new List<NamespaceDeclaration>
                {
                    new NamespaceDeclaration(null, "System", new List<UsingBase>(), new List<TypeEntity>
                    {
                        new BindingClass("Test", new UnknownType("Test"))
                    }, null)
                });

            AccumInfo ai = new AccumInfo();

            ai.Visit(cu);

            var t = ai.Table.FindType("System.Test");

            Assert.NotNull(t);


            /*
                 AstClass BaseAstClass
                    V        V
             type Test : SomeClass
             {
               
             
             }
                         
             AstClass : BaseAstClass
                        Lookup:
                            1) ClassTable (search here firstly) (check for the AST class, if found throw AmbiguousTypeNameException)
                            2) If not found start search in the AST tree, if found we compile this AST in-priority
                            3) If not found throw exception
             
             type Collection : List<SomeRowType> 
                    
                          
             
             AstTree => ClassTable (contains all AstClasses with CLR Type relation)
             
             
             */
        }
    }
}