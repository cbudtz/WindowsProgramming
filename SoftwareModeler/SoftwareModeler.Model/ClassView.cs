using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Area51.SoftwareModeler.Models
{
    [XmlInclude(typeof(ClassData))]
    [XmlInclude(typeof(Comment))]
    public abstract class ClassView : NotifyBase
    {

        public int? id { get; set; }
        public static int nextId { get; set; } = 0;

        public Point InitialPosition { get; set; }

        private double x = 200;
  
        //public Thickness Position => new Thickness(X,Y,-X,-Y);
        public Thickness Position => new Thickness(X, Y, Width, Height);

        //public Point PositionPoint { get

        public double X { get { return x; } set { x = value; NotifyPropertyChanged(); NotifyPropertyChanged(() => CanvasCenterX); NotifyPropertyChanged(() => Position); } }

        private double y = 200;
       
        public double Y { get { return y; } set { y = value; NotifyPropertyChanged(); NotifyPropertyChanged(() => CanvasCenterY); NotifyPropertyChanged(() => Position);} }

        private double width = 150;
      
        public double Width { get { return width; } set { width = value; NotifyPropertyChanged(); NotifyPropertyChanged(() => CanvasCenterX); NotifyPropertyChanged(() => CenterX); } }

        private double height = 100;
       
        public double Height { get { return height; } set { height = value; NotifyPropertyChanged(); NotifyPropertyChanged(() => CanvasCenterY); NotifyPropertyChanged(() => CenterY); } }

        // A lambda expression can be given, because the 'NotifyPropertyChanged' method can get the property name from it.
        public double CanvasCenterX { get { return X + Width / 2; } set { X = value - Width / 2; NotifyPropertyChanged(() => X); } }

        // A lambda expression can be given, because the 'NotifyPropertyChanged' method can get the property name from it.
        public double CanvasCenterY { get { return Y + Height / 2; } set { Y = value - Height / 2; NotifyPropertyChanged(() => Y); } }

        
        public double CenterX => Width / 2;

       
        public double CenterY => Height / 2;

       
        private bool isSelected;

        public bool IsSelected { get { return isSelected; } set { isSelected = value; NotifyPropertyChanged(); NotifyPropertyChanged(() => SelectedColor); } }
        // This method uses an expression-bodied member (http://www.informit.com/articles/article.aspx?p=2414582) to simplify a method that only returns a value;
        public Brush SelectedColor => IsSelected ? Brushes.Red : Brushes.Yellow;

   

    }
}
