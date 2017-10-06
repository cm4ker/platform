using System;

namespace SqlPlusDbSync.Platform
{
    internal class ObjectNotfound : Exception
    {
        public ObjectNotfound(string objectName) : base($"not found object names {objectName} below")
        {

        }
    }
}