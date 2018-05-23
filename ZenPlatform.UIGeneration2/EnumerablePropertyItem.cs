using System;
using System.Collections.Generic;

namespace ZenPlatform.UIGeneration2 {
    public class EnumerablePropertyItem {

        public IEnumerable<String> ClassPropertyNames { get; }

        public Boolean IsClass { get; }

        public String Name { get; }

        public EnumerablePropertyItem(String name) {
            if (string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            }
            this.Name = name;
            this.IsClass = false;
        }

        public EnumerablePropertyItem(String name, IEnumerable<String> classPropertyNames) {
            if (classPropertyNames == null) {
                throw new ArgumentNullException(nameof(classPropertyNames));
            }
            if (string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            }
            this.Name = name;
            this.IsClass = true;
            this.ClassPropertyNames = classPropertyNames;
        }

        public override String ToString() {
            return this.Name;
        }

    }
}
