using System.Collections.Generic;

namespace ZenPlatform.Core.Language.QueryLanguage.Model
{
    public class QDataRequest : QItem
    {
        public QDataRequest(List<QField> source)
        {
            Source = source;
        }

        public List<QField> Source { get; }
    }
}