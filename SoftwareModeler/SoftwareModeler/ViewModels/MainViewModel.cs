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
        private bool isAddingAssociation = false;
        private bool isAddingAggregation = false;
        private bool isAddingComposition = false;
        private bool isAddingClass = false;
        private bool isAddingAbstract = false;
        private bool isAddingInterface = false;

        double initialWidth = 0;
        private double minShapeWidth = 150;
        private double minShapeHeight = 100;
        
        private Connection newConnection = null;

        public string Text { get; set; }

        // Commands that the UI can be bound to.
        // Shapes
        public ICommand MouseDownShapeCommand { get; }
        public ICommand MouseMoveShapeCommand { get; }
        public ICommand MouseUpShapeCommand { get; }

        public ICommand MouseDownShapeResizeCommand { get; }
        public ICommand MouseUpShapeResizeCommand { get; }
        // connections
        public ICommand MouseDownConnectionCommand { get; }
        public ICommand MouseMoveConnectionCommand { get; }
        public ICommand MouseUpConnectionCommand { get; }

        public ICommand KeyDownCommand { get; }

        // toolbox
        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }

        public ICommand AddAssociationCommand { get; }
        public ICommand AddAggregationCommand { get; }
        public ICommand AddCompositionCommand { get; }

        public ICommand NewClassCommand { get; }
        public ICommand NewAbstractCommand { get; }
        public ICommand NewInterfaceCommand { get; }
        public ICommand NewCommentCommand { get; }
 
        public ICommand MouseClickCommand { get; }
        
        // Used for saving the classRep that a line is drawn from, while it is being drawn.
        private Shape addingLineFrom;
        // Saves the initial point that the mouse has during a move operation.
        private Point initialMousePosition;
        // Saves the initial point that the classRep has during a move operation.
        private Point initialShapePosition;

        //view access to observables
        public ObservableCollection<BaseCommand> commands { get { return commandController.commands; } }
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

            SaveCommand = new RelayCommand(saveFile);
            LoadCommand = new RelayCommand(loadFile);

            AddAggregationCommand = new RelayCommand(AddAggregation);
            AddAssociationCommand = new RelayCommand(AddAssociation);
            AddCompositionCommand = new RelayCommand(AddComposition);

            NewClassCommand = new RelayCommand(AddClass);
            NewInterfaceCommand = new RelayCommand(AddInterface);
            NewAbstractCommand = new RelayCommand(AddAbstract);

            AddCompositionCommand = new RelayCommand(AddComposition);

            KeyDownCommand = new RelayCommand<KeyEventArgs>(KeyDown);

            // TODO remove
            test();
        }

        public void KeyDown(KeyEventArgs e)
        {
            KeyStates ctrl = e.KeyStates & e.KeyboardDevice.GetKeyStates(Key.LeftCtrl) & KeyStates.Down;
            KeyStates z = e.KeyStates & e.KeyboardDevice.GetKeyStates(Key.Z) & KeyStates.Down; 
            KeyStates y = e.KeyStates & e.KeyboardDevice.GetKeyStates(Key.Y) & KeyStates.Down; 

            if (e == null) return;
            if(ctrl > 0 && z > 0)
            {
                commandController.undo();
            }
            else if (ctrl > 0 && y > 0)
            {
                commandController.redo();
            }
        }

        public void MouseMoveShape(MouseEventArgs e)
        {
            if (Mouse.Captured != null)
            {
                // The Shape is gotten from the mouse event.
                var shape = TargetShape(e);
                if (shape == null) return;
                // The mouse position relative to the target of the mouse event.
                var mousePosition = RelativeMousePosition(e);
                if((isAddingAggregation || isAddingAssociation || isAddingComposition) && newConnection != null)
                {
                    newConnection.EndPoint = mousePosition;
                }
                else if (isResizing)
                {
                    shape.Width =  mousePosition.X- shape.X;
                    shape.Height = mousePosition.Y - shape.Y;
                    if (Math.Abs(shape.Width) < minShapeWidth) shape.Width = minShapeWidth;
                    if (Math.Abs(shape.Height) < minShapeHeight) shape.Height = minShapeHeight;
                }
                else
                {
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
            if (shape == null) return;
            // The mouse position relative to the target of the mouse event.
            var mousePosition = RelativeMousePosition(e);

            if ((isAddingAggregation || isAddingAssociation || isAddingComposition) && newConnection != null)
            {
                newConnection.End = shape;
                isAddingComposition = false;
                isAddingAssociation = false;
                isAddingAggregation = false;
                commandController.addAndExecute(new AddConnectionCommand(newConnection.Start, "", newConnection.End, "", newConnection.type)); // TODO command not implemented yet
                newConnection = null;
            }
            else
            {
                // The Shape is moved back to its original position, so the offset given to the move command works.
                shape.X = initialShapePosition.X;
                shape.Y = initialShapePosition.Y;
                commandController.addAndExecute(new MoveShapeCommand(shape, mousePosition.X - initialMousePosition.X, mousePosition.Y - initialMousePosition.Y));
            }
            // The mouse is released, as the move operation is done, so it can be used by other controls.
            e.MouseDevice.Target.ReleaseMouseCapture();
        }

        public void MouseDownShape(MouseButtonEventArgs e)
        {
            var shape = TargetShape(e);
            if (shape == null) return;
            // The mouse position relative to the target of the mouse event.
            var mousePosition = RelativeMousePosition(e);

            if (isAddingAggregation || isAddingAssociation || isAddingComposition)
            {
                Console.WriteLine("adding line...");
                ConnectionType type = ConnectionType.Aggregation;
                if (isAddingComposition) type = ConnectionType.Composition;
                else if (isAddingAssociation) type = ConnectionType.Association;
                newConnection = new Connection(shape, "", shape, "", type);
                newConnection.EndPoint = mousePosition;
                connections.Add(newConnection);
            }
          

            initialMousePosition = mousePosition;
            initialShapePosition = new Point(shape.X, shape.Y);

            e.MouseDevice.Target.CaptureMouse();
        }

        public void MouseDownResizeShape(MouseButtonEventArgs e)
        {
            var shape = TargetShape(e);
            if (shape == null) return;
            double borderX = shape.X + shape.Width;
            double borderY = shape.Y + shape.Height;
            
            // The mouse position relative to the target of the mouse event.
            var mousePosition = RelativeMousePosition(e);

            if (Math.Abs(mousePosition.X - borderX) > 5 && Math.Abs(mousePosition.Y - borderY) > 5) return;

            var border = (Border)e.MouseDevice.Target;

            initialMousePosition = mousePosition;
            initialShapePosition = new Point(shape.X, shape.Y);

            initialWidth = border.ActualWidth;

            isResizing = true;
    
            e.MouseDevice.Target.CaptureMouse();
        }

        public void MouseUpResizeShape(MouseButtonEventArgs e)
        {
            if (!isResizing) return;
            // The Shape is gotten from the mouse event.
            var shape = TargetShape(e);
            if (shape == null) return;
            // The mouse position relative to the target of the mouse event.
            var mousePosition = RelativeMousePosition(e);

            // The Shape is moved back to its original position, so the offset given to the move command works.
            shape.Width = initialMousePosition.X - shape.X;
            shape.Height = initialMousePosition.Y - shape.Y;
            
            double xOffset = mousePosition.X - initialMousePosition.X;
            double yOffset = mousePosition.Y - initialMousePosition.Y;

            if (Math.Abs(shape.Width + xOffset) < minShapeWidth) xOffset = minShapeWidth - shape.Width;
            if (Math.Abs(shape.Height + yOffset) < minShapeHeight) yOffset = minShapeHeight - shape.Height;

            commandController.addAndExecute(new ResizeShapeCommand(shape,xOffset, yOffset ));

            isResizing = false;

            // The mouse is released, as the move operation is done, so it can be used by other controls.
            e.MouseDevice.Target.ReleaseMouseCapture();
        }

        public void MouseClicked(MouseEventArgs e)
        {
            if (isAddingClass || isAddingAbstract || isAddingInterface)
            {
                if (e == null) return;
                var mousePosition = RelativeMousePosition(e);

                Point anchorpoint = new Point(mousePosition.X - minShapeWidth / 2, mousePosition.Y - minShapeHeight / 2);

                // default is normal class
                string stereoType = "";
                bool isAbstract = false;
                Models.Visibility visibility = Models.Visibility.Public;
                
                
                if (isAddingAbstract) { 
                    isAbstract = true; 
                }else if (isAddingInterface)
                {
                    stereoType = "<<interface>>";
                }

                commandController.addAndExecute(new AddClassCommand(null, stereoType, isAbstract, anchorpoint, visibility));
                Console.WriteLine("new class");

            }
            isAddingInterface = false;
            isAddingAbstract = false;
            isAddingClass = false;
        }

        private void loadFile()
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "XML files (*.xml)|*.xml";
            ofd.Title = "load diagram from xml";
            ofd.ShowDialog();

            if(ofd.FileName != "")
            {
                System.IO.Stream fileStream = ofd.OpenFile();
                using (System.IO.StreamReader reader = new System.IO.StreamReader(fileStream))
                {
                    CommandTree.load(reader);
                }
                fileStream.Close();
            }
        }
        
        private void saveFile()
        {
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.Filter = "XML files (*.xml)|*.xml";
            sfd.Title = "Save diagram as xml";
            sfd.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (sfd.FileName != "")
            {
               
                System.IO.FileStream filestream = (System.IO.FileStream)sfd.OpenFile();
                using(System.IO.StreamWriter writer = new System.IO.StreamWriter(filestream))
                {
                    CommandTree.save(commandController, writer);
                }
                filestream.Close();
            }
        }

        private void AddComposition()
        {
            isAddingComposition = true;
        }

        private void AddAssociation()
        {
            isAddingAssociation = true;
        }

        private void AddAggregation()
        {
            isAddingAggregation = true;
        }

        private void AddClass()
        {
            isAddingClass = true;
        }


        private void AddAbstract()
        {
            isAddingAbstract = true;
        }

        private void AddInterface()
        {
            isAddingInterface = true;
        }

       
        private Shape TargetShape(MouseEventArgs e)
        {
            // Here the visual element that the mouse is captured by is retrieved.
            var shapeVisualElement = (FrameworkElement)e.MouseDevice.Target;
            // From the shapes visual element, the Shape object which is the DataContext is retrieved.
            return shapeVisualElement.DataContext as Shape;
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

        public void test()
        {
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
    }
}