using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet.Writer;
using ServiceStack;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Core.Language.QueryLanguage.ZqlModel;

namespace ZenPlatform.Core.Language.QueryLanguage.Model
{
    public class LogicStack : Stack<object>
    {
        public XCComponent PopComponent()
        {
            return (XCComponent) this.Pop();
        }
    }

    public class QLang
    {
        private readonly XCRoot _conf;
        private InstructionContext _instructionContext = InstructionContext.None;
        private QueryContext _queryContext;
        private LogicStack _logicStack;

        private enum QueryContext
        {
            None,
            Select,
            From,
            GroupBy,
            Having,
            Where
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
        }

        #region Context modifiers

        public void m_from()
        {
            _queryContext = QueryContext.From;
        }

        public void m_select()
        {
            _queryContext = QueryContext.Select;
        }

        public void m_where()
        {
            _queryContext = QueryContext.Where;
        }

        public void m_group_by()
        {
            _queryContext = QueryContext.GroupBy;
        }

        public void m_having()
        {
            _queryContext = QueryContext.Having;
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
            _logicStack.Push(new QObjectTable(type));
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

        public void ld_name(string name)
        {
        }

        public void ld_field(string name)
        {
        }

        #endregion

        public void alias(string alias)
        {
            if (_queryContext == QueryContext.From)
            {
                var source = _logicStack.Pop();

                if (source is IQAliased a)
                {
                    a.Alias = alias;
                }
                else
                {
                    throw new Exception($"You can't alias this object {source}");
                }
            }
            else if (_queryContext == QueryContext.Select)
            {
                
            }
        }

        public void beign_query()
        {
            if (_queryContext == QueryContext.None)
                _logicStack.Push(new BeginQueryToken());
        }

        private void pop()
        {
            _logicStack.Pop();
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
    ld_field "Id"                                SELECT SUM(A.Count) AS SumOfCount
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

    st_field <---save complex field on the stack
        
    st_query <--- save query on  the stack
    
    */
}