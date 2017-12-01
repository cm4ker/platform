using System;
using System.ComponentModel;
using ZenPlatform.UIGeneration2.Infrastructure;

namespace ZenPlatform.UIGeneration2 {
    [Serializable]
    public class CreateObjectDefinition {

        public CreateObject CreateObject { get; }

        public ProjectType ProjectType { get; }

        public String ShortName { get; }

        public CreateObjectDefinition(CreateObject createObject) {
            this.CreateObject = createObject;
            if (!Enum.IsDefined(typeof(CreateObject), createObject)) {
                throw new InvalidEnumArgumentException(nameof(createObject), (Int32)createObject, typeof(CreateObject));
            }

            var value = createObject.ToString();

            if (value.StartsWith(Constants.Wpf)) {
                this.ProjectType = ProjectType.Wpf;
                this.ShortName = value.Substring(3).SplitWords();
            } else if (value.StartsWith(Constants.Xamarin)) {
                this.ProjectType = ProjectType.Xamarin;
                this.ShortName = value.Substring(7).SplitWords();
            } else if (value.StartsWith(Constants.Silverlight)) {
                this.ProjectType = ProjectType.Silverlight;
                this.ShortName = value.Substring(11).SplitWords();
            } else {
                this.ProjectType = ProjectType.Uwp;
                this.ShortName = value.Substring(3).SplitWords();
            }
        }

        public override String ToString() {
            return this.ShortName;
        }

    }
}
