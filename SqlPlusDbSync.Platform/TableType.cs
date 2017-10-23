//using System;
//using System.Collections.Generic;
//using SqlPlusDbSync.Configuration.Configuration;
//using SqlPlusDbSync.Platform.Configuration;

//namespace SqlPlusDbSync.Platform
//{
//    public class TableType : PType
//    {
//        public TableType()
//        {

//        }

//        public STable Table { get; set; }

//        public override List<SRelation> Relations
//        {
//            get { throw new NotSupportedException(); }
//        }

//        public SWhereCondition WhereCondition { get; set; }

//        public override List<PProperty> Fields
//        {
//            get { return Table.Fields; }
//        }
//    }
//}