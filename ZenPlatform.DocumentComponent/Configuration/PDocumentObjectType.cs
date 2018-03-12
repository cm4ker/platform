using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.Data;
using ZenPlatform.Configuration.Data.SimpleRealization;
using ZenPlatform.DataComponent.Configuration;

namespace ZenPlatform.DocumentComponent
{
    public class PDocumentObjectType : PDataObjectType
    {
        public PDocumentObjectType(string name, Guid id, PComponent owner) : base(name, id, owner)
        {

        }

        public string RelTableName { get; }

    }
}
