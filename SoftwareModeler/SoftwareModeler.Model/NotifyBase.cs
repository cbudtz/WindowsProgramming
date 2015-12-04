using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Area51.SoftwareModeler.Models
{
    public abstract class NotifyBase : INotifyPropertyChanged
    {
        // This is the event that is raised when the INotifyPropertyChanged interface is used to let the View (GUI) know that a property of a bound object has changed.
        public event PropertyChangedEventHandler PropertyChanged;

        // This method is used by inheriting classes to raise the INotifyPropertyChanged event.
        // It must be called in all set methods that change the state of model objects, to be sure that the view (GUI) is always updated, when data is changed behind the scenes.
        // This version of the method takes a lambda expression that has to point to the property that has changed
        protected void NotifyPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            // This uses 'var' which is an implicit Type variable (https://msdn.microsoft.com/en-us/library/bb383973.aspx).
            var propertyName = (propertyExpression?.Body as MemberExpression)?.Member?.Name;
            NotifyPropertyChanged(propertyName);
        }

        // This method is used by inheriting classes to raise the INotifyPropertyChanged event.
        // It must be called in all set methods that change the state of model objects, to be sure that the view (GUI) is always updated, when data is changed behind the scenes.
        // This version of the method takes a string that has to be equivalent to the name of the property that has changed.
        // If no string is given, then the name of the method/property that called this method is used.
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName != null && PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
