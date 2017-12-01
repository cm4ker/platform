using System.Collections.Generic;
using System.Linq;

namespace ZenPlatform.UIGeneration2 {
    public class ClassEntities : List<ClassEntity> {

        public ClassEntity SelectedItem => (from c in this where c.IsSelected select c).SingleOrDefault();

        public ClassEntities() {
        }

    }
}
