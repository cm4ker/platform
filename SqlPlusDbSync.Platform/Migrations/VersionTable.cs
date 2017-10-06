using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator.VersionTableInfo;

namespace SqlPlusDbSync.Platform.Migrations
{
    [VersionTableMetaData]
    public class VersionTable : DefaultVersionTableMetaData
    {
        public override string TableName
        {
            get { return "__Migrations"; }
        }
    }
}
