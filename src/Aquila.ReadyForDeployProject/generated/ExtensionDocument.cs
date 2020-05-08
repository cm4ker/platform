using System;
using Aquila.Core;
using Aquila.Core.Annotations;
using Aquila.DocumentComponent;

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
