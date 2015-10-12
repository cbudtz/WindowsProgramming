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
        public ObservableCollection<Class> classes { get;  set;}
        ObservableCollection<Connection> connections;
        CommandTree commandController;

        public MainViewModel()
        {
            classes = new ObservableCollection<Class>() { new Class("321", "", false, new Point(), Models.Visibility.Default)};
            TestClass = new Class("A Class", "", false, new Point(), Models.Visibility.Default);
            TestClass.addAttribute("int", "something");
            TestClass.addAttribute("String", "someAttribute");
            classes.Add(TestClass);
            TestClass = new Class("Another Class", "", false, new Point(), Models.Visibility.Default);
            TestClass.addAttribute("int", "nothing");
            TestClass.addAttribute("bool", "True");
            classes.Add(TestClass);
        }
        public string Text { get; set; }
    }
}