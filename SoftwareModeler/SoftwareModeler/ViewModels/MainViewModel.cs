using Area51.SoftwareModeler.Models;
using System.Collections.ObjectModel;
using System.Windows;
using GalaSoft.MvvmLight;


namespace Area51.SoftwareModeler.ViewModels
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
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        
        //Dynamic 
        ObservableCollection<Class> classes;
        ObservableCollection<Connection> connections;
        CommandTree commandController;

        public MainViewModel()
        {
            
            TestClass = new Class("123", "", false, new Point(), Models.Visibility.Default);
        }
        public string Text { get; set; }
    }
}