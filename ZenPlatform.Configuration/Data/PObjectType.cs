using System;
using System.Collections.Generic;

namespace ZenPlatform.Configuration.Data
{
    public abstract class PObjectType : PTypeBase
    {
        private readonly string _name;
        private readonly List<PProperty> _propertyes;
        private readonly List<PEvent> _events;

        protected PObjectType(string name)
        {
            _name = name;
            _propertyes = new List<PProperty>();
            _events = new List<PEvent>();
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Это абстрактный тип.
        /// Если да, в таком случае от этого класса можно только наследоваться. Экзепляр этого класса создавать нельзя
        /// </summary>
        public bool IsAbstractType { get; set; }

        public Guid Id { get; set; }

        public string TableName { get; set; }


        public List<PProperty> Propertyes
        {
            get { return _propertyes; }
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