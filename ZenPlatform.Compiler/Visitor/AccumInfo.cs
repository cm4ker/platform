using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Infrastructure;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.AST;
using ZenPlatform.Language.Ast.Definitions;

namespace ZenPlatform.Compiler.Visitor
{
    /// <summary>
    /// Визитор для заполнения классов
    /// </summary>
    public class AccumInfo : AstVisitorBase<object>
    {
        private ClassTable _table;

        public List<string> _cuNamespace;
        public List<string> _namespaceNamespace;
        public List<string> _bodyNamespace;

        public AccumInfo()
        {
            _cuNamespace = new List<string>();
        }

        public override object VisitCompilationUnit(CompilationUnit obj)
        {
            _cuNamespace.Clear();

            foreach (var @using in obj.Usings)
            {
                if (@using is UsingDeclaration ud)
                    _cuNamespace.Add(ud.Name);
            }

            return base.VisitCompilationUnit(obj);
        }


        public override object VisitClass(Class obj)
        {
            return base.VisitClass(obj);
        }


        public override object VisitTypeBody(TypeBody obj)
        {
            _bodyNamespace.Clear();

            foreach (var @using in obj.Usings)
            {
                if (@using is UsingDeclaration ud)
                    _bodyNamespace.Add(ud.Name);
            }

            return base.VisitTypeBody(obj);
        }
    }
}