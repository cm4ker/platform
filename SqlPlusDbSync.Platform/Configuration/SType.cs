using System;
using System.Collections.Generic;

namespace SqlPlusDbSync.Platform.Configuration
{
    public abstract class SType : STypeBase
    {
        private readonly List<SEvent> _events;
        private readonly List<SRelation> _relations;
        private readonly List<SField> _fields;

        protected SType()
        {
            _events = new List<SEvent>();
            _relations = new List<SRelation>();
            _fields = new List<SField>();
        }

        public virtual List<SEvent> Events
        {
            get { return _events; }
        }

        public virtual List<SField> Fields
        {
            get { return _fields; }
        }



        public virtual SCondition Condition { get; set; }

        public virtual List<SRelation> Relations
        {
            get { return _relations; }
        }

        public virtual List<SField> GetFields()
        {
            return _fields;
        }

        public virtual bool IsTransfered { get; set; }
        public virtual SType Source { get; set; }

        public virtual SField GetIdentity()
        {
            return Fields.Find(x => x.IsIdentifier);
        }

        public virtual TableType GetTableObject()
        {
            if (this is TableType) return this as TableType;
            else return this.Source.GetTableObject();
        }

        public virtual string GetFullName()
        {
            return GetFullName(this).TrimStart('.');
        }

        private string GetFullName(SType obj)
        {
            if (obj is TableType) return $"{(obj as TableType).Name}.{(obj as TableType).Table.Name}";
            return $"{obj.Name}.{GetFullName(Source)}";
        }
    }


    public abstract class SComplexType : SObjectType
    {
        private readonly SObjectType _objectType;
        private readonly List<SEvent> _events;

        protected SComplexType(SObjectType objectType)
        {
            _objectType = objectType;
            _events = new List<SEvent>();
            Propertyes.AddRange(_objectType.Propertyes);
        }

        public virtual List<SEvent> Events
        {
            get { return _events; }
        }

    }

    public abstract class SDTOType : SObjectType
    {
        public virtual SCondition TableSplitter { get; set; }
    }

    public abstract class SObjectType : STypeBase
    {
        private readonly List<SField> _propertyes;

        public SObjectType()
        {
            _propertyes = new List<SField>();
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public virtual List<SField> Propertyes
        {
            get { return _propertyes; }
        }
    }

    public abstract class STypeBase
    {
        protected STypeBase()
        {

        }

        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public abstract bool IsComplexType { get; }
    }

    public class SNumeric : STypeBase
    {
        public override bool IsComplexType
        {
            get { return false; }
        }
    }

    public class SBoolean : STypeBase
    {
        public override bool IsComplexType
        {
            get { return false; }
        }
    }

    public class SDateTime : STypeBase
    {
        public override bool IsComplexType
        {
            get { return false; }
        }
    }

    public class SText : STypeBase
    {
        public override bool IsComplexType
        {
            get { return false; }
        }
    }
}