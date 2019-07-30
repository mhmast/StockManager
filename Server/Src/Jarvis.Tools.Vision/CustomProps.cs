using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace Jarvis.Tools.Vision
{
    class CustomProps : UserControl
    {

        public event Action<object> SelectedPropertyChanged;
        private PropertyGrid PropertyGrid = new PropertyGrid
        {
            ShowSearchBox = false,
            AutoGenerateProperties = true,
        };
        public CustomProps()
        {
            PropertyGrid.SelectedPropertyItemChanged += PropertyGrid_SelectedPropertyItemChanged;
            Content = PropertyGrid;
        }

        private void PropertyGrid_SelectedPropertyItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<PropertyItemBase> e)
        {
            var item = ((PropertyItem)e.NewValue);
            SelectedPropertyChanged?.Invoke(item.Value??item.Instance);
        }

        public object SelectedObject
        {
            get => PropertyGrid.SelectedObject;
            set => PropertyGrid.SelectedObject = value;
        }
    }
}
