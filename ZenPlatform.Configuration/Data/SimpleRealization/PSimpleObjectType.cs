using System;
using Newtonsoft.Json;

namespace ZenPlatform.Configuration.Data.SimpleRealization
{
    /// <summary>
    /// ������� ������ � ������. 
    /// </summary>
    public class PSimpleObjectType : PObjectType
    {
        public PSimpleObjectType(string name, Guid id, PComponent owner) : base(name, id, owner)
        {
            Init();
        }

        public PSimpleObjectType(string name, PComponent owner) : base(name, Guid.Empty, owner)
        {
            Init();
        }

        private void Init()
        {
            var property = new PSimpleProperty(this)
            {
                Unique = true,
                Name = "Id"
            };

            property.Types.Add(new PGuid());
            Properties.Add(property);
        }

        public override bool IsAbstractType => false;

        /// <summary>
        /// ��� ������� � SQL ��� ����� �������
        /// </summary>
        public string TableName { get; set; }
    }
}