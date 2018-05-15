using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ZenPlatform.Configuration.Data.Types.Complex
{
    /// <summary>
    /// ���� (��������) ������� ������������.
    /// �������� � ���� ����������, ����������� � ���� ������. 
    /// 1) ������������ ������� ��������� ������ � ��������
    /// 2) ������������ ���� � ������ DTO (���������/aliase)
    /// </summary>
    public abstract class PProperty
    {
        private readonly PObjectType _owner;

        public PProperty(PObjectType owner)
        {
            _owner = owner;
            // _owner.Propertyes.Add(this);
            Types = new List<PTypeBase>();
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// ������������� ��������
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ������������ ������� � ���� ������
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ������� ������������
        /// ����������, ����� �� �� ����� ������� ���������� ��������
        /// </summary>
        public bool Unique { get; set; }

        /*
        /// <summary>
        /// ��������� �� ��, ��� ������������ ����������� ����.
        /// ���� �� ������������� ��� ��������, � ����� ������ ��� ���������� ������� 
        /// </summary>
        /// public bool CompositeUnique { get; set; }
        */

        /// <summary>
        /// ���������, ���� ���������� ��� ��������, �� ��� ����������� Name
        /// 
        /// ������������ ��� ��������� � ����. �.�. ����� ������������ ��������� ��������������
        /// �������� AliasNamed ����� �������� Column("Name") �������������, ��� ���������� ������� ����� ��������� ��������� � �������
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// ��� ��������� ������
        /// ��� ����� ���� �����������, ������� ��� ������������ ������ �����
        /// </summary>
        public List<PTypeBase> Types { get; set; }


        /// <summary>
        /// �������� ��������.
        /// �� ����� ����� ��������� ������ ������. ���� �������� ����������� ������-�� �������, �� ��� ���� � ��� �� ������� ��� �����������
        /// </summary>
        [JsonIgnore]
        public PObjectType Owner => _owner;
    }

}