using System;
using System.Collections.Generic;

namespace ZenPlatform.Configuration.Data.Types.Complex
{
    public abstract class PObjectType : PTypeBase
    {
        protected PObjectType(string name, Guid id, PComponent component)
        {
            Name = name;
            Id = (id == Guid.Empty) ? Guid.NewGuid() : id;
            Properties = new List<PProperty>();

            OwnerComponent = component;
        }

        /// <summary>
        /// ���������-��������
        /// </summary>
        public PComponent OwnerComponent { get; }

        //TODO: ������� ������ �� ������������ ��������� ��� ������� �������. ������ ����� ������������� �� ��������� �����������

        /// <summary>
        /// ��� ����������� ���.
        /// ���� ��, � ����� ������ �� ����� ������ ����� ������ �������������. �������� ����� ������ ��������� ������
        /// </summary>
        public virtual bool IsAbstractType { get; set; }

        public override Guid Id { get; }

        //public string TableName { get; set; }

        public List<PProperty> Properties { get; }

        public override string Name { get; }

    }
}