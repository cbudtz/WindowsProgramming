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
using Area51.SoftwareModeler.Models;

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
        private bool moveShape = false;
        private bool isAddingShape = false;
        private bool isResizing = false;
        double initialWidth = 0;

        private Class newShape = null;
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

        public ICommand NewClassCommand { get; }
        public ICommand NewAbstractCommand { get; }
        public ICommand NewInterfaceCommand { get; }
        public ICommand MouseDownShapeResizeCommand { get; }
        public ICommand MouseUpShapeResizeCommand { get; }

        public ICommand MouseClickCommand { get; }

        // Used for saving the classRep that a line is drawn from, while it is being drawn.
        private Shape addingLineFrom;
        // Saves the initial point that the mouse has during a move operation.
        private Point initialMousePosition;
        // Saves the initial point that the classRep has during a move operation.
        private Point initialShapePosition;



        //view access to observables
        public ObservableCollection<Shape> classes { get { return ShapeCollector.getI().obsShapes; } }
        public ObservableCollection<Connection> connections { get { return ShapeCollector.getI().obsConnections; } }

        //Dynamic 
        private CommandTree commandController { get; set; }
        private ShapeCollector observables = ShapeCollector.getI();

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            commandController = new CommandTree();
            // The commands are given the methods they should use to execute, and find out if they can execute.
            MouseDownShapeCommand = new RelayCommand<MouseButtonEventArgs>(MouseDownShape);
            MouseMoveShapeCommand = new RelayCommand<MouseEventArgs>(MouseMoveShape);
            MouseUpShapeCommand = new RelayCommand<MouseButtonEventArgs>(MouseUpShape);

            MouseDownShapeResizeCommand = new RelayCommand<MouseButtonEventArgs>(MouseDownResizeShape);
            MouseUpShapeResizeCommand = new RelayCommand<MouseButtonEventArgs>(MouseUpResizeShape);

            MouseClickCommand = new RelayCommand<MouseEventArgs>(MouseClicked);

            NewClassCommand = new RelayCommand(AddClass);
            NewInterfaceCommand = new RelayCommand(AddInterface);
            NewAbstractCommand = new RelayCommand(AddAbstract);

            Class TestClass1 = new Class("A Class", "", false, new Point(0, 0), Models.Visibility.Default);
            TestClass1.addAttribute("int", "something");
            TestClass1.addAttribute("String", "someAttribute");
            string[] parameters = { "string", "Int", "Bool" };
            TestClass1.addMethod(Models.Visibility.Public, "somemethod", parameters);
            observables.obsShapes.Add(TestClass1);
            Class TestClass2 = new Class("Another Class", "", false, new Point(300, 320), Models.Visibility.Default);
            TestClass2.addAttribute("int", "nothing");
            TestClass2.addAttribute("bool", "True");
            observables.obsShapes.Add(TestClass2);

            Connection conn = new Connection(TestClass1, "asd", TestClass2, "efg", ConnectionType.Aggregation);
            //observables.obsConnections.Add(conn);
            Console.WriteLine(TestClass1.CanvasCenterX + "," + TestClass1.CanvasCenterY);
            Console.WriteLine(TestClass2.CanvasCenterX + "," + TestClass2.CanvasCenterY);
        }

        public void MouseDownShape(MouseButtonEventArgs e)
        {
            var shape = TargetShape(e);
            // The mouse position relative to the target of the mouse event.
            var mousePosition = RelativeMousePosition(e);

            // When the classRep is moved with the mouse, the MouseMoveShape method is called many times, 
            //  for each part of the movement.
            // Therefore to only have 1 Undo/Redo command saved for the whole movement, the initial position is saved, 
            //  during the start of the movement, so that it together with the final position, 
            //  from when the mouse is released, can become one Undo/Redo command.
            // The initial classRep position is saved to calculate the offset that the classRep should be moved.
            initialMousePosition = mousePosition;
            initialShapePosition = new Point(shape.X, shape.Y);

            // The mouse is captured, so the current classRep will always be the target of the mouse events, 
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

                if (isResizing)
                {
                    Console.WriteLine("resizing...");
                    shape.Width =  mousePosition.X- shape.X;                    
                }
                else
                {
                    
                    // The Shape is moved by the offset between the original and current mouse position.
                    // The View (GUI) is then notified by the Shape, that its properties have changed.
                    shape.X = initialShapePosition.X + (mousePosition.X - initialMousePosition.X);
                    shape.Y = initialShapePosition.Y + (mousePosition.Y - initialMousePosition.Y);

                    // lambda expr. update all connections. first connections where end classRep is the moving classRep then where start classRep is moving classRep
                    observables.obsConnections.Where(x => x.End.id == shape.id).ToList().ForEach(x => x.End = shape);
                    observables.obsConnections.Where(x => x.Start.id == shape.id).ToList().ForEach(x => x.Start = shape);
                }
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
        public void MouseDownResizeShape(MouseButtonEventArgs e)
        {
            var shape = TargetShape(e);
            // The mouse position relative to the target of the mouse event.
            var mousePosition = RelativeMousePosition(e);

            var border = (Border)e.MouseDevice.Target;

            // When the classRep is moved with the mouse, the MouseMoveShape method is called many times, 
            //  for each part of the movement.
            // Therefore to only have 1 Undo/Redo command saved for the whole movement, the initial position is saved, 
            //  during the start of the movement, so that it together with the final position, 
            //  from when the mouse is released, can become one Undo/Redo command.
            // The initial classRep position is saved to calculate the offset that the classRep should be moved.
            initialMousePosition = mousePosition;
            initialShapePosition = new Point(shape.X, shape.Y);

            initialWidth = border.ActualWidth;

            isResizing = true;
            Console.WriteLine("start resize");

            // The mouse is captured, so the current classRep will always be the target of the mouse events, 
            //  even if the mouse is outside the application window.
            e.MouseDevice.Target.CaptureMouse();
        }

        public void MouseUpResizeShape(MouseButtonEventArgs e)
        {
            Console.WriteLine("resized done");
            // The Shape is gotten from the mouse event.
            var shape = TargetShape(e);
            // The mouse position relative to the target of the mouse event.
            var mousePosition = RelativeMousePosition(e);

            // The Shape is moved back to its original position, so the offset given to the move command works.

            //shape.Width = initialMousePosition.X - mousePosition.X;
            //shape.X = initialShapePosition.X; //TODO uncomment when command works
            //shape.Y = initialShapePosition.Y;
            // Now that the Move Shape operation is over, the Shape is moved to the final position, 
            //  by using a MoveNodeCommand to move it.
            // The MoveNodeCommand is given the offset that it should be moved relative to its original position, 
            //  and with respect to the Undo/Redo functionality the Shape has only been moved once, with this Command.
            //TODO fix command

            isResizing = false;
            //commandController.addAndExecute(new MoveShapeCommand(shape, mousePosition.X - initialMousePosition.X, mousePosition.Y - initialMousePosition.Y));

            // The mouse is released, as the move operation is done, so it can be used by other controls.
            e.MouseDevice.Target.ReleaseMouseCapture();
        }

        public void MouseClicked(MouseEventArgs e)
        {
            if (isAddingShape && newShape != null)
            {
                if (e == null) return;
                var mousePosition = RelativeMousePosition(e);
                //Point mousePosition = Mouse.GetPosition(Application.Current.MainWindow);
                newShape.X = mousePosition.X - newShape.Width / 2;
                newShape.Y = mousePosition.Y - newShape.Height / 2;
                commandController.addAndExecute(new AddClassCommand(newShape.name, newShape.StereoType, newShape.IsAbstract, new Point(newShape.X, newShape.Y), newShape.Visibility));

                //classes.Add(newShape); // TODO remove when commancontroller is fixed
            }
            isAddingShape = false;
            newShape = null;
        }

        private void AddClass()
        {
            Class shape = new Class();
            shape.IsAbstract = false;
            shape.StereoType = "";
            shape.Visibility = Models.Visibility.Public;
            AddShape(shape);

        }


        private void AddAbstract()
        {
            Class shape = new Class();
            shape.IsAbstract = true;
            shape.StereoType = "";
            shape.Visibility = Models.Visibility.Public;
            AddShape(shape);
        }

        private void AddInterface()
        {
            Class shape = new Class();
            shape.IsAbstract = false;
            shape.StereoType = "<<interface>>";
            shape.Visibility = Models.Visibility.Public;
            AddShape(shape);
        }

        private void AddShape(Class shape)
        {
            isAddingShape = true;
            newShape = shape;
        }


        // Gets the classRep that was clicked.
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
            if (shapeVisualElement is Canvas)
            {
                return Mouse.GetPosition(shapeVisualElement);

            }
            else
            {
                var canvas = FindParentOfType<Canvas>(shapeVisualElement);
                return Mouse.GetPosition(canvas);
            }
            // The mouse position relative to the canvas is gotten here.

        }
        private static T FindParentOfType<T>(DependencyObject o)
        {
            dynamic parent = VisualTreeHelper.GetParent(o);
            return parent.GetType().IsAssignableFrom(typeof(T)) ? parent : FindParentOfType<T>(parent);
        }
    }
}