using System.Collections.Generic;

namespace Aquila.Metadata
{
    public class TestMetadata
    {
        public static string DefaultConnetionString =
            "Data Source=.;Initial Catalog=TestDb;Integrated Security=true;TrustServerCertificate=True";

        public static MetadataProvider GetTestMetadata()
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
                },
                Tables =
                {
                    new EntityTable
                    {
                        Name = "Nomenclatures",
                        Properties =
                        {
                            new EntityProperty
                            {
                                Name = "NomenclatureName", Types = { new MetadataType { Name = "string", Size = 100 } }
                            }
                        }
                    },
                    new EntityTable
                    {
                        Name = "Contracts",
                        Properties =
                        {
                            new EntityProperty
                            {
                                Name = "ContractName", Types = { new MetadataType { Name = "string", Size = 100 } }
                            }
                        }
                    }
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

            return new MetadataProvider(new[] { em1, em2, em3 }, getTestSecPolicyMetadatas);
        }

        public static string TestSecName = "TestSec";

        private static List<SecPolicyMetadata> getTestSecPolicyMetadatas => new()
        {
            new SecPolicyMetadata
            {
                Name = TestSecName,
                Subjects = new List<SecPolicySubjectMetadata>
                {
                    new(name: "Store", permission: SecPermission.Read),
                    new(name: "Invoice", permission: SecPermission.Read | SecPermission.Create | SecPermission.Update |
                                                     SecPermission.Delete)
                },
                Criteria = new List<SecPolicyCriterionMetadata>
                {
                    new(permission: SecPermission.Read, query: "FROM Subject s WHERE Name = 'Test'", subject: "Store"),
                    new(permission: SecPermission.Read, query: "FROM Subject s WHERE Name = 'Hello'", subject: "Store"),
                    new(permission: SecPermission.Create, query: "FROM Subject s WHERE Name = 'CreateMe'",
                        subject: "Invoice"),
                    new(permission: SecPermission.Update, query: "FROM Subject s WHERE Name = 'UpdateMe'",
                        subject: "Invoice"),
                    new(permission: SecPermission.Delete, query: "FROM Subject s WHERE Name = 'DeleteMe'",
                        subject: "Invoice")
                }
            }
        };

        public static MetadataProvider GetEmptyMetadata() => new MetadataProvider();
    }
}