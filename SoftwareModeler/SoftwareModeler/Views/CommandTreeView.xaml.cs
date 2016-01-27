using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Area51.SoftwareModeler.Models;
using Area51.SoftwareModeler.Models.Commands;

namespace Area51.SoftwareModeler.Views
{
    /// <summary>
    /// Interaction logic for CommandTreeView.xaml
    /// </summary>
    public partial class CommandTreeView : UserControl
    {
        private Cursor _cursor;

        public CommandTreeView()
        {
            InitializeComponent();
        }

        private void OnResizeThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            _cursor = Cursor;
            Cursor = Cursors.SizeNWSE;
        }

        private void OnResizeThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            Cursor = _cursor;
        }

        private void OnResizeThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            //double yAdjust = sizableContent.Height + e.VerticalChange;
            double xAdjust = Width - e.HorizontalChange;

            //make sure not to resize to negative width or heigth            
            xAdjust = (ActualWidth + xAdjust) > MinWidth ? xAdjust : MinWidth;

            Width = xAdjust > MaxWidth ? MaxWidth : xAdjust;
        }

        public void AutoScrollAS()
        {
            Console.WriteLine("testing auto scrols");
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            //IEnumerator<BaseCommand> commands = CommandsItems.ItemsSource?.OfType<BaseCommand>().GetEnumerator();
            //double curWidth = CommandScroll.ActualWidth;
            //double curHeight = CommandScroll.ActualHeight;
            //if (commands == null) return;
            //while (commands.MoveNext())
            //{
            //    BaseCommand cur = commands.Current;
            //    if (cur.BranchLayer*30+50 > curWidth) curWidth = cur.BranchLayer*30+50;
            //    if (cur.Id*35 > curHeight) curHeight = cur.Id*35;
            //}

            //if (curWidth >= CommandScroll.ActualWidth) CommandCanvas.Width = curWidth;
            //if (curHeight >= CommandScroll.ActualHeight) CommandCanvas.Height = curHeight;

            //base.OnMouseEnter(e);
        }

        private void CommandCanvas_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            CommandScroll.ScrollToBottom();
            CommandScroll.ScrollToRightEnd();
            BaseCommand active = ShapeCollector.GetI().Commands.First(x => x.IsActive);
            double width = active.BranchLayer*30;
            Console.WriteLine("test: " + width + ";" + CommandScroll.ExtentWidth);
            CommandScroll.ScrollToLeftEnd();
            
            //CommandScroll.
            CommandScroll.ScrollToHorizontalOffset(width - CommandScroll.ActualWidth / 2);
        }
    }
}
