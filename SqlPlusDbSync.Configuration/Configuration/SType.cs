using System;
using System.Collections.Generic;
using System.Data;
using SqlPlusDbSync.Platform.Configuration;

namespace SqlPlusDbSync.Configuration.Configuration
{
    public class PType : PTypeBase
    {
        private readonly List<SEvent> _events;
        private readonly List<SField> _fields;

        protected PType()
        {
            _events = new List<SEvent>();
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

        public virtual List<SField> GetFields()
        {
            return _fields;
        }

        public virtual bool IsTransfered { get; set; }

        public virtual PType Source { get; set; }

        public virtual SField GetIdentity()
        {
            return Fields.Find(x => x.IsIdentifier);
        }

    }

    public abstract class PComplexType : PObjectType
    {
        private readonly PObjectType _objectType;
        private readonly List<SEvent> _events;

        protected PComplexType(PObjectType objectType)
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

    public abstract class SDTOType : PObjectType
    {
        public virtual SCondition TableSplitter { get; set; }
    }

    public abstract class PObjectType : PTypeBase
    {
        private readonly List<SField> _propertyes;

        public PObjectType()
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

    public abstract class PTypeBase
    {
        protected PTypeBase()
        {

        }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public abstract bool IsNullable { get; set; }

        public abstract int ColumnSize { get; set; }

        public abstract int Precision { get; set; }

        public abstract int Scale { get; set; }

        public abstract bool IsComplexType { get; }


        public abstract SqlDbType Type { get; set; }
    }

    public class PNumeric : PTypeBase
    {
        public override bool IsNullable { get; set; }
        public override int ColumnSize { get; set; }
        public override SqlDbType Type { get; set; }
        public override int Precision { get; set; }
        public override int Scale { get; set; }

        public override bool IsComplexType
        {
            get { return false; }
        }
    }

    public class PBoolean : PTypeBase
    {
        public override bool IsNullable { get; set; }
        public override int ColumnSize { get; set; }
        public override SqlDbType Type { get; set; }
        public override int Precision { get; set; }
        public override int Scale { get; set; }

        public override bool IsComplexType
        {
            get { return false; }
        }
    }

    public class PDateTime : PTypeBase
    {
        public override bool IsNullable { get; set; }
        public override int ColumnSize { get; set; }
        public override SqlDbType Type { get; set; }
        public override int Precision { get; set; }
        public override int Scale { get; set; }

        public override bool IsComplexType
        {
            get { return false; }
        }
    }

    public class PText : PTypeBase
    {
        public override bool IsNullable { get; set; }
        public override int ColumnSize { get; set; }

        public override int Precision { get; set; }
        public override int Scale { get; set; }

        public override SqlDbType Type { get; set; }


        public override bool IsComplexType
        {
            get { return false; }
        }
    }
}