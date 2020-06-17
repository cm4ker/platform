using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Channels;
using Newtonsoft.Json.Linq;

namespace Aquila.Configuration.Common.DbManager
{
    enum RelationType
    {
        //numeric to decimal
        DbToClr,
        ClrToDb
    }

    public class TypeMapper
    {
    }

    public class DbSchemaSystem
    {
        private List<DbComponent> _components;

        public DbSchemaSystem()
        {
            _components = new List<DbComponent>();
        }

        public IReadOnlyList<DbComponent> Components => _components;

        public DbTableBuilder DefineTable()
        {
            return new DbTableBuilder();
        }
    }

    public class DbComponent
    {
    }

    public class DbTable
    {
        public virtual string Name => "Unknown";

        public virtual string Schema => "Unknown";
    }

    public sealed class DbTableBuilder
    {
        public void SetName(string name)
        {
        }

        public void SetSchema(string schema)
        {
        }
    }

    public class DbColumn
    {
        public virtual string Name => "Unknown";

        public void SetName(string name)
        {
        }

        public void SetType(Guid typeId)
        {
        }
    }


    public sealed class DbColumnBuilder
    {
    }

    //===============

    class PlSchema
    {
    }

    class PlComponent
    {
    }

    class PlEntity
    {
    }

    class PlField
    {
    }

    public class PlTable
    {
    }
}