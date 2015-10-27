using System;
using System.Windows;
using System.Windows.Data;

namespace Area51.SoftwareModeler.Model
{
    public class PointConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Console.WriteLine("value1: " + values[0] + targetType + ";" + parameter);
            Console.WriteLine("value2: " + values[1]);
            double xValue = (values[0] is double ? (double)values[0] : 0);
            double yValue = (values[1] is double ? (double)values[1] : 0);
            return new Point(xValue, yValue);
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
