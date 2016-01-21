using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Area51.SoftwareModeler.Models;

namespace Area51.SoftwareModeler.Views
{
    /// <summary>
    /// Interaction logic for DiagramDrawingWindowUserControl.xaml
    /// </summary>
    public partial class DrawingContainerUserControl : UserControl
    {
        public DrawingContainerUserControl()
        {
            InitializeComponent();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton != MouseButtonState.Pressed) return;
            
            IEnumerator<ClassData> classes  = ItemsClass.ItemsSource?.OfType<ClassData>().GetEnumerator();
            double curWidth = MainViewScroll.ActualWidth;
            double curHeight = MainViewScroll.ActualHeight; 
            if (classes == null) return;
            while (classes.MoveNext())
            {
                ClassData cur = classes.Current;
                if (cur.Width + cur.X > curWidth) curWidth = cur.Width + cur.X;
                if (cur.Height + cur.Y > curHeight) curHeight = cur.Height + cur.Y;
            }

            if(curWidth >= MainViewScroll.ActualWidth) MainCanvas.Width = curWidth + 20;
            if(curHeight >= MainViewScroll.ActualHeight) MainCanvas.Height = curHeight + 20;
        }


    }
}
