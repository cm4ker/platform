using System.Collections.Generic;

namespace Aquila.Metadata
{
    public class TestMetadata
    {
        public static string DefaultConnetionString = "Data Source=.;Initial Catalog=TestDb;Integrated Security=true";

        public static EntityMetadataCollection GetTestMetadata()
        {
            var em1 = new EntityMetadata
            {
                Name = "Invoice",
                Properties =
                {
                    new EntityProperty { Name = "Name", Types = { new MetadataType { Name = "string", Size = 30 } } },
                    new EntityProperty
                    {
                        Name = "ComplexProperty",
                        Types =
                        {
                            new MetadataType { Name = "string", Size = 30 },
                            new MetadataType { Name = "int" },
                            new MetadataType { Name = "Entity.StoreLink" },
                            new MetadataType { Name = "Entity.ContractLink" }
                        }
                    },
                    new EntityProperty
                        { Name = "Contract", Types = { new MetadataType { Name = "Entity.ContractLink" } } },
                    new EntityProperty { Name = "Store", Types = { new MetadataType { Name = "Entity.StoreLink" } } },
                }
            };

            var em2 = new EntityMetadata
            {
                Name = "Contract",
                Properties =
                {
                    new EntityProperty { Name = "Name", Types = { new MetadataType { Name = "string", Size = 30 } } },
                }
            };

            var em3 = new EntityMetadata
            {
                Name = "Store",
                Properties =
                {
                    new EntityProperty { Name = "Name", Types = { new MetadataType { Name = "string", Size = 30 } } },
                    new EntityProperty
                        { Name = "Invoice", Types = { new MetadataType { Name = "Entity.InvoiceLink" } } },
                }
            };

            return new EntityMetadataCollection(new[] { em1, em2, em3 }, getTestSecPolicyMetadatas);
        }

        public static string TestSecName = "TestSec";

        private static List<SecPolicyMetadata> getTestSecPolicyMetadatas => new()
        {
            new SecPolicyMetadata
            {
                Name = TestSecName,
                Subjects = new()
                {
                    new() { Name = "Store", Permission = SecPermission.Read }
                },
                Criteria = new()
                {
                    new() { Permission = SecPermission.Read, Query = "FROM Subject s WHERE Name = 'Test'", Subject = "Store" },
                    new() { Permission = SecPermission.Read, Query = "FROM Subject s WHERE Name = 'Hello'", Subject = "Store" }
                }
            }
        };

        public static EntityMetadataCollection GetEmptyMetadata() => new EntityMetadataCollection();
    }
}