using System;
using System.Collections.Generic;

namespace ZenPlatform.Configuration.Data
{
    public abstract class PObjectType : PTypeBase
    {
        private readonly string _name;
        private readonly List<PProperty> _properties;
        private readonly List<PEvent> _events;

        protected PObjectType(string name)
        {
            _name = name;
            _properties = new List<PProperty>();

            var property = new PProperty(this)
            {
                Unique = true,
                Name = "Id", 
            };
            property.Types.Add(new PGuid());
            _properties.Add(property);

            _events = new List<PEvent>();
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// ���������-��������
        /// </summary>
        public PComponent OwnerComponent { get; set; }

        //TODO: ������� ������ �� ������������ ��������� ��� ������� �������. ������ ����� ������������� �� ��������� �����������

        /// <summary>
        /// ��� ����������� ���.
        /// ���� ��, � ����� ������ �� ����� ������ ����� ������ �������������. �������� ����� ������ ��������� ������
        /// </summary>
        public virtual bool IsAbstractType { get; set; }

        public Guid Id { get; set; }

        public string TableName { get; set; }

        public List<PProperty> Properties
        {
            get { return _properties; }
        }

        public List<PEvent> Events
        {
            get { return _events; }
        }

        public override string Name
        {
            get => _name;
        }


    }
}