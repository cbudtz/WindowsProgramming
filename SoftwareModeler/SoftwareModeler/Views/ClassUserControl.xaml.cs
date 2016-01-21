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

    

        protected override void OnChildDesiredSizeChanged(UIElement child)
        {
            Console.WriteLine("test1");
            base.OnChildDesiredSizeChanged(child);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            Console.WriteLine("test2");
            return base.ArrangeOverride(arrangeBounds);
        }

        //protected override void OnMouseMove(MouseEventArgs e)
        //{
        //    //Console.WriteLine("test3");
        //    Canvas c = VisualParent as Canvas;
        //    //Console.WriteLine("type: " + ));
        //    //this.
        //    if(c != null)c.Width = Width + GridTest.Width;
        //    base.OnMouseMove(e);
        //}

        protected override void OnRender(DrawingContext drawingContext)
        {
            Console.WriteLine("testrender");
            base.OnRender(drawingContext);
            
        }

        //protected override Size MeasureOverride(Size constraint)
        //{
        //    Size availableSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
        //    double minX = 900000; //some dummy high number
        //    double minY = 900000; //some dummy high number
        //    double maxX = 0;
        //    double maxY = 0;
            
        //    foreach (UIElement element in this.GridTest.Children)
        //    {
        //        element.Measure(availableSize);

        //        Rect box = GetDimension(element);
        //        if (minX > box.X) minX = box.X;
        //        if (minY > box.Y) minY = box.Y;
        //        if (maxX < box.X + box.Width) maxX = box.X + box.Width;
        //        if (maxY < box.Y + box.Height) maxY = box.Y + box.Height;
        //    }

        //    if (minX == 900000) minX = 0;
        //    if (minY == 900000) minY = 0;
        //    Console.WriteLine("test");
        //    return new Size { Width = maxX - minX, Height = maxY - minY };
        //}

        public static Rect GetDimension(UIElement element)
        {
            Rect box = new Rect();
            box.X = Canvas.GetLeft(element);
            box.Y = Canvas.GetTop(element);
            box.Width = element.DesiredSize.Width;
            box.Height = element.DesiredSize.Height;
            return box;
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
