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
    //public enum ConnectionToAdd { NONE, ASSOCIATION, AGGREGATION, COMPOSITION }
    //public enum ClassToAdd { NONE, NORMAL, ABSTRACT, INTERFACE, COMMENT}
    public enum ButtonCommand { NONE, CLASS, ABSTRACT, INTERFACE, COMMENT, ASSOCIATION, AGGREGATION, COMPOSITION}
    public class MainViewModel : ViewModelBase
    {
        private Shape selectedShape = null;
        private bool isResizing = false;


        private ButtonCommand buttonDown = ButtonCommand.NONE;
        //private ConnectionToAdd isAddingConnection = ConnectionToAdd.NONE;
        //private bool isAddingAssociation = false;
        //private bool isAddingAggregation = false;
        //private bool isAddingComposition = false;


        //private ClassToAdd isAddingClass = ClassToAdd.NONE;
        //private bool isAddingClass = false;
        //private bool isAddingAbstract = false;
        //private bool isAddingInterface = false;
        //private bool isAddingComment = false;

        double initialWidth = 0;
        private double minShapeWidth = 150;
        private double minShapeHeight = 100;
        
        private Connection newConnection = null;

        public string Text { get; set; }

        public int MaxBranchLayer{ get{return getMaxBranchLayer();}}
        #region command variables
        // Commands that the UI can be bound to.
        // Shapes
        public ICommand MouseDownShapeCommand { get; }
        public ICommand MouseMoveShapeCommand { get; }
        public ICommand MouseUpShapeCommand { get; }

        public ICommand MouseDownShapeResizeCommand { get; }
        public ICommand MouseUpShapeResizeCommand { get; }

        public ICommand CherryPickCommand { get; }
        
        // connections
        public ICommand MouseDownConnectionCommand { get; }
        public ICommand MouseMoveConnectionCommand { get; }
        public ICommand MouseUpConnectionCommand { get; }

        // mouse and keyboard
        public ICommand KeyDownCommand { get; }
        public ICommand MouseClickCommand { get; }

        // toolbox
        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }
        public ICommand TeamCommand { get; }
        public ICommand NewCommand { get; }

        public ICommand AddAssociationCommand { get; }
        public ICommand AddAggregationCommand { get; }
        public ICommand AddCompositionCommand { get; }

        public ICommand NewClassCommand { get; }
        public ICommand NewAbstractCommand { get; }
        public ICommand NewInterfaceCommand { get; }
        public ICommand NewCommentCommand { get; }
      
        #endregion
        // Used for saving the classRep that a line is drawn from, while it is being drawn.
        private Shape addingLineFrom;
        // Saves the initial point that the mouse has during a move operation.
        private Point initialMousePosition;
        // Saves the initial point that the classRep has during a move operation.
        private Point initialShapePosition;

        //view access to observables
        public ObservableCollection<BaseCommand> commands { get { return ShapeCollector.getI().commands; } }
        public ObservableCollection<Shape> classes { get { return ShapeCollector.getI().obsShapes; } }
        public ObservableCollection<Connection> connections { get { return ShapeCollector.getI().obsConnections; } }

        //Dynamic 
        private CommandTree commandController { get; set; }
        //private ShapeCollector observables = ShapeCollector.getI();

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            commandController = new CommandTree();
            // The commands are given the methods they should use to execute, and find out if they can execute.

            #region initialize commands
            MouseDownShapeCommand = new RelayCommand<MouseButtonEventArgs>(MouseDownShape);
            MouseMoveShapeCommand = new RelayCommand<MouseEventArgs>(MouseMoveShape);
            MouseUpShapeCommand = new RelayCommand<MouseButtonEventArgs>(MouseUpShape);

            MouseDownShapeResizeCommand = new RelayCommand<MouseButtonEventArgs>(MouseDownResizeShape);
            MouseUpShapeResizeCommand = new RelayCommand<MouseButtonEventArgs>(MouseUpResizeShape);

            CherryPickCommand = new RelayCommand<MouseEventArgs>(CherryPick);

            //TODO implement these
            //MouseDownConnectionCommand =
            //MouseMoveConnectionCommand = 
            //MouseUpConnectionCommand =
             
            KeyDownCommand = new RelayCommand<KeyEventArgs>(KeyDown);
            MouseClickCommand = new RelayCommand<MouseEventArgs>(MouseClicked);

            SaveCommand = new RelayCommand(saveFileCmd);
            LoadCommand = new RelayCommand(loadFile);
            NewCommand = new RelayCommand(StartNewProject);
            TeamCommand = new RelayCommand(StartTeamProject);

            AddAggregationCommand = new RelayCommand(AddAggregation);
            AddAssociationCommand = new RelayCommand(AddAssociation);
            AddCompositionCommand = new RelayCommand(AddComposition);

            NewClassCommand = new RelayCommand(AddClass);
            NewInterfaceCommand = new RelayCommand(AddInterface);
            NewAbstractCommand = new RelayCommand(AddAbstract);
            NewCommentCommand = new RelayCommand(AddComment);
            
            #endregion
            // TODO remove
            test();
        }



        public void KeyDown(KeyEventArgs e)
        {
            KeyStates ctrl = e.KeyStates & e.KeyboardDevice.GetKeyStates(Key.LeftCtrl) & KeyStates.Down;
            KeyStates z = e.KeyStates & e.KeyboardDevice.GetKeyStates(Key.Z) & KeyStates.Down; 
            KeyStates y = e.KeyStates & e.KeyboardDevice.GetKeyStates(Key.Y) & KeyStates.Down;
            KeyStates s = e.KeyStates & e.KeyboardDevice.GetKeyStates(Key.S) & KeyStates.Down;
            KeyStates o = e.KeyStates & e.KeyboardDevice.GetKeyStates(Key.O) & KeyStates.Down;
            KeyStates esc = e.KeyStates & e.KeyboardDevice.GetKeyStates(Key.Escape) & KeyStates.Down;
            KeyStates del = e.KeyStates & e.KeyboardDevice.GetKeyStates(Key.Delete) & KeyStates.Down;


            if (e == null) return;
            if(ctrl > 0)
            {
                if (z > 0)
                {
                    commandController.undo();
                }
                else if (y > 0)
                {
                    commandController.redo();
                }
                else if (s > 0)
                {
                    saveFile();
                }
                else if (o > 0){
                    loadFile();
                }
            }
            else if (esc > 0)
            {
                Keyboard.ClearFocus();
                selectedShape = null;
                buttonDown = ButtonCommand.NONE;
            }
            else if(del > 0 && selectedShape != null)
            {
                commandController.addAndExecute(new DeleteShapeCommand(selectedShape));
                selectedShape = null;
            }
        }

        public void MouseMoveShape(MouseEventArgs e)
        {
            if (Mouse.Captured != null)
            {
                // The Shape is gotten from the mouse event.
                var shape = TargetShape(e);
                
                // The mouse position relative to the target of the mouse event.
                var mousePosition = RelativeMousePosition(e);
                if (!buttonDown.Equals(ButtonCommand.NONE) && newConnection != null)
                {
                    newConnection.updatePoints(mousePosition);
                }
                else if(buttonDown.Equals(ButtonCommand.NONE) && shape != null)
                {
                    if (isResizing)
                    {

                        shape.Width = mousePosition.X - shape.X;
                        shape.Height = mousePosition.Y - shape.Y;
                        if (Math.Abs(shape.Width) < minShapeWidth) shape.Width = minShapeWidth;
                        if (Math.Abs(shape.Height) < minShapeHeight) shape.Height = minShapeHeight;
                    }
                    else
                    {
                        shape.X = initialShapePosition.X + (mousePosition.X - initialMousePosition.X);
                        shape.Y = initialShapePosition.Y + (mousePosition.Y - initialMousePosition.Y);

                        // lambda expr. update all connections. first connections where end classRep is the moving classRep then where start classRep is moving classRep
                        connections.Where(x => x.End.id == shape.id).ToList().ForEach(x => x.End = shape);
                        connections.Where(x => x.Start.id == shape.id).ToList().ForEach(x => x.Start = shape);
                    }
                }
            }
        }
        public void MouseUpShape(MouseButtonEventArgs e)
        {
            // The Shape is gotten from the mouse event.
            e.MouseDevice.Target.ReleaseMouseCapture();
            var shape = TargetShape(e);
            
            
            // The mouse position relative to the target of the mouse event.
            var mousePosition = RelativeMousePosition(e);

            if (!buttonDown.Equals(ButtonCommand.NONE) && newConnection != null)
            {
                if (shape == null || newConnection.Start.Equals(shape))
                {
                    connections.Remove(newConnection);
                }
                else
                {
                    newConnection.End = shape;
                    commandController.addAndExecute(new AddConnectionCommand(newConnection.Start, "", newConnection.End, "", newConnection.type)); // TODO command not implemented yet
                    
                }
                newConnection = null;
            }
            else if(buttonDown.Equals(ButtonCommand.NONE) && shape != null)
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

            ConnectionType type = ConnectionType.Association;
            bool addConnection = true;
            switch (buttonDown)
            {
                case ButtonCommand.AGGREGATION:
                    type = ConnectionType.Aggregation;
                    break;
                case ButtonCommand.ASSOCIATION:
                    type = ConnectionType.Association;
                    break;
                case ButtonCommand.COMPOSITION:
                    type = ConnectionType.Composition;
                    break;
                default:
                    addConnection = false;
                    break;

            }

            if (addConnection) { 
            newConnection = new Connection(shape, "", null, "", type);
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

        public void CherryPick(MouseEventArgs e)
        {
            var cmd = TargetCommand(e);
            commandController.setActiveCommand(cmd);
        }

        private int getMaxBranchLayer()
        {
            int max = 0;
            foreach(BaseCommand b in commands){
                if (b.BranchLayer > max) max = b.BranchLayer;
            }
            return max;
        }
        public void MouseClicked(MouseEventArgs e)
        {
            
            if (!buttonDown.Equals(ButtonCommand.NONE))
            {
                if (e == null) return;
                var mousePosition = RelativeMousePosition(e);

                Point anchorpoint = new Point(mousePosition.X - minShapeWidth / 2, mousePosition.Y - minShapeHeight / 2);

                // default is normal class/comment --- comment should perhaps have some modifications
                string stereoType = "";
                bool isAbstract = false;
                Models.Visibility visibility = Models.Visibility.Public;

                switch(buttonDown){
                    case ButtonCommand.ABSTRACT:
                        isAbstract = true;
                        break;
                    case ButtonCommand.INTERFACE:
                        stereoType = "<<interface>>";
                        break;
                    case ButtonCommand.CLASS:
                    case ButtonCommand.COMMENT:
                        break;
                    default:
                        return;
                }
             

                commandController.addAndExecute(new AddClassCommand(null, stereoType, isAbstract, anchorpoint, visibility));
            }
            else
            {
                var shape = TargetShape(e);
                selectedShape = shape;
            }
        }

        private void StartNewProject()
        {
            bool clear = false;
            if (ShapeCollector.getI().obsConnections.Any() || ShapeCollector.getI().obsShapes.Any() || commands.Any())
            {
                MessageBoxResult res = MessageBox.Show("You have already a diagram in progress. Do you want to save first?", "Save current diagram", MessageBoxButton.YesNoCancel);
                if (res == MessageBoxResult.Yes)
                {
                    clear = saveFile();
                }
                else if (res == MessageBoxResult.Cancel)
                {
                    return;
                }
                else if (res == MessageBoxResult.No)
                {
                    clear = true;
                }

            }
            if (clear)
            {
                commandController = new CommandTree();
                ShapeCollector.getI().obsConnections.Clear();
                ShapeCollector.getI().obsShapes.Clear();
                ShapeCollector.getI().commands.Clear();
            }
        }

        private void StartTeamProject()
        {
            MessageBox.Show("This feature is not implemented yet");
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
        
        private void saveFileCmd()
        {
            saveFile();
        }
        private bool saveFile()
        {
            bool saved = false;
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
                    saved = true;
                }
                filestream.Close();
            }
            return saved;
        }

        private void AddComposition()
        {
            buttonDown = (buttonDown.Equals(ButtonCommand.COMPOSITION) ? ButtonCommand.NONE : ButtonCommand.COMPOSITION);
        }

        private void AddAssociation()
        {
            buttonDown = (buttonDown.Equals(ButtonCommand.ASSOCIATION) ? ButtonCommand.NONE : ButtonCommand.ASSOCIATION);

        }

        private void AddAggregation()
        {
            buttonDown = (buttonDown.Equals(ButtonCommand.AGGREGATION) ? ButtonCommand.NONE : ButtonCommand.AGGREGATION);
        }

        private void AddClass()
        {
            buttonDown = (buttonDown.Equals(ButtonCommand.CLASS) ? ButtonCommand.NONE : ButtonCommand.CLASS);
        }


        private void AddAbstract()
        {
            buttonDown = (buttonDown.Equals(ButtonCommand.ABSTRACT) ? ButtonCommand.NONE : ButtonCommand.ABSTRACT);
        }

        private void AddInterface()
        {
            buttonDown = (buttonDown.Equals(ButtonCommand.INTERFACE) ? ButtonCommand.NONE : ButtonCommand.INTERFACE);
        }

        private void AddComment()
        {
            buttonDown = (buttonDown.Equals(ButtonCommand.COMMENT) ? ButtonCommand.NONE : ButtonCommand.COMMENT);
        }

        private BaseCommand TargetCommand(MouseEventArgs e)
        {
            // Here the visual element that the mouse is captured by is retrieved.
            var shapeVisualElement = (FrameworkElement)e.MouseDevice.Target;
            // From the shapes visual element, the Shape object which is the DataContext is retrieved.
            return shapeVisualElement.DataContext as BaseCommand;
        }
        private Shape TargetShape(MouseEventArgs e)
        {
            // Here the visual element that the mouse is captured by is retrieved.
            var shapeVisualElement = (FrameworkElement)e.MouseDevice.Target;
            if (shapeVisualElement == null) return null;
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
            
            dynamic parent = (o == null ? null : VisualTreeHelper.GetParent(o));
            if (parent == null) return parent;
            return parent.GetType().IsAssignableFrom(typeof(T)) ? parent : FindParentOfType<T>(parent);
        }

        public void test()
        {

            commands.Add(new DummyCommand());
            Class TestClass1 = new Class("A Class", "", false, new Point(0, 0), Models.Visibility.Default);
            TestClass1.addAttribute(Models.Visibility.Private, "int", "something");
            TestClass1.addAttribute("String", "someAttribute");
            string[] parameters = { "string", "Int", "Bool" };
            TestClass1.addMethod(Models.Visibility.Private, "somemethod", parameters);
            //observables.obsShapes.Add(TestClass1);
            Class TestClass2 = new Class("Another Class", "", false, new Point(300, 320), Models.Visibility.Default);
            TestClass2.addAttribute("int", "nothing");
            TestClass2.addAttribute("bool", "True");
            //observables.obsShapes.Add(TestClass2);
            Class TestClass3 = new Class("Another Class", "", false, new Point(100, 60), Models.Visibility.Default);
            TestClass2.addAttribute("int", "nothing");
            TestClass2.addAttribute("bool", "True");
            //observables.obsShapes.Add(TestClass3);

            //Connection conn = new Connection(TestClass1, "asd", TestClass2, "efg", ConnectionType.Association);
            //observables.obsConnections.Add(conn);
            //Console.WriteLine(TestClass1.CanvasCenterX + "," + TestClass1.CanvasCenterY);
            //Console.WriteLine(TestClass2.CanvasCenterX + "," + TestClass2.CanvasCenterY);
            //Connection conn2 = new Connection(TestClass2, "asd", TestClass3, "efg", ConnectionType.Association);
            //observables.obsConnections.Add(conn2);
            //Console.WriteLine(TestClass1.CanvasCenterX + "," + TestClass1.CanvasCenterY);
            //Console.WriteLine(TestClass2.CanvasCenterX + "," + TestClass2.CanvasCenterY);
        }
    }
}