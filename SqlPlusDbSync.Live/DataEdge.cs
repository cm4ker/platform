using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GraphX.PCL.Common.Models;
using SqlPlusDbSync.Live.Annotations;

namespace SqlPlusDbSync.Live
{
    public class DataEdge : EdgeBase<DataVertex>, INotifyPropertyChanged
    {
        private bool _isLive;
        private int _thick;
        private string _pointFrom;
        private string _pointTo;

        public DataEdge(DataVertex source, DataVertex target, double weight = 1)
            : base(source, target, weight)
        {
        }

        public DataEdge()
            : base(null, null, 1)
        {
        }

        public string PointFrom
        {
            get { return _pointFrom; }
            set
            {
                _pointFrom = value;
                OnPropertyChanged();
            }
        }

        public string PointTo
        {
            get { return _pointTo; }
            set
            {
                _pointTo = value;
                OnPropertyChanged();
            }
        }

        public Guid MessageId { get; set; }

        public bool IsLive
        {
            get { return _isLive; }
            set
            {
                _isLive = value;
                OnPropertyChanged();
            }
        }


        public override string ToString()
        {
            return $"{PointFrom}";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}