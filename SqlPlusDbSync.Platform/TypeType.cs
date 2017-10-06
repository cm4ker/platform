using System;
using SqlPlusDbSync.Platform.Configuration;

namespace SqlPlusDbSync.Platform
{
    public class TypeType : SType
    {
        public TypeType()
        {

        }

        public override SCondition Condition
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }


    }
}