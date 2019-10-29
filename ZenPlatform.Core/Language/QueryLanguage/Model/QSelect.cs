using System.Collections.Generic;

namespace ZenPlatform.Core.Language.QueryLanguage.Model
{
    public class QSelect
    {
        public List<QField> Fields { get; }

        public QSelect(List<QField> fields)
        {
            Fields = fields;
        }
    }
}