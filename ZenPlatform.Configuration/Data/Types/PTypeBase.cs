using System;

namespace ZenPlatform.Configuration.Data
{
    public abstract class PTypeBase
    {
        protected PTypeBase()
        {

        }

        /// <summary>
        /// ������������� ����
        /// </summary>
        public abstract Guid Id { get; }
        
        /// <summary>
        /// ������������ ����
        /// </summary>
        public virtual string Name { get; }


        /// <summary>
        /// �������� ����
        /// </summary>
        public virtual string Description { get; set; }
    }
}