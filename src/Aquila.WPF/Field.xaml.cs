using System.Windows;
using System.Windows.Controls;

namespace Aquila
{
    public partial class Field : UserControl
    {
        private object _currentObject;

        public Field()
        {
            InitializeComponent();
        }

        public object Value
        {
            set
            {
                _currentObject = value;
                OnItemChanged();
            }
        }

        public int CurrentObjectType { get; set; }


        private void OnItemChanged()
        {
            this.PART_TextBlock.Text = _currentObject.ToString();
        }

        public bool IsTypeButtonVisible
        {
            get => this.PART_TypeButton.Visibility == Visibility.Visible;
            set => this.PART_TypeButton.Visibility = (value) ? Visibility.Visible : Visibility.Hidden;
        }

        public bool IsTypeOpenVisible
        {
            get => this.PART_OpenButton.Visibility == Visibility.Visible;
            set => this.PART_OpenButton.Visibility = (value) ? Visibility.Visible : Visibility.Hidden;
        }

        public bool IsTypeClearVisible
        {
            get => this.PART_ClearButton.Visibility == Visibility.Visible;
            set => this.PART_ClearButton.Visibility = (value) ? Visibility.Visible : Visibility.Hidden;
        }

        public bool IsTypeSelectVisible
        {
            get => this.PART_SelectButton.Visibility == Visibility.Visible;
            set => this.PART_SelectButton.Visibility = (value) ? Visibility.Visible : Visibility.Hidden;
        }
    }
}