using System.Collections.Generic;

namespace ZenPlatform.Configuration.Data
{

    /// <summary>
    /// ���� (��������) ������� ������������.
    /// �������� � ���� ����������, ����������� � ���� ������. 
    /// 1) ������������ ������� ��������� ������ � ��������
    /// 2) ������������ ���� � ������ DTO (���������/aliase)
    /// </summary>
    public class PProperty
    {
        private readonly PObjectType _owner;

        public PProperty(PObjectType owner)
        {
            _owner = owner;
            // _owner.Propertyes.Add(this);
            Types = new List<PTypeBase>();
        }

        /// <summary>
        /// ������������ ������� � ���� ������
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ������� ������������
        /// ����������, ����� �� �� ����� ������� ���������� ��������
        /// </summary>
        public bool Unique { get; set; }

        /// <summary>
        /// ��������� �� ��, ��� ������������ ����������� ����.
        /// ���� �� ������������� ��� ��������, � ����� ������ ��� ���������� ������� 
        /// </summary>
        public bool CompositeUnique { get; set; }

        /// <summary>
        /// ���������, ���� ���������� ��� ��������, �� ��� ����������� Name
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
        public PObjectType Owner => _owner;
    }
}