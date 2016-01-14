using Area51.SoftwareModeler.Models.Commands;
using Area51.SoftwareModeler.Models;
using System.Collections.ObjectModel;
using System.Windows;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Area51.SoftwareModeler.Views;
using Visibility = System.Windows.Visibility;
using Area51.SoftwareModeler.Model.CodeGen;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

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
    [Flags]
    public enum ButtonCommand
    {
        None = 0,
        Class = 1,
        Abstract = 2,
        Interface = 4,
        Comment = 8,
        Association = 16,
        Aggregation = 32,
        Composition = 64,
        Inheritance = 128,
        Shape = Class | Abstract | Interface | Comment,
        Connection = Aggregation | Association | Composition | Inheritance
    }

    [Flags]
    public enum Action
    {
        Resizing = 1,
        MovingClass = 2,
        MovingComment = 4,
        MovingConnection = 8,
        AddingClass = 16,
        AddingConnection = 32,
        Selecting = 64,
        NoSelecting = 128,
        Adding = AddingClass | AddingConnection,
        Moving = MovingClass | MovingConnection,
        All = Resizing | MovingClass | MovingConnection | AddingClass | AddingConnection | Selecting | NoSelecting,
        Nothing = 0
    }

    public class MainViewModel : ViewModelBase
	{
	    private EditClassPopupWindow EditClassWindow { get; set; } = new EditClassPopupWindow();

	    public ButtonCommand ButtonDown { get; private set; } = ButtonCommand.None;

	    protected double InitialWidth = 0;
		private double minShapeWidth = 150;
		private double minShapeHeight = 100;
		private bool IsEditable { get; set; } = false;
		

	    private ConnectionData NewConnectionData { get; set; } = null;

		public string Text { get; set; }

		#region command variables
		// Commands that the UI can be bound to.
		// Shapes
		public ICommand ClearSelectionCommand { get; }
		public ICommand EditClassContentCancelCommand { get; }
		public ICommand EditClassContentOkCommand { get; }

        public ICommand MouseDownClassCommand { get; }
        public ICommand MouseMoveClassCommand { get; }
        public ICommand MouseUpClassCommand { get; }


        public ICommand MouseDownCommentCommand { get; }
        public ICommand MouseMoveCommentCommand { get; }
        public ICommand MouseUpCommentCommand { get; }

        public ICommand CherryPickCommand { get; }

        // Connections
        public ICommand MouseDownConnectionCommand { get; }
        public ICommand MouseMoveConnectionCommand { get; }
        public ICommand MouseUpConnectionCommand { get; }

        // mouse and keyboard
        public ICommand KeyEventCommand { get; }
		public ICommand MouseClickCommand { get; }

		// toolbox
		public ICommand SaveCommand { get; }
		public ICommand LoadCommand { get; }
		public ICommand TeamCommand { get; }
		public ICommand NewCommand { get; }
        public ICommand GenerateCodeCommand { get; }
        public ICommand HelpCommand { get; }

        public ICommand AddInheritanceCommand { get; }
		public ICommand AddAssociationCommand { get; }
		public ICommand AddAggregationCommand { get; }
		public ICommand AddCompositionCommand { get; }

		public ICommand NewClassCommand { get; }
		public ICommand NewAbstractCommand { get; }
		public ICommand NewInterfaceCommand { get; }
		public ICommand NewCommentCommand { get; }


        #endregion
        private List<ClassData> SelectedClasses = new List<ClassData>();
        private List<Comment> SelectedComments = new List<Comment>(); 
        private List<ConnectionData> SelectedConnections = new List<ConnectionData>(); 
        private List<ClassData> CopiedClasses = new List<ClassData>();
        private List<Comment> copiedComments = new List<Comment>();
	    private List<ConnectionData> CopiedConnection = new List<ConnectionData>();

        private ClassData _classDataToEdit = null;
        private bool _multiSelect = false;
        private bool _multiAdd = false;
        private Action action = Action.Nothing;

		// Saves the initial point that the mouse has during a move operation.
		private Point _initialMousePosition;
        // Saves the initial point that the ClassDataRep has during a move operation.
        //private List<Point> _initialClassPosition = new List<Point>();
		private long _doubleClickTimer;
		private long doubleClickTimeout = 500*10000; // nanosec. is 500msec
        

        //maxbranchlayer added as an observablecollection for now, not a nice fix, but it works (for scroll area).
		public ObservableCollection<int> MaxBranchLayer { get { return ShapeCollector.GetI().MaxBranchLayer; } }
        //new collection for the lines in the command-tree.
        public ObservableCollection<LineCommandTree> treeArrows { get { return ShapeCollector.GetI().treeArrows; } }
        //private ClassData _classDataToEdit = null;
		public ObservableCollection<BaseCommand> Commands => ShapeCollector.GetI().Commands;
	    public ObservableCollection<ClassData> Classes => ShapeCollector.GetI().ObsShapes;
        public ObservableCollection<Comment> Comments => ShapeCollector.GetI().ObsComments; 
	    public ObservableCollection<ConnectionData> Connections => ShapeCollector.GetI().ObsConnections;


		//Dynamic 
		private CommandTree CommandController { get; set; }
		//private ShapeCollector observables = ShapeCollector.GetI();

		/// <summary>
		/// Initializes a new instance of the MainViewModel class.
		/// </summary>
		public MainViewModel()
		{

			CommandController = new CommandTree();
			// The Commands are given the methods they should use to execute, and find out if they can execute.

			#region initialize Commands
			ClearSelectionCommand = new RelayCommand(ClearSelectedShapes);
			EditClassContentOkCommand = new RelayCommand(EditClassOk);
			EditClassContentCancelCommand = new RelayCommand(EditClassCancel);

            MouseDownClassCommand = new RelayCommand<MouseButtonEventArgs>(MouseDownClass);
            MouseMoveClassCommand = new RelayCommand<MouseEventArgs>(MouseMoveClass);
            MouseUpClassCommand = new RelayCommand<MouseButtonEventArgs>(MouseUpClass);

            MouseDownCommentCommand = new RelayCommand<MouseButtonEventArgs>(MouseDownComment);
            MouseMoveCommentCommand = new RelayCommand<MouseEventArgs>(MouseMoveComment);
            MouseUpCommentCommand = new RelayCommand<MouseButtonEventArgs>(MouseUpComment);

            MouseDownConnectionCommand = new RelayCommand<MouseButtonEventArgs>(MouseDownConnection);
            MouseMoveConnectionCommand = new RelayCommand<MouseEventArgs>(MouseMoveConnection);
            MouseUpConnectionCommand = new RelayCommand<MouseButtonEventArgs>(MouseUpConnection);

            CherryPickCommand = new RelayCommand<MouseEventArgs>(CherryPick);

            KeyEventCommand = new RelayCommand<KeyEventArgs>(KeyEvent);
			MouseClickCommand = new RelayCommand<MouseEventArgs>(MouseClicked);

			SaveCommand = new RelayCommand(saveFileCmd);
			LoadCommand = new RelayCommand(loadFile);
			NewCommand = new RelayCommand(StartNewProject);
			TeamCommand = new RelayCommand(StartTeamProject);
            GenerateCodeCommand = new RelayCommand(generateCode);
            HelpCommand = new RelayCommand(Help);

            AddInheritanceCommand = new RelayCommand(AddInheritance);
			AddAggregationCommand = new RelayCommand(AddAggregation);
			AddAssociationCommand = new RelayCommand(AddAssociation);
			AddCompositionCommand = new RelayCommand(AddComposition);

			NewClassCommand = new RelayCommand(AddClass);
			NewInterfaceCommand = new RelayCommand(AddInterface);
			NewAbstractCommand = new RelayCommand(AddAbstract);
			NewCommentCommand = new RelayCommand(AddComment);
			
			#endregion
		}

		public void KeyEvent(KeyEventArgs e)
		{
			KeyStates ctrl = e.KeyStates & e.KeyboardDevice.GetKeyStates(Key.LeftCtrl) & KeyStates.Down;
			KeyStates z = e.KeyStates & e.KeyboardDevice.GetKeyStates(Key.Z) & KeyStates.Down; 
			KeyStates y = e.KeyStates & e.KeyboardDevice.GetKeyStates(Key.Y) & KeyStates.Down;
			KeyStates s = e.KeyStates & e.KeyboardDevice.GetKeyStates(Key.S) & KeyStates.Down;
			KeyStates o = e.KeyStates & e.KeyboardDevice.GetKeyStates(Key.O) & KeyStates.Down;
			KeyStates esc = e.KeyStates & e.KeyboardDevice.GetKeyStates(Key.Escape) & KeyStates.Down;
			KeyStates del = e.KeyStates & e.KeyboardDevice.GetKeyStates(Key.Delete) & KeyStates.Down;
            KeyStates shiftL = e.KeyStates & e.KeyboardDevice.GetKeyStates(Key.LeftShift) & KeyStates.Down;
            KeyStates shiftR = e.KeyStates & e.KeyboardDevice.GetKeyStates(Key.RightShift) & KeyStates.Down;
            KeyStates c = e.KeyStates & e.KeyboardDevice.GetKeyStates(Key.C) & KeyStates.Down;
            KeyStates v = e.KeyStates & e.KeyboardDevice.GetKeyStates(Key.V) & KeyStates.Down;



            _multiSelect = false;
            _multiAdd = false;
			if(ctrl > 0)
			{
				_multiSelect = true;
				if (z > 0)
				{
					CommandController.Undo();
				}
				else if (y > 0)
				{
					CommandController.Redo();
				}
				else if (s > 0)
				{
					saveFile();
				}
				else if (o > 0){
					loadFile();
				}else if(c > 0)
                {
                    CopiedClasses.Clear();
                    SelectedClasses.ForEach(x => CopiedClasses.Add(x));
                }else if(v > 0)
                {
                    CopiedClasses.ForEach(x => execCommand(new CopyClassCommand(x)));
                }
			}
			else if (esc > 0)
			{
				ClearSelectedShapes();
				ButtonDown = ButtonCommand.None;
			}
			else if(del > 0 && SelectedClasses != null)
			{
				SelectedClasses.ForEach(x => execCommand(new DeleteShapeCommand(x)));
                SelectedConnections.ForEach(x => execCommand(new DeleteConnectionCommand(x)));
				ClearSelectedShapes();
			}
            else if (shiftR > 0 || shiftL > 0)
            {
                _multiAdd = true;
		}
		}

		public void EditClassContent(ClassView classView)
		{
            bool newWind = (EditClassWindow == null ||
                                    EditClassWindow.Visibility == Visibility.Collapsed ||
                                    EditClassWindow.Visibility == Visibility.Hidden ||
                                    !EditClassWindow.IsEnabled);
            if (EditClassWindow != null) EditClassWindow.Close();

            EditClassWindow = new EditClassPopupWindow();
            _classDataToEdit = (ClassData)classView;
            EditClassWindow.ClassName.Text = _classDataToEdit.name;
            string stereoType = _classDataToEdit.StereoType;
            stereoType = stereoType == null || stereoType.Length < 4? "" : stereoType.Remove(0, 2);
            stereoType = stereoType == null || stereoType.Length < 2? "" : stereoType.Remove(stereoType.Length - 2, 2);
            EditClassWindow.StereoType.Text = stereoType;
            EditClassWindow.IsAbstract.IsChecked = _classDataToEdit.IsAbstract;
            EditClassWindow.Methods.ItemsSource = _classDataToEdit.Methods;
            EditClassWindow.Attributes.ItemsSource = _classDataToEdit.Attributes;
            EditClassWindow.Ok.Command = EditClassContentOkCommand;
            EditClassWindow.Cancel.Command = EditClassContentCancelCommand;
            EditClassWindow.Show();
        }

		public void EditClassOk()
		{
		  var methods =   EditClassWindow.Methods.Items.OfType<Method>().ToList<Method>(); ;// as List<Method>;

			var attributes = EditClassWindow.Attributes.Items.OfType<Models.Attribute>().ToList<Models.Attribute>();

			execCommand(new UpdateClassInfoCommand(_classDataToEdit, EditClassWindow.ClassName.Text, EditClassWindow.StereoType.Text, EditClassWindow.IsAbstract.IsChecked, methods, attributes));
			
			_classDataToEdit = null;
			ClearSelectedShapes();
			EditClassWindow.Hide();
		}

        public void EditClassCancel()
        {
            _classDataToEdit = null;
            EditClassWindow.Visibility = Visibility.Collapsed;
        }

        public void ClearSelectedShapes()
		{
			SelectedClasses.ForEach(x => x.IsSelected = false);
            SelectedComments.ForEach(x => x.IsSelected = false);
            SelectedConnections.ForEach(x => x.IsSelected = false);
            
			SelectedClasses.Clear();
            SelectedComments.Clear();
            SelectedConnections.Clear();
		}

        public void SelectComment(Comment c)
        {
            Console.WriteLine("selecting comment: " + c);
            if (c == null) return;
            if (SelectedComments == null) SelectedComments = new List<Comment>();

            if (!_multiSelect) ClearSelectedShapes();

            if (!SelectedComments.Contains(c))
            {
                c.IsSelected = true;
                SelectedComments.Add(c);

                c.InitialPosition = new Point(c.X, c.Y);
            }
            else
            {
                c.IsSelected = false;
                SelectedComments.Remove(c);
            }
        }

		public void SelectShape(ClassData s)
		{
			if (s == null) return;
			if (SelectedClasses == null) SelectedClasses = new List<ClassData>();
			
			if (!_multiSelect) ClearSelectedShapes();

		    if (!SelectedClasses.Contains(s))
		    {
		        s.IsSelected = true;
		        SelectedClasses.Add(s);

		        s.InitialPosition = new Point(s.X, s.Y);
		    }
		    else
		    {
		        s.IsSelected = false;
		        SelectedClasses.Remove(s);
		    }
		}

	    public void SelectConnection(ConnectionData c)
	    {
	        if (c == null) return;
            if(SelectedConnections == null) SelectedConnections = new List<ConnectionData>();

            if(!_multiSelect) ClearSelectedShapes();

	        if (!SelectedConnections.Contains(c))
	        {
	            c.IsSelected = true;
	            SelectedConnections.Add(c);
	        }
	        else
	        {
	            c.IsSelected = false;
	            SelectedConnections.Remove(c);
	        }
	    }

		public void MoveShape(Point mousePosition)
		{

		    double xOffset = (mousePosition.X - _initialMousePosition.X);
		    double yOffset = (mousePosition.Y - _initialMousePosition.Y);
            if(xOffset + yOffset > 0) action = Action.Moving;
            for (int i = 0; i < SelectedClasses.Count; i++) {

                SelectedClasses.ElementAt(i).X = SelectedClasses.ElementAt(i).InitialPosition.X + xOffset;
                SelectedClasses.ElementAt(i).Y = SelectedClasses.ElementAt(i).InitialPosition.Y + yOffset;

                // lambda expr. update all Connections. first Connections where end ClassDataRep is the moving ClassDataRep then where start ClassDataRep is moving ClassDataRep
                Connections.Where(x => x.EndShapeId == SelectedClasses.ElementAt(i).id).ToList().ForEach(x => x.updatePoints());
                Connections.Where(x => x.StartShapeId == SelectedClasses.ElementAt(i).id).ToList().ForEach(x => x.updatePoints());
            }

            for (int i = 0; i < SelectedComments.Count; i++)
            {

                SelectedComments.ElementAt(i).X = SelectedComments.ElementAt(i).InitialPosition.X + xOffset;
                SelectedComments.ElementAt(i).Y = SelectedComments.ElementAt(i).InitialPosition.Y + yOffset;

                // lambda expr. update all Connections. first Connections where end ClassDataRep is the moving ClassDataRep then where start ClassDataRep is moving ClassDataRep
                Connections.Where(x => x.EndShapeId == SelectedComments.ElementAt(i).id).ToList().ForEach(x => x.updatePoints());
                Connections.Where(x => x.StartShapeId == SelectedComments.ElementAt(i).id).ToList().ForEach(x => x.updatePoints());
            }
        }

        public void MoveShapeDone(Point mousePosition)
        {
      
            double xOffset = mousePosition.X - _initialMousePosition.X;
            double yOffset = mousePosition.Y - _initialMousePosition.Y;

            if (Math.Abs(xOffset) < 10 && Math.Abs(yOffset) < 10) return;

            for (int i = 0; i < SelectedClasses.Count; i++) {
                // The ClassView is moved back to its original position, so the offset given to the move command works.
                double X = SelectedClasses.ElementAt(i).InitialPosition.X;
                double Y = SelectedClasses.ElementAt(i).InitialPosition.Y;
                SelectedClasses.ElementAt(i).X = X;
                SelectedClasses.ElementAt(i).Y = Y;

                SelectedClasses.ElementAt(i).InitialPosition = new Point(X + xOffset, Y+yOffset);            
              
                execCommand(new MoveShapeCommand(SelectedClasses.ElementAt(i), xOffset, yOffset));
            }

            if (Math.Abs(xOffset) < 10 && Math.Abs(yOffset) < 10) return;

            for (int i = 0; i < SelectedComments.Count; i++)
            {
                // The ClassView is moved back to its original position, so the offset given to the move command works.
                double X = SelectedComments.ElementAt(i).InitialPosition.X;
                double Y = SelectedComments.ElementAt(i).InitialPosition.Y;
                SelectedComments.ElementAt(i).X = X;
                SelectedComments.ElementAt(i).Y = Y;

                SelectedComments.ElementAt(i).InitialPosition = new Point(X + xOffset, Y + yOffset);

                execCommand(new MoveShapeCommand(SelectedComments.ElementAt(i), xOffset, yOffset));
            }

        }

        public bool ResizeShapeInit(ClassView @class, MouseButtonEventArgs e)
        {
            if (@class == null) return false;
            double borderX = @class.X + @class.Width;
            double borderY = @class.Y + @class.Height;

            // The mouse position relative to the target of the mouse event.
            var mousePosition = RelativeMousePosition(e);

            if (Math.Abs(mousePosition.X - borderX) < 5 && Math.Abs(mousePosition.Y - borderY) < 5) return false;

            var border = e.MouseDevice.Target as Border;
            if (border == null) return false;

            _initialMousePosition = mousePosition;

            InitialWidth = border.ActualWidth;

            return true;


        }

		public void ResizeShape(ClassView classView, Point mousePosition)
		{
            ClearSelectedShapes();
            classView.Width = mousePosition.X - classView.X;
			classView.Height = mousePosition.Y - classView.Y;
			if (Math.Abs(classView.Width) < minShapeWidth) classView.Width = minShapeWidth;
			if (Math.Abs(classView.Height) < minShapeHeight) classView.Height = minShapeHeight;
		}

        public void ResizeShapeDone(ClassView shape, Point mousePosition)
        {
            if (action != Action.Resizing) return;          
            if (shape == null) return;
           
            // The ClassView is moved back to its original position, so the offset given to the move command works.
            shape.Width = _initialMousePosition.X - shape.X;
            shape.Height = _initialMousePosition.Y - shape.Y;

            double xOffset = mousePosition.X - _initialMousePosition.X;
            double yOffset = mousePosition.Y - _initialMousePosition.Y;

            if (Math.Abs(shape.Width + xOffset) < minShapeWidth) xOffset = minShapeWidth - shape.Width;
            if (Math.Abs(shape.Height + yOffset) < minShapeHeight) yOffset = minShapeHeight - shape.Height;

            execCommand(new ResizeShapeCommand(shape, xOffset, yOffset));

     
        }

        public void AddConnection(ClassData shape)
            {
            if (shape != null && NewConnectionData.StartShapeId != shape.id)
            {
                NewConnectionData.EndShapeId = shape.id;
                    execCommand(new AddConnectionCommand(NewConnectionData.ConnectionId, NewConnectionData.StartShapeId, "", shape.id ?? 0, "", NewConnectionData.Type));
                if (!_multiAdd) ButtonDown = ButtonCommand.None;

            }
            Connections.Remove(NewConnectionData);
            NewConnectionData = null;
        }
      
        public void AddConnectionInit(ClassData shape, Point mousePosition)
        {
            ConnectionType type = ConnectionType.Association;
            switch (ButtonDown)
            {
                case ButtonCommand.Aggregation:
                    type = ConnectionType.Aggregation;
                    break;
                case ButtonCommand.Association:
                    type = ConnectionType.Association;
                    break;
                case ButtonCommand.Composition:
                    type = ConnectionType.Composition;
                    break;
                case ButtonCommand.Inheritance:
                    type = ConnectionType.Inheritance;
                    break;
                default:
                    return;
            }
            
            NewConnectionData = new ConnectionData(shape.id ?? 0, "", null, "", type);
            NewConnectionData.EndPoint = mousePosition;
            Connections.Add(NewConnectionData);
        }

        public void MouseDownClass(MouseButtonEventArgs e)
        {
            var classObj = TargetShape(e);
            var mousePosition = RelativeMousePosition(e);
            _initialMousePosition = mousePosition;

            if ((ButtonDown & ButtonCommand.Connection) > 0)
            {
                AddConnectionInit(classObj, mousePosition);
                action = action | Action.AddingConnection;
            }
            else
            {
                // if cursor is close enough to right/bottom border, we want to resize
                // therefore resize and move is not set at the same time 
                if (ResizeShapeInit(classObj, e))
                {
                    action = action | Action.Resizing;
                }
                else
                {
                    action = action | Action.MovingClass; // can only move class at the moment
                }
            }
            e.MouseDevice.Target.CaptureMouse();
        }

        public void MouseDownComment(MouseButtonEventArgs e)
        {
            var comment = TargetComment(e);
            var mousePosition = RelativeMousePosition(e);
            _initialMousePosition = mousePosition;

            // if cursor is close enough to right/bottom border, we want to resize
            // therefore resize and move is not set at the same time 
            if (ResizeShapeInit(comment, e))
            {
                action = action | Action.Resizing;
            }
            else
            {
                action = action | Action.MovingComment;
                action = action | Action.Selecting;
            }
            e.MouseDevice.Target.CaptureMouse();
        }
        public void MouseDownConnection(MouseButtonEventArgs e) { }

        //public void MouseDown(MouseButtonEventArgs e)
        //{
            
        //    var shape = TargetShape(e);
        //    var connection = TargetConnection(e);
        //    if (shape != null || connection != null) action = action | Action.Selecting;

        //    // The mouse position relative to the target of the mouse event.
        //    var mousePosition = RelativeMousePosition(e);
        //    _initialMousePosition = mousePosition;

        //    if((ButtonDown & ButtonCommand.Connection) > 0) { 
        //        AddConnectionInit(shape, mousePosition);
        //        action = action | Action.AddingConnection;
        //    }
        //    else if ((ButtonDown & ButtonCommand.Shape) > 0)
        //    {
        //        // no class initialization needed
        //        action = action | Action.AddingClass;
        //    }
        //    else
        //    {
        //        // if cursor is close enough to right/bottom border, we want to resize
        //        // therefore resize and move is not set at the same time 
        //        if (ResizeShapeInit(shape, e))
        //        {
        //            action = action | Action.Resizing;
        //        }
        //        else
        //        {
        //            action = action | Action.Moving; // can only move class at the moment
        //        }
        //    }
        //    e.MouseDevice.Target.CaptureMouse();
        //}

        public void MouseMoveClass(MouseEventArgs e)
        {
            if (Mouse.Captured == null) return;

            var classObj = TargetShape(e);
            var mousePosition = RelativeMousePosition(e);

            // draw connection 
            if ((action & Action.AddingConnection) > 0)
            {
                NewConnectionData.updatePoints(mousePosition);
            }
            // resize or move ClassView
            else if ((action & Action.Resizing) > 0)
            {   // resizing
                ResizeShape(classObj, mousePosition);
            }
            else if ((action & Action.MovingClass) > 0)
            {
                // move ClassView if class in focus is selected, otherwise it is expected that a selection is about to be made
                if (SelectedClasses.Contains(classObj))
                {
                    MoveShape(mousePosition);
                    action = action | Action.NoSelecting;
                }
                else action = action | Action.Selecting;
            }
            else action = action | Action.Selecting;
        }

        public void MouseMoveComment(MouseEventArgs e)
        {
            if (Mouse.Captured == null) return;

            var comment = TargetComment(e);
            var mousePosition = RelativeMousePosition(e);

            // draw connection 
            if ((action & Action.AddingConnection) > 0)
            {
                NewConnectionData.updatePoints(mousePosition);
            }
            // resize or move ClassView
            else if ((action & Action.Resizing) > 0)
            {   // resizing
                ResizeShape(comment, mousePosition);
            }
            else if ((action & Action.MovingComment) > 0)
            {
                // move ClassView if class in focus is selected, otherwise it is expected that a selection is about to be made
                if (SelectedComments.Contains(comment))
                {
                    MoveShape(mousePosition);
                    action = action | Action.NoSelecting;
                }
                else action = action | Action.Selecting;
            }
            else action = action | Action.Selecting;
        }
        public void MouseMoveConnection(MouseEventArgs e) { }

  //      public void MouseMove(MouseEventArgs e)
		//{
		//	if (Mouse.Captured == null) return;
			

		//	// The ClassView is gotten from the mouse event.
		//	var shape = TargetShape(e);
  //          var comment = TargetComment(e);
				
		//	// The mouse position relative to the target of the mouse event.
		//	var mousePosition = RelativeMousePosition(e);
            
  //          // draw connection 
		//	if ((action & Action.AddingConnection) > 0)
		//	{
		//		NewConnectionData.updatePoints(mousePosition);
		//	}
  //          // resize or move ClassView
		//	else if ((action & Action.Resizing) > 0)
		//	{   // resizing
		//		ResizeShape(shape, mousePosition);
		//	}
		//	else if ((action & Action.MovingClass) > 0)
		//	{
		//	    // move ClassView if class in focus is selected, otherwise it is expected that a selection is about to be made
		//	    if (SelectedClasses.Contains(shape) || SelectedComments.Contains(comment))
		//	    {
		//	        MoveShape(mousePosition);
		//	        action = action | Action.NoSelecting;
		//	    }
		//	    else action = action | Action.Selecting;
		//	}
		//	else action = action | Action.Selecting;
		//}

        public void MouseUpClass(MouseButtonEventArgs e)
        {
            if (e == null) return;
            e.MouseDevice.Target?.ReleaseMouseCapture();
            var classObj = TargetShape(e);

            // The mouse position relative to the target of the mouse event.
            var mousePosition = RelativeMousePosition(e);
            // The mouse is released, as the move operation is done, so it can be used by other controls.
  

            if ((ButtonDown & ButtonCommand.Connection) > 0 && (action & Action.AddingConnection) > 0)
            {
                AddConnection(classObj);
            }
            //else if((ButtonDown & ButtonCommand.Shape) > 0 && (action & Action.AddingClass) > 0) // adding shape is made in MouseClick
            else if ((action & Action.Resizing) > 0) ResizeShapeDone(classObj, mousePosition);
            else if ((action & Action.MovingClass) > 0 && SelectedClasses.Contains(classObj)) MoveShapeDone(mousePosition);
            if ((action & Action.Selecting) > 0 && (action & Action.NoSelecting) == 0)
            {
                SelectShape(classObj);
            }

            if (DateTime.Now.Ticks - _doubleClickTimer < doubleClickTimeout)
            {
                if (classObj != null) EditClassContent(classObj);
            }
            _doubleClickTimer = DateTime.Now.Ticks;

            action = Action.Nothing;
        }

        public void MouseUpComment(MouseButtonEventArgs e)
        {
            e.MouseDevice.Target.ReleaseMouseCapture();
            var comment = TargetComment(e);

            // The mouse position relative to the target of the mouse event.
            var mousePosition = RelativeMousePosition(e);
            // The mouse is released, as the move operation is done, so it can be used by other controls.
            e.MouseDevice.Target.ReleaseMouseCapture();

           
            if ((action & Action.Resizing) > 0) ResizeShapeDone(comment, mousePosition);
            else if ((action & Action.MovingClass) > 0 && SelectedComments.Contains(comment)) MoveShapeDone(mousePosition);
            if ((action & Action.Selecting) > 0 && (action & Action.NoSelecting) == 0)
            {
                SelectComment(comment);
            }

            if (DateTime.Now.Ticks - _doubleClickTimer < doubleClickTimeout)
            {
                if (comment != null) EditClassContent(comment);
            }
            _doubleClickTimer = DateTime.Now.Ticks;

            action = Action.Nothing;
        }
        public void MouseUpConnection(MouseButtonEventArgs e) { }
	   
  //      public void MouseUp(MouseButtonEventArgs e)
		//{
		//	// The ClassView is gotten from the mouse event.
  //          e.MouseDevice.Target.ReleaseMouseCapture();
		//	var classData = TargetShape(e);
  //          var comment = TargetComment(e);
  //          var connection = TargetConnection(e); 

  //          // The mouse position relative to the target of the mouse event.
  //          var mousePosition = RelativeMousePosition(e);
  //          // The mouse is released, as the move operation is done, so it can be used by other controls.
  //          e.MouseDevice.Target.ReleaseMouseCapture();

  //          if((ButtonDown & ButtonCommand.Connection) > 0 && (action & Action.AddingConnection) > 0)
  //          //if (!ButtonDown.Equals(ButtonCommand.None) && NewConnectionData != null)
		//	{
  //              AddConnection(classData);
		//	}
		//	//else if((ButtonDown & ButtonCommand.Shape) > 0 && (action & Action.AddingClass) > 0) // adding shape is made in MouseClick
  //          else if((action & Action.Resizing) > 0) ResizeShapeDone((ClassView)classData==null? (ClassView) comment : classData, mousePosition);
  //          else if((action & Action.MovingClass) > 0 && SelectedClasses.Contains(classData)) MoveShapeDone(mousePosition);
  //          if ((action & Action.Selecting) > 0  && (action & Action.NoSelecting) == 0)
  //          //else
  //          {

  //              SelectShape(classData);
  //              SelectComment(comment);
  //              SelectConnection(connection);
  //          }


  //          if (DateTime.Now.Ticks - _doubleClickTimer < doubleClickTimeout)
  //          {
  //              if(classData != null)EditClassContent(classData);
  //          }
  //          _doubleClickTimer = DateTime.Now.Ticks;

  //          action = Action.Nothing;
		//}

	    public void MouseMoveShape(MouseEventArgs e)
	    {
            if (ButtonDown != ButtonCommand.None) return;
        }

        public void MouseUpShape(MouseButtonEventArgs e)
        {
            if (ButtonDown != ButtonCommand.None) return;
        }

        public void CherryPick(MouseEventArgs e)
		{
			var cmd = TargetCommand(e);
			CommandController.SetActiveCommand(cmd);
		}
		
		public void MouseClicked(MouseEventArgs e)
		{

		    if (!ButtonDown.Equals(ButtonCommand.None))
		    {
		        if (e == null) return;
		        var mousePosition = RelativeMousePosition(e);

		        Point anchorpoint = new Point(mousePosition.X - minShapeWidth/2, mousePosition.Y - minShapeHeight/2);

		        // default is normal class/comment --- comment should perhaps have some modifications
		        string stereoType = "";
		        bool isAbstract = false;

		        switch (ButtonDown)
		        {
		            case ButtonCommand.Abstract:
		                isAbstract = true;
		                break;
		            case ButtonCommand.Interface:
		                stereoType = "interface";
		                break;
		            case ButtonCommand.Class:
		                break;
		            case ButtonCommand.Comment:
                        execCommand(new AddCommentCommand(mousePosition));
		                return;
		            default:
		                return;
		        }


		        execCommand(new AddClassCommand(null, stereoType, isAbstract, anchorpoint));
		        // if shift is down, you can add several classes
		        if (!_multiAdd) ButtonDown = ButtonCommand.None;

		    }
		    else
		    {
                if(TargetShape(e) == null)ClearSelectedShapes();
		    }
			
		}

		private void StartNewProject()
		{
			bool clear = false;
			if (ShapeCollector.GetI().ObsConnections.Any() || ShapeCollector.GetI().ObsShapes.Any() || Commands.Any())
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
				CommandController = new CommandTree();
				ShapeCollector.GetI().ObsConnections.Clear();
				ShapeCollector.GetI().ObsShapes.Clear();
				ShapeCollector.GetI().Commands.Clear();
                ShapeCollector.GetI().treeArrows.Clear();
                ShapeCollector.GetI().MaxBranchLayer.Clear();
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
					CommandController = CommandTree.Load(reader);
				}
				fileStream.Close();
			}
		}
		
		private void saveFileCmd()
		{
			saveFile();
		}

	    private void generateCode()
	    {
           
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "choose folder for generated files";

            fbd.ShowDialog();
            CodeGen.GenerateJavaCode(fbd.SelectedPath);
        }

		private bool saveFile()
		{
            bool saved = false;
            SaveFileDialog sfd = new SaveFileDialog();
			sfd.Filter = "XML files (*.xml)|*.xml";
			sfd.Title = "Save diagram as xml";
			sfd.ShowDialog();

			// If the file name is not an empty string open it for saving.
			if (sfd.FileName == "") return saved;
			System.IO.FileStream filestream = (System.IO.FileStream)sfd.OpenFile();
			   
			CommandTree.AsyncSave(CommandController, filestream);
			saved = true;
			return saved;
		}

        private void Help()
        {

            string msg =      "CTRL+S : save file\n"
                            + "CTRL+O : load file\n"
                            + "CTRL+Z : Undo\n"
                            + "CTRL+Y : Redo\n"
                            + "CTRL+Right click : select multiple classes\n"
                            + "CTRL+C : copy selected objects. Connections not included\n"
                            + "CTRL+V : paste the copied objects\n"
                            + "LSHIFT/RSHIFT : allows to add multiple classes/connections without selecting inbetween in the toolbox\n"
                            + "ESC : clear selections\n"
                            + "DEL : delete selected objects. Connections can only be deleted by deleting one of the classes connected to it\n"
                            + "Double click : edit content of class\n"
                            + "Click on number in command tree to browse history";
            MessageBox.Show(msg, "available shortcuts");
        }

		private void AddComposition()
		{
			ButtonDown = (ButtonDown.Equals(ButtonCommand.Composition) ? ButtonCommand.None : ButtonCommand.Composition);
		}

        private void AddInheritance()
        {
            ButtonDown = (ButtonDown.Equals(ButtonCommand.Inheritance) ? ButtonCommand.None : ButtonCommand.Inheritance);
        }

        private void AddAssociation()
		{
			ButtonDown = (ButtonDown.Equals(ButtonCommand.Association) ? ButtonCommand.None : ButtonCommand.Association);

		}

		private void AddAggregation()
		{
			ButtonDown = (ButtonDown.Equals(ButtonCommand.Aggregation) ? ButtonCommand.None : ButtonCommand.Aggregation);
		}

		private void AddClass()
		{
			ButtonDown = (ButtonDown.Equals(ButtonCommand.Class) ? ButtonCommand.None : ButtonCommand.Class);
		}

		private void AddAbstract()
		{
			ButtonDown = (ButtonDown.Equals(ButtonCommand.Abstract) ? ButtonCommand.None : ButtonCommand.Abstract);
		}

		private void AddInterface()
		{
			ButtonDown = (ButtonDown.Equals(ButtonCommand.Interface) ? ButtonCommand.None : ButtonCommand.Interface);
		}

		private void AddComment()
		{
			ButtonDown = (ButtonDown.Equals(ButtonCommand.Comment) ? ButtonCommand.None : ButtonCommand.Comment);
		}

		private BaseCommand TargetCommand(MouseEventArgs e)
		{
			// Here the visual element that the mouse is captured by is retrieved.
			var shapeVisualElement = (FrameworkElement)e.MouseDevice.Target;
			// From the shapes visual element, the ClassView object which is the DataContext is retrieved.
			return shapeVisualElement.DataContext as BaseCommand;
		}

	    private ConnectionData TargetConnection(MouseEventArgs e)
	    {
            // Here the visual element that the mouse is captured by is retrieved.
            var shapeVisualElement = (FrameworkElement)e.MouseDevice.Target;
	        // From the shapes visual element, the ClassData object which is the DataContext is retrieved.
            return shapeVisualElement?.DataContext as ConnectionData;
        }

		private ClassData TargetShape(MouseEventArgs e)
		{
			// Here the visual element that the mouse is captured by is retrieved.
			var shapeVisualElement = (FrameworkElement)e.MouseDevice.Target;
		
			// From the shapes visual element, the ClassData object which is the DataContext is retrieved.
			return shapeVisualElement?.DataContext as ClassData;
		}

        private Comment TargetComment(MouseEventArgs e)
        {
            // Here the visual element that the mouse is captured by is retrieved.
            var shapeVisualElement = (FrameworkElement)e.MouseDevice.Target;

            // From the shapes visual element, the ClassData object which is the DataContext is retrieved.
            return shapeVisualElement?.DataContext as Comment;
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
            else if (shapeVisualElement is Grid)
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

		public void execCommand(BaseCommand command)
		{
			CommandController.AddAndExecute(command);
			
		}

	
	}
}