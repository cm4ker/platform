using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.Data;
using ZenPlatform.DataComponent;
using ZenPlatform.DataComponent.QueryBuilders;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.Queries;

namespace ZenPlatform.Tests
{
    [TestClass]
    public class DataComponentTest
    {

        private PObjectType _pObject;

        public DataComponentTest()
        {
            _pObject = new PSimpleObjectType("test");
            var property = new PProperty(_pObject)
            {
                Name = "test_property"
            };
            property.Types.Add(new PString());
            _pObject.Properties.Add(property);

            property = new PProperty(_pObject)
            {
                Name = "test_property2",
            };
            property.Types.Add(new PGuid());
            _pObject.Properties.Add(property);
            _pObject.TableName = "test_table";
        }

        [TestMethod]
        public void ComponentQueryBuilderTest()
        {

            QueryBuilderComponent queryBuilder = new QueryBuilderComponent(_pObject);

            DBTable table = queryBuilder.MakeTable(_pObject);


       
        }
    }
}
