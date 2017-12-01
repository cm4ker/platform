using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ZenPlatform.UIGeneration2 {
    [Serializable]
    public abstract class ObservableObject : INotifyPropertyChanged {

        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] String propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}