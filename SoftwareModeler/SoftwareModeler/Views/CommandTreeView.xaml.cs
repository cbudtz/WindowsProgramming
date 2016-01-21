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
using Area51.SoftwareModeler.Models.Commands;

namespace Area51.SoftwareModeler.Views
{
    /// <summary>
    /// Interaction logic for CommandTreeView.xaml
    /// </summary>
    public partial class CommandTreeView : UserControl
    {
        public CommandTreeView()
        {
            InitializeComponent();
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            IEnumerator<BaseCommand> commands = CommandsItems.ItemsSource?.OfType<BaseCommand>().GetEnumerator();
            double curWidth = CommandScroll.ActualWidth;
            double curHeight = CommandScroll.ActualHeight;
            if (commands == null) return;
            while (commands.MoveNext())
            {
                BaseCommand cur = commands.Current;
                if (cur.BranchLayer*30+50 > curWidth) curWidth = cur.BranchLayer*30+50;
                if (cur.Id*35 > curHeight) curHeight = cur.Id*35;
            }

            if (curWidth >= CommandScroll.ActualWidth) CommandCanvas.Width = curWidth;
            if (curHeight >= CommandScroll.ActualHeight) CommandCanvas.Height = curHeight;

            base.OnMouseEnter(e);
        }
    }
}
