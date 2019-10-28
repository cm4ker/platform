using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Writer;
using ServiceStack;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Core.Language.QueryLanguage.ZqlModel;

namespace ZenPlatform.Core.Language.QueryLanguage.Model
{
    public class LogicScope
    {
        public LogicScope()
        {
            Scope = new Dictionary<string, IQDataSource>();
            ScopedDataSources = new List<IQDataSource>();
        }

        public Dictionary<string, IQDataSource> Scope { get; set; }

        public List<IQDataSource> ScopedDataSources { get; set; }
    }


    public class QLang
    {
        private readonly XCRoot _conf;
        private InstructionContext _iContext = InstructionContext.None;
        private QueryContext _qContext;
        private LogicStack _logicStack;
        private LogicScope _scope;

        private enum QueryContext
        {
            None = 0,
            From = 1,
            GroupBy = 2,
            Having = 3,
            Where = 4,
            Select = 5,
        }

        private enum InstructionContext
        {
            None,
            Component
        }

        public QLang(XCRoot conf)
        {
            _conf = conf;
            _logicStack = new LogicStack();
            _scope = new LogicScope();
        }

        #region Context modifiers

        private void ChangeContext(QueryContext nContext)
        {
            if (nContext < _qContext) throw new Exception($"Can't change context {_qContext} to {nContext}");
            _qContext = nContext;
        }

        public void m_from()
        {
            ChangeContext(QueryContext.From);
        }

        public void m_select()
        {
            ChangeContext(QueryContext.Select);
        }

        public void m_where()
        {
            ChangeContext(QueryContext.Where);
        }

        public void m_group_by()
        {
            ChangeContext(QueryContext.GroupBy);
        }

        public void m_having()
        {
            ChangeContext(QueryContext.Having);
        }

        #endregion

        #region Load instructions

        public void ld_component(string componentName)
        {
            var component = _conf.Data.Components.First(x => x.Info.ComponentName == componentName);
            _logicStack.Push(component);
        }

        public void ld_type(string typeName)
        {
            var type = _logicStack.PopComponent().GetTypeByName(typeName);
            var ds = new QObjectTable(type);
            _logicStack.Push(ds);
            _scope.ScopedDataSources.Add(ds);
        }

        /// <summary>
        /// Записать источник данных. Ссылка на него полностью получается из метаданных 
        /// </summary>
        /// <param name="componentName"></param>
        /// <param name="typeName"></param>
        /// <param name="alias"></param>
        public void ld_source(string componentName, string typeName, string p_alias = "")
        {
            ld_component(componentName);
            ld_type(typeName);

            if (!p_alias.IsNullOrEmpty())
            {
                alias(p_alias);
            }
        }

        public void ld_source_context()
        {
            _logicStack.Push(new QCombinedDataSource(_scope.ScopedDataSources));
        }

        public void ld_name(string name)
        {
            if (_scope.Scope.TryGetValue(name, out var source))
            {
                _logicStack.Push(source);
            }
            else
            {
                throw new Exception($"The name :'{name}' not found in scope");
            }
        }

        public void ld_field(string name)
        {
            var ds = _logicStack.PopDataSource();

            var result = ds.GetFields().Where(x => x.GetName() == name).ToList();

            if (result.Count() > 1)
            {
                throw new Exception($"Ambiguous field with name {name}");
            }

            if (!result.Any())
            {
                throw new Exception($"Field with name {name} not found");
            }

            _logicStack.Push(result.First());
        }

        #endregion

        public void alias(string alias)
        {
            if (_qContext == QueryContext.From)
            {
                var source = _logicStack.PopDataSource();
                var ds = new QAliasedDataSource(source, alias);
                _logicStack.Push(ds);
                _scope.Scope.Add(alias, ds);
            }
            else if (_qContext == QueryContext.Select)
            {
                var expr = _logicStack.PopExpression();
                _logicStack.Push(new QAliasedSelectExpression(expr, alias));
            }
        }

        public void begin_query()
        {
            if (_qContext == QueryContext.None)
                _logicStack.Push(new BeginQueryToken());
        }

        public object pop()
        {
            return _logicStack.Pop();
        }

        private void clear()
        {
            _logicStack.Clear();
        }
    }

    /*
     
    begin_query 
    
    m_from
    
    ld_component "Document"
    ld_type "Invoice"
    alias "A"
    ld_component "Reference"
    ld_type "Contractors"
    alias "B"
    
    ld_name "A" <-- name context
    ld_field "Id"                                SELECT SUM(A.Count) AS SumOfCount, COUNT(Fld123)
                                            =>   FROM Document.Invoice A
    ld_name "B" <-- name context                    JOIN Reference.Contractors B
    ld_field "Id"                                        ON A.Id = B.Id
    
    eq
    
    on 
    
    join    
    
    m_select
    
    ld_name "A"
    ld_field "Count" 
    sum
    alias "SumOfCount" <------ depend on context automatically wrap into SelectAliasedExpr Or SourceAliasedExpr 
    ?st_field <---save complex field on the stack

    ld_source_context        // load entire context
    ld_field "Fld123"   
    count             
        
    st_query <--- save query on  the stack
    
    */
}