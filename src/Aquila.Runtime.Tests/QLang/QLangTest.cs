using System.Linq;
using System.Reflection.Metadata;
using Xunit;
using Aquila.Core.Querying.Model;

namespace Aquila.Core.Querying.Test
{
    public class QLangTest
    {
        private Querying.Model.QLang _m;

        public QLangTest()
        {
            _m = new QLang();
        }

        [Fact]
        public void QlangSimpleTest()
        {
            _m.reset();
            _m.new_scope();

            _m.ld_component("Entity");
            _m.ld_object_type("Store");
            _m.@as("A");
            _m.@from();

            _m.create(QObjectType.FieldList);
            _m.ld_name("A");
            _m.ld_field("Id");
            _m.st_elem();
            _m.select();

            _m.new_query();

            var query = (QQuery)_m.top();

            Assert.NotNull(query);
        }

        [Fact]
        public void QlangSimpleAliasedChildrens()
        {
            _m.reset();
            _m.new_scope();


            _m.ld_component("Entity");
            _m.ld_object_type("Store");
            _m.@as("A");


            var ds = _m.top() as QAliasedDataSource;
            _m.@from();
            _m.create(QObjectType.FieldList);

            Assert.Single(ds.GetChildren());
            _m.ld_name("A");
            Assert.Single(ds.GetChildren());
            _m.ld_field("Id");
            Assert.Single(ds.GetChildren());
            _m.new_query();

            var query = (QQuery)_m.top();

            Assert.Single(ds.GetChildren());
            Assert.NotNull(query);
        }

        [Fact]
        public void QlangTest()
        {
            _m.reset();

            _m.new_scope();


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


            _m.ld_name("A");
            _m.ld_field("Store");

            var result = (QField)_m.top();

            Assert.Equal(1, result.GetExpressionType().Count());

            _m.new_query();

            var query = (QQuery)_m.top();

            Assert.NotNull(query);
        }


        [Fact]
        public void QlangCaseTest()
        {
            _m.reset();
            _m.new_scope();


            _m.ld_component("Entity");
            _m.ld_object_type("Invoice");
            _m.@as("A");

            _m.@from();

            _m.create(QObjectType.WhenList);

            //condition
            _m.ld_name("A");
            _m.ld_field("Store");
            _m.ld_param("Store");
            _m.eq();

            //result
            _m.ld_const(1);

            _m.when();

            _m.st_elem();

            _m.ld_name("A");
            _m.ld_field("Store");
            _m.ld_param("Store");
            _m.ne();
            _m.ld_const(2);
            _m.when();
            _m.st_elem();

            //else
            _m.ld_const("Test");

            _m.@case();

            var result = (QCase)_m.top();

            Assert.NotNull(result);
            Assert.Equal(2, result.Whens.Count());


            var wens = result.Whens.ToArray();
            Assert.NotNull(wens[0]);
            Assert.NotNull(wens[0].Then);
            Assert.NotNull(result.Else);

            Assert.Equal(2, result.GetExpressionType().Count());

            _m.new_query();

            var query = (QQuery)_m.top();

            Assert.NotNull(query);
        }

        [Fact]
        public void NastedQueryTest()
        {
            _m.reset();
            _m.new_scope();

            _m.ld_component("Entity");
            _m.ld_object_type("Invoice");
            _m.@as("A");

            _m.create(QObjectType.JoinList);

            //start nested query
            _m.new_scope();

            _m.ld_component("Entity");
            _m.ld_object_type("Store");
            _m.@as("B");

            _m.@from();

            _m.create(QObjectType.FieldList);
            _m.ld_name("B");
            _m.ld_field("Id");
            _m.@as("NestedIdField");
            _m.st_elem();
            _m.@select();

            _m.new_query();

            //store query on stack

            Assert.True(_m.top() is QNestedQuery);

            _m.@as("NestedQuery");

            _m.ld_name("A");
            _m.ld_field("Store");

            _m.ld_name("NestedQuery");
            _m.ld_field("NestedIdField");

            _m.eq();
            _m.join();

            _m.st_elem();
            _m.@from();

            _m.create(QObjectType.FieldList);

            _m.ld_name("A");
            _m.ld_field("Store");
            _m.st_elem();
            _m.@select();


            _m.new_query();
            var query = _m.top();

            var q = Assert.IsType<QQuery>(query);
            Assert.NotNull(q.From);
            Assert.NotNull(q.Select);
        }

        [Fact]
        public void WhereTest()
        {
            _m.reset();
            _m.new_scope();


            _m.ld_component("Entity");
            _m.ld_object_type("Invoice");
            _m.@as("A");


            _m.ld_name("A");
            _m.ld_field("Id");

            _m.ld_param("P_01");

            _m.eq();

            _m.@where();
            Assert.True(_m.top() is QWhere);

            _m.create(QObjectType.FieldList);
            _m.ld_name("A");
            _m.ld_field("Store");
            var result = (QField)_m.top();
            Assert.Equal(1, result.GetExpressionType().Count());

            _m.st_elem();
            _m.@select();
            _m.new_query();

            var query = (QQuery)_m.top();

            Assert.NotNull(query);
            Assert.NotNull(query.Where);
        }

        [Fact]
        public void LookupTest()
        {
            _m.reset();
            _m.new_scope();


            _m.ld_component("Entity");
            _m.ld_object_type("Invoice");
            _m.@as("A");


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

            _m.new_query();
        }

        [Fact]
        public void OrderByTest()
        {
            _m.reset();
            _m.new_scope();

            _m.ld_component("Entity");
            _m.ld_object_type("Store");
            _m.@as("A");
            _m.@from();

            _m.create(QObjectType.FieldList);
            _m.ld_name("A");
            _m.ld_field("Id");
            _m.st_elem();
            _m.select();

            _m.create(QObjectType.OrderList);
            _m.dup();


            _m.ld_const(1);
            _m.ld_sort(QSortDirection.Ascending);
            _m.create(QObjectType.OrderExpression);
            _m.st_elem();
            _m.order_by();

            _m.new_query();

            var query = (QQuery)_m.top();

            Assert.NotNull(query);
            Assert.NotNull(query.OrderBy);
            Assert.NotEmpty(query.OrderBy.Expressions);
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

            _m.new_scope();
            _m.create(QObjectType.FieldList);
            _m.dup();

            _m.ld_component("Entity");
            _m.ld_object_type("Store");
            _m.ld_field("Property1");
            _m.lookup("Id");
            _m.st_elem();

            _m.st_data_request();

            var dr = _m.top() as QDataRequest;

            Assert.Single(dr.Source);
        }
    }
}