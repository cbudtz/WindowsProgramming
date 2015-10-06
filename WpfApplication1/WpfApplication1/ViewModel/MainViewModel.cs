using GalaSoft.MvvmLight;
using ModelLibrary;
using System.Windows;

namespace WpfApplication1.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public Class TestClass { get; set; }
        

        public MainViewModel()
        {
            
            TestClass = new Class("123", "", false, new Point(), ModelLibrary.Visibility.Default);
        }
        public string Text { get; set; }
    }
}