using System.Collections.Generic;

namespace ZenPlatform.Core.Quering.Model
{
    public class QSelect : QItem
    {
        public List<QField> Fields { get; }

        public QSelect(List<QField> fields)
        {
            Fields = fields;

            foreach (var field in fields)
            {
                field.Parent = this;
            }
        }
    }
}