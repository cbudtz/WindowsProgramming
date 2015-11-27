using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Area51.SoftwareModeler.Views
{
    /// <summary>
    /// Interaction logic for ClassUserControll.xaml
    /// </summary>
    public partial class ClassUserControl : UserControl
    {
        public ClassUserControl()
        {
            InitializeComponent();
        }

        private void DataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {

        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {

        }
    }
    public class EventTriggerWithoutPropagation : System.Windows.Interactivity.EventTrigger
    {
        protected override void OnEvent(System.EventArgs eventArgs)
        {
            var routedEventArgs = eventArgs as RoutedEventArgs;
            if (routedEventArgs != null)
                routedEventArgs.Handled = true;

            base.OnEvent(eventArgs);
        }
    }
}
