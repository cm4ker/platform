using System;
using ZenPlatform.Core;
using ZenPlatform.Core.Annotations;
using ZenPlatform.DocumentComponent;

namespace DefaultNamespace
{
    public static class DocumentComponentSessionExtension
    {
        public static DocumentInterface Document(this Session session)
        {
            return new DocumentInterface(session);
        }
    }
}
