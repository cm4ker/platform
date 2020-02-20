using System;
using System.Linq;
using MoreLinq.Extensions;
using Xunit;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.ConfigurationExample;
using ZenPlatform.Core.Querying.Model;
using ZenPlatform.QueryBuilder.Model;
using ZenPlatform.QueryBuilder.Visitor;
using ZenPlatform.Test.Tools;

namespace ZenPlatform.Core.Querying.Test
{
    public class QLangTest
    {
        private Project conf;
        private Querying.Model.QLang _m;

        public QLangTest()
        {
            conf = ConfigurationFactory.Create();
            _m = new QLang(conf);
        }

        [Fact]
        public void QlangSimpleTest()
        {
            _m.reset();
            _m.bg_query();
            _m.m_from();
            
            _m.ld_component("Entity");
            _m.ld_object_type("Store");
            _m.@as("A");
            
            _m.m_select();

            _m.ld_name("A");
            _m.ld_field("Id");
            
            _m.st_query();
            
            var query = (QQuery) _m.top();

            Assert.NotNull(query);
        }
        
        [Fact]
        public void QlangSimpleAliasedChildrens()
        {
            _m.reset();
            _m.bg_query();
            _m.m_from();
            
            _m.ld_component("Entity");
            _m.ld_object_type("Store");
            _m.@as("A");

            var ds = _m.top() as QAliasedDataSource;
            
            Assert.Equal(1, ds.Childs.Count);
            _m.m_select();
            Assert.Equal(1, ds.Childs.Count);
            _m.ld_name("A");
            Assert.Equal(1, ds.Childs.Count);
            _m.ld_field("Id");
            Assert.Equal(1, ds.Childs.Count);
            _m.st_query();
            
            var query = (QQuery) _m.top();
            
            Assert.Equal(1, ds.Childs.Count);
            Assert.NotNull(query);
        }

        [Fact]
        public void QlangTest()
        {
            _m.reset();

            _m.bg_query();

            _m.m_from();

            _m.ld_component("Entity");
            _m.ld_object_type("Invoice");
            _m.@as("A");

            _m.ld_component("Entity");
            _m.ld_object_type("Store");
            _m.@as("B");

            _m.ld_name("A");
            _m.ld_field("Store");

            _m.ld_name("B");
            _m.ld_field("Id");

            _m.eq();

            _m.join();

            _m.m_select();

            _m.ld_name("A");
            _m.ld_field("Store");

            var result = (QField) _m.top();

            Assert.Equal(3, result.GetExpressionType().Count());

            _m.st_query();

            var query = (QQuery) _m.top();

            Assert.NotNull(query);
        }


        [Fact]
        public void QlangCaseTest()
        {
            _m.reset();
            _m.bg_query();

            _m.m_from();

            _m.ld_component("Entity");
            _m.ld_object_type("Invoice");
            _m.@as("A");

            _m.m_select();

            _m.ld_name("A");
            _m.ld_field("Store");

            _m.ld_param("Store");

            _m.eq();

            _m.ld_const(1);

            _m.when();

            _m.ld_name("A");
            _m.ld_field("Store");

            _m.ld_param("Store");

            _m.eq();

            _m.ld_const(1);

            _m.when();

            _m.ld_const("Test");

            _m.@case();

            var result = (QCase) _m.top();

            Assert.NotNull(result);
            Assert.Equal(2, result.Whens.Count);

            Assert.NotNull(result.Whens[0]);
            Assert.NotNull(result.Whens[0].Then);
            Assert.NotNull(result.Else);


            Assert.Equal(3, result.GetExpressionType().Count());

            _m.st_query();

            var query = (QQuery) _m.top();

            Assert.NotNull(query);
        }

        [Fact]
        public void NastedQueryTest()
        {
            _m.reset();
            _m.bg_query();

            _m.m_from();

            _m.ld_component("Entity");
            _m.ld_object_type("Invoice");
            _m.@as("A");

            //start nested query
            _m.bg_query();
            _m.m_from();

            _m.ld_component("Entity");
            _m.ld_object_type("Store");
            _m.@as("B");

            _m.m_select();

            _m.ld_name("B");
            _m.ld_field("Id");
            _m.@as("NestedIdField");

            _m.st_query();
            //store query on stack

            Assert.True(_m.top() is QNestedQuery);

            _m.@as("NestedQuery");

            _m.ld_name("A");
            _m.ld_field("Store");

            _m.ld_name("NestedQuery");
            _m.ld_field("NestedIdField");

            _m.eq();

            _m.join();

            _m.m_select();

            _m.ld_name("A");
            _m.ld_field("Store");

            _m.st_query();
            var query = _m.top();
        }

        [Fact]
        public void WhereTest()
        {
            _m.reset();
            _m.bg_query();

            _m.m_from();

            _m.ld_component("Entity");
            _m.ld_object_type("Invoice");
            _m.@as("A");

            _m.m_where();

            _m.ld_name("A");
            _m.ld_field("Id");

            _m.ld_param("P_01");

            _m.eq();
            _m.m_select();

            Assert.True(_m.top() is QWhere);
            _m.ld_name("A");
            _m.ld_field("Store");

            var result = (QField) _m.top();

            Assert.Equal(3, result.GetExpressionType().Count());

            _m.st_query();

            var query = (QQuery) _m.top();

            Assert.NotNull(query);
            Assert.NotNull(query.Where);
        }

        [Fact]
        public void LookupTest()
        {
            _m.reset();
            _m.bg_query();

            _m.m_from();

            _m.ld_component("Entity");
            _m.ld_object_type("Invoice");
            _m.@as("A");

            _m.m_select();
            _m.ld_name("A");
            _m.ld_field("Store");
            var storeField = _m.top() as QField;
            Assert.NotNull(storeField);

            _m.lookup("Invoice");
            var field = _m.top() as QLookupField;
            Assert.NotNull(field);

            _m.lookup("Store");
            field = _m.top() as QLookupField;
            Assert.NotNull(field);

            Assert.Equal(storeField.GetExpressionType(), field.GetExpressionType());

            _m.st_query();
        }


        [Fact]
        public void DataRequestTest()
        {
            /*
             
                Entity.Invoice.Store.Name
                ^      ^       ^     ^
                Component      Field
                       Type          Lookup
                             
             */
            _m.reset();

            _m.begin_data_request();
            _m.ld_component("Entity");
            _m.ld_object_type("Invoice");
            _m.ld_field("Store");
            _m.lookup("Name");
            _m.st_data_request();

            var dr = _m.top() as QDataRequest;

            Assert.Single(dr.Source);


            // DataRequestGenerator drg = new DataRequestGenerator();

            // drg.Gen(dr);
        }
    }
}