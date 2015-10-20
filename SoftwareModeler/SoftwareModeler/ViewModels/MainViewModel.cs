using Area51.SoftwareModeler.Models.Commands;
using Area51.SoftwareModeler.Models;
using System.Collections.ObjectModel;
using System.Windows;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Area51.SoftwareModeler.Model;

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
        public bool moveShape = false;
        public Class TestClass { get; set; }
        public string Text { get; set; }

        // Commands that the UI can be bound to.
        // Shapes
        public ICommand MouseDownShapeCommand { get; }
        public ICommand MouseMoveShapeCommand { get; }
        public ICommand MouseUpShapeCommand { get; }
        // connections
        public ICommand MouseDownConnectionCommand { get; }
        public ICommand MouseMoveConnectionCommand { get; }
        public ICommand MouseUpConnectionCommand { get; }
        // Used for saving the shape that a line is drawn from, while it is being drawn.
        private Shape addingLineFrom;
        // Saves the initial point that the mouse has during a move operation.
        private Point initialMousePosition;
        // Saves the initial point that the shape has during a move operation.
        private Point initialShapePosition;


        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>

        //Dynamic 
        public ObservableCollection<Shape> classes { get;  set;}
        public ObservableCollection<Connection> connections { get; set; }
        private CommandTree commandController { get; set; }

        public MainViewModel()
        {
            commandController = new CommandTree();
            // The commands are given the methods they should use to execute, and find out if they can execute.
            MouseDownShapeCommand = new RelayCommand<MouseButtonEventArgs>(MouseDownShape);
            MouseMoveShapeCommand = new RelayCommand<MouseEventArgs>(MouseMoveShape);
            MouseUpShapeCommand = new RelayCommand<MouseButtonEventArgs>(MouseUpShape);

            classes = ShapeCollector.getI().obsShapes;
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

        public void MouseDownShape(MouseButtonEventArgs e)
        {
            var shape = TargetShape(e);
            // The mouse position relative to the target of the mouse event.
            var mousePosition = RelativeMousePosition(e);

            // When the shape is moved with the mouse, the MouseMoveShape method is called many times, 
            //  for each part of the movement.
            // Therefore to only have 1 Undo/Redo command saved for the whole movement, the initial position is saved, 
            //  during the start of the movement, so that it together with the final position, 
            //  from when the mouse is released, can become one Undo/Redo command.
            // The initial shape position is saved to calculate the offset that the shape should be moved.
            initialMousePosition = mousePosition;
            initialShapePosition = new Point(shape.X, shape.Y);

            // The mouse is captured, so the current shape will always be the target of the mouse events, 
            //  even if the mouse is outside the application window.
            e.MouseDevice.Target.CaptureMouse();
        }

        public void MouseMoveShape(MouseEventArgs e)
        {
            if (Mouse.Captured != null)
            {
                // The Shape is gotten from the mouse event.
                var shape = TargetShape(e);
                // The mouse position relative to the target of the mouse event.
                var mousePosition = RelativeMousePosition(e);

                // The Shape is moved by the offset between the original and current mouse position.
                // The View (GUI) is then notified by the Shape, that its properties have changed.
                shape.X = initialShapePosition.X + (mousePosition.X - initialMousePosition.X);
                shape.Y = initialShapePosition.Y + (mousePosition.Y - initialMousePosition.Y);

                // lambda expr. update all connections. first connections where end shape is the moving shape then where start shape is moving shape
                connections.Where(x => x.End.id == shape.id).ToList().ForEach(x => x.End = shape);
                connections.Where(x => x.Start.id == shape.id).ToList().ForEach(x => x.Start = shape);

            }
        }
        public void MouseUpShape(MouseButtonEventArgs e)
        {
            // The Shape is gotten from the mouse event.
            var shape = TargetShape(e);
            // The mouse position relative to the target of the mouse event.
            var mousePosition = RelativeMousePosition(e);

            // The Shape is moved back to its original position, so the offset given to the move command works.
            shape.X = initialShapePosition.X; //TODO uncomment when command works
            shape.Y = initialShapePosition.Y;

            // Now that the Move Shape operation is over, the Shape is moved to the final position, 
            //  by using a MoveNodeCommand to move it.
            // The MoveNodeCommand is given the offset that it should be moved relative to its original position, 
            //  and with respect to the Undo/Redo functionality the Shape has only been moved once, with this Command.
            //TODO fix command
            commandController.addAndExecute(new MoveShapeCommand(shape, mousePosition.X - initialMousePosition.X, mousePosition.Y - initialMousePosition.Y));

            // The mouse is released, as the move operation is done, so it can be used by other controls.
            e.MouseDevice.Target.ReleaseMouseCapture();
        }
        // Gets the shape that was clicked.
        private Shape TargetShape(MouseEventArgs e)
        {
            // Here the visual element that the mouse is captured by is retrieved.
            var shapeVisualElement = (FrameworkElement)e.MouseDevice.Target;
            // From the shapes visual element, the Shape object which is the DataContext is retrieved.
            return (Shape)shapeVisualElement.DataContext;
        }

        // Gets the mouse position relative to the canvas.
        private Point RelativeMousePosition(MouseEventArgs e)
        {
            // Here the visual element that the mouse is captured by is retrieved.
            var shapeVisualElement = (FrameworkElement)e.MouseDevice.Target;
            // The canvas holding the shapes visual element, is found by searching up the tree of visual elements.
            var canvas = FindParentOfType<Canvas>(shapeVisualElement);
            // The mouse position relative to the canvas is gotten here.
            return Mouse.GetPosition(canvas);
        }
        private static T FindParentOfType<T>(DependencyObject o)
        {
            dynamic parent = VisualTreeHelper.GetParent(o);
            return parent.GetType().IsAssignableFrom(typeof(T)) ? parent : FindParentOfType<T>(parent);
        }
    }
}