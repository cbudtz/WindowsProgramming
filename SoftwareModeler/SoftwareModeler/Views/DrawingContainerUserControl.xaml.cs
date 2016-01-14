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
            //visua
            Console.WriteLine("mouse move");
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
            Console.WriteLine("on drag over");
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            Console.WriteLine("on content changed");
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Console.WriteLine("testcanvas render");
            base.OnRender(drawingContext);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Console.WriteLine("measure override");
            Size availableSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            double minX = 900000; //some dummy high number
            double minY = 900000; //some dummy high number
            double maxX = 0;
            double maxY = 0;

            foreach (UIElement element in this.MainGrid.Children)
            {
                element.Measure(availableSize);

                Rect box = GetDimension(element);
                if (minX > box.X) minX = box.X;
                if (minY > box.Y) minY = box.Y;
                if (maxX < box.X + box.Width) maxX = box.X + box.Width;
                if (maxY < box.Y + box.Height) maxY = box.Y + box.Height;
            }

            if (minX == 900000) minX = 0;
            if (minY == 900000) minY = 0;

            return new Size { Width = maxX - minX, Height = maxY - minY };
        }

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
}
