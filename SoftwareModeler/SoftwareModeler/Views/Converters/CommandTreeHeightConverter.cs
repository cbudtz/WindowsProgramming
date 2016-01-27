using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Area51.SoftwareModeler.Models;
using Area51.SoftwareModeler.Models.Commands;

namespace Area51.SoftwareModeler.Views.Converters
{
    class CommandTreeHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double height = 0;
            foreach (var c in ShapeCollector.GetI().Commands)
            {
                if (!c.IsCollapsed) height += 30;
            }
            return height;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double height = 0;
            foreach (var c in ShapeCollector.GetI().Commands)
            {
                if (!c.IsCollapsed) height += 30;
            }
            return height;
        }
    }
}
