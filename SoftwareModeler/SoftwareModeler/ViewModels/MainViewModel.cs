using Area51.SoftwareModeler.Models.Commands;
using Area51.SoftwareModeler.Models;
using System.Collections.ObjectModel;
using System.Windows;
using GalaSoft.MvvmLight;
using System;


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
        public ObservableCollection<Connection> connections { get; set; }
        CommandTree commandController;

        public MainViewModel()
        {
            classes = new ObservableCollection<Class>();
            Class TestClass1 = new Class("A Class", "", false, new Point(0,0), Models.Visibility.Default);
            TestClass1.addAttribute("int", "something");
            TestClass1.addAttribute("String", "someAttribute");
            string[] parameters = { "string", "Int", "Bool" };
            TestClass1.addMethod(Models.Visibility.Public, "somemethod", parameters);
            classes.Add(TestClass1);
            Class TestClass2 = new Class("Another Class", "", false, new Point(300,320), Models.Visibility.Default);
            TestClass2.addAttribute("int", "nothing");
            TestClass2.addAttribute("bool", "True");
            classes.Add(TestClass2);
            connections = new ObservableCollection<Connection>();
            Connection conn = new Connection(TestClass1, "asd", TestClass2, "efg", ConnectionType.Aggregation);
            connections.Add(conn);
            Console.WriteLine(TestClass1.CanvasCenterX + "," + TestClass1.CanvasCenterY);
            Console.WriteLine(TestClass2.CanvasCenterX + "," + TestClass2.CanvasCenterY);
        }
        public string Text { get; set; }
    }
}