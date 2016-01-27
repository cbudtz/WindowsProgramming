using Area51.SoftwareModeler.Models.Commands;
using Area51.SoftwareModeler.Models;
using System.Collections.ObjectModel;
using System.Windows;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;


using System.ComponentModel;
using System.Data;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.IO.Packaging;
using System.IO;
using System.Windows.Xps.Packaging;
using System.Printing;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.Printing;
using System.Windows.Xps;

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

	    protected double InitialWidth;
        private const double MinShapeWidth = 150;
        private const double MinShapeHeight = 100;
		

	    private ConnectionData NewConnectionData { get; set; }

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
        public ICommand CollapseNodeCommand { get; }

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

        public ICommand ResizeCommandTreeWindowCommand { get; }



        #endregion
        private List<ClassData> _selectedClasses = new List<ClassData>();
        private List<Comment> _selectedComments = new List<Comment>(); 
        private List<ConnectionData> _selectedConnections = new List<ConnectionData>(); 
        private readonly List<ClassData> _copiedClasses = new List<ClassData>();
        public List<Comment> CopiedComments { get; }
        public List<ConnectionData> CopiedConnection { get; }

        private ClassData _classDataToEdit;
        private bool _multiSelect;
        private bool _multiAdd;
        private Action _action = Action.Nothing;

		// Saves the initial point that the mouse has during a move operation.
		private Point _initialMousePosition;
        // Saves the initial point that the ClassDataRep has during a move operation.
        //private List<Point> _initialClassPosition = new List<Point>();
		private long _doubleClickTimer;
		private long doubleClickTimeout = 500*10000; // nanosec. is 500msec
        
        //new collection for the lines in the command-tree.
        public ObservableCollection<LineCommandTree> TreeArrows => ShapeCollector.GetI().treeArrows;
        //private ClassData _classDataToEdit = null;
		public ObservableCollection<BaseCommand> Commands => ShapeCollector.GetI().Commands;
	    public ObservableCollection<ClassData> Classes => ShapeCollector.GetI().ObsShapes;
        public ObservableCollection<Comment> Comments => ShapeCollector.GetI().ObsComments; 
	    public ObservableCollection<ConnectionData> Connections => ShapeCollector.GetI().ObsConnections;


		//Dynamic 
		private CommandTree CommandController { get; set; }

		/// <summary>
		/// Initializes a new instance of the MainViewModel class.
		/// </summary>
		public MainViewModel()
		{
		    CopiedComments = new List<Comment>();
		    CopiedConnection = new List<ConnectionData>();

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
            CollapseNodeCommand = new RelayCommand<MouseEventArgs>(CollapseNode);

            KeyEventCommand = new RelayCommand<KeyEventArgs>(KeyEvent);
			MouseClickCommand = new RelayCommand<MouseEventArgs>(MouseClicked);

			SaveCommand = new RelayCommand(SaveFileCmd);
			LoadCommand = new RelayCommand(LoadFile);
			NewCommand = new RelayCommand(StartNewProject);
			TeamCommand = new RelayCommand(StartTeamProject);
            GenerateCodeCommand = new RelayCommand(GenerateCode);
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
            KeyStates a = e.KeyStates & e.KeyboardDevice.GetKeyStates(Key.A) & KeyStates.Down;

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
					SaveFile();
				}
				else if (o > 0){
					LoadFile();
				}else if(c > 0)
                {
                    _copiedClasses.Clear();
                    _selectedClasses.ForEach(x => _copiedClasses.Add(x));
                }else if(v > 0)
                {
                    _copiedClasses.ForEach(x => ExecCommand(new CopyClassCommand(x)));
                }else if (a > 0)
                {
                    SelectAll();
                }
			}
			else if (esc > 0)
			{
				ClearSelectedShapes();
				ButtonDown = ButtonCommand.None;
			}
			else if(del > 0 && _selectedClasses != null)
			{
				_selectedClasses.ForEach(x => ExecCommand(new DeleteShapeCommand(x)));
                _selectedConnections.ForEach(x => ExecCommand(new DeleteConnectionCommand(x)));
				ClearSelectedShapes();
			}
            else if (shiftR > 0 || shiftL > 0)
            {
                _multiAdd = true;
		}
		}

		public void EditClassContent(ClassView classView)
		{

            EditClassWindow?.Close();

            EditClassWindow = new EditClassPopupWindow();
            _classDataToEdit = (ClassData)classView;
            EditClassWindow.ClassName.Text = _classDataToEdit.Name;
            string stereoType = _classDataToEdit.StereoType;
            stereoType = stereoType == null || stereoType.Length < 4? "" : stereoType.Remove(0, 2);
            stereoType = stereoType.Length < 2? "" : stereoType.Remove(stereoType.Length - 2, 2);
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
		  var methods =   EditClassWindow.Methods.Items.OfType<Method>().ToList(); // as List<Method>;

			var attributes = EditClassWindow.Attributes.Items.OfType<Models.Attribute>().ToList();

			ExecCommand(new UpdateClassInfoCommand(_classDataToEdit, EditClassWindow.ClassName.Text, EditClassWindow.StereoType.Text, EditClassWindow.IsAbstract.IsChecked, methods, attributes));
			
			_classDataToEdit = null;
			ClearSelectedShapes();
            EditClassWindow.Hide();
        }

        public void EditClassCancel()
        {
            _classDataToEdit = null;
            EditClassWindow.Visibility = Visibility.Collapsed;
        }

        #region select and deselect


        public void SelectAll()
        {
            ClearSelectedShapes();
            foreach (var classData in Classes)
            {
                SelectShape(classData);
            }
            foreach (var comment in Comments)
            {
                SelectComment(comment); 
            }
            foreach (var connectionData in Connections)
            {
                SelectConnection(connectionData);
            }
        }

        public void ClearSelectedShapes()
		{
			_selectedClasses.ForEach(x => x.IsSelected = false);
            _selectedComments.ForEach(x => x.IsSelected = false);
            _selectedConnections.ForEach(x => x.IsSelected = false);
            
			_selectedClasses.Clear();
            _selectedComments.Clear();
            _selectedConnections.Clear();
		}

        public void SelectComment(Comment c)
        {
            if (c == null) return;
            if (_selectedComments == null) _selectedComments = new List<Comment>();

            if (!_multiSelect) ClearSelectedShapes();

            if (!_selectedComments.Contains(c))
            {
                c.IsSelected = true;
                _selectedComments.Add(c);

                c.InitialPosition = new Point(c.X, c.Y);
            }
            else
            {
                c.IsSelected = false;
                _selectedComments.Remove(c);
            }
        }

        public void DeSelectComment(Comment c)
        {
            if (c == null || _selectedComments == null) return;

            if (!_multiSelect) ClearSelectedShapes();

            if (!_selectedComments.Contains(c))
            {
                c.IsSelected = true;
                _selectedComments.Add(c);

                c.InitialPosition = new Point(c.X, c.Y);
            }
        }
		public void SelectShape(ClassData s)
		{
			if (s == null) return;
			if (_selectedClasses == null) _selectedClasses = new List<ClassData>();
			
			if (!_multiSelect) ClearSelectedShapes();

		    if (!_selectedClasses.Contains(s))
		    {
		        s.IsSelected = true;
		        _selectedClasses.Add(s);

		        s.InitialPosition = new Point(s.X, s.Y);
		    }
            else
            {
                s.IsSelected = false;
                _selectedClasses.Remove(s);
            }
        }

	    public void SelectConnection(ConnectionData c)
	    {
	        if (c == null) return;
            if(_selectedConnections == null) _selectedConnections = new List<ConnectionData>();

            if(!_multiSelect) ClearSelectedShapes();

	        if (!_selectedConnections.Contains(c))
	        {
	            c.IsSelected = true;
	            _selectedConnections.Add(c);
	        }
            else
            {
                c.IsSelected = false;
                _selectedConnections.Remove(c);
            }
        }
        #endregion

#region move shape
        public void MoveShape(Point mousePosition)
		{
		    double xOffset = (mousePosition.X - _initialMousePosition.X);
		    double yOffset = (mousePosition.Y - _initialMousePosition.Y);
            if(xOffset + yOffset > 0) _action = Action.Moving;
            for (int i = 0; i < _selectedClasses.Count; i++) {

                _selectedClasses.ElementAt(i).X = _selectedClasses.ElementAt(i).InitialPosition.X + xOffset;
                _selectedClasses.ElementAt(i).Y = _selectedClasses.ElementAt(i).InitialPosition.Y + yOffset;

                // lambda expr. update all Connections. first Connections where end ClassDataRep is the moving ClassDataRep then where start ClassDataRep is moving ClassDataRep
                Connections.Where(x => x.EndShapeId == _selectedClasses.ElementAt(i).id).ToList().ForEach(x => x.updatePoints());
                Connections.Where(x => x.StartShapeId == _selectedClasses.ElementAt(i).id).ToList().ForEach(x => x.updatePoints());
            }

            for (int i = 0; i < _selectedComments.Count; i++)
            {
                _selectedComments.ElementAt(i).X = _selectedComments.ElementAt(i).InitialPosition.X + xOffset;
                _selectedComments.ElementAt(i).Y = _selectedComments.ElementAt(i).InitialPosition.Y + yOffset;

                // lambda expr. update all Connections. first Connections where end ClassDataRep is the moving ClassDataRep then where start ClassDataRep is moving ClassDataRep
                Connections.Where(x => x.EndShapeId == _selectedComments.ElementAt(i).id).ToList().ForEach(x => x.updatePoints());
                Connections.Where(x => x.StartShapeId == _selectedComments.ElementAt(i).id).ToList().ForEach(x => x.updatePoints());
            }
        }

        public bool MoveShapeDone(Point mousePosition)
        {
            double xOffset = mousePosition.X - _initialMousePosition.X;
            double yOffset = mousePosition.Y - _initialMousePosition.Y;

            if (Math.Abs(xOffset) < 10 && Math.Abs(yOffset) < 10) return false;

            for (int i = 0; i < _selectedClasses.Count; i++) {
                // The ClassView is moved back to its original position, so the offset given to the move command works.
                double x = _selectedClasses.ElementAt(i).InitialPosition.X;
                double y = _selectedClasses.ElementAt(i).InitialPosition.Y;
                _selectedClasses.ElementAt(i).X = x;
                _selectedClasses.ElementAt(i).Y = y;

                _selectedClasses.ElementAt(i).InitialPosition = new Point(x + xOffset, y+yOffset);

                ExecCommand(new MoveShapeCommand(_selectedClasses.ElementAt(i), xOffset, yOffset));
            }
            

            for (int i = 0; i < _selectedComments.Count; i++)
            {
                // The ClassView is moved back to its original position, so the offset given to the move command works.
                double x = _selectedComments.ElementAt(i).InitialPosition.X;
                double y = _selectedComments.ElementAt(i).InitialPosition.Y;
                _selectedComments.ElementAt(i).X = x;
                _selectedComments.ElementAt(i).Y = y;

                _selectedComments.ElementAt(i).InitialPosition = new Point(x + xOffset, y + yOffset);

                ExecCommand(new MoveShapeCommand(_selectedComments.ElementAt(i), xOffset, yOffset));
            }
            return true;
        }
#endregion

        #region resizing
        public bool ResizeShapeInit(ClassView @class, MouseButtonEventArgs e)
        {
            if (@class == null) return false;
            double borderX = @class.X + @class.Width;
            double borderY = @class.Y + @class.Height;

            // The mouse position relative to the target of the mouse event.
            var mousePosition = RelativeMousePosition(e);
            if (Math.Abs(mousePosition.X - borderX) > 5 && Math.Abs(mousePosition.Y - borderY) > 5) return false;

            var shape = TargetShape(e);
            if (shape == null) return false;

            _initialMousePosition = mousePosition;

            InitialWidth = shape.Width;
            ClearSelectedShapes();
            return true;
        }

		public void ResizeShape(ClassView classView, Point mousePosition)
		{
            classView.Width = mousePosition.X - classView.X;
			classView.Height = mousePosition.Y - classView.Y;
			if (Math.Abs(classView.Width) < MinShapeWidth) classView.Width = MinShapeWidth;
			if (Math.Abs(classView.Height) < MinShapeHeight) classView.Height = MinShapeHeight;
		}

        public void ResizeShapeDone(ClassView shape, Point mousePosition)
        {
            if (_action != Action.Resizing) return;          
            if (shape == null) return;
           
            // The ClassView is moved back to its original position, so the offset given to the move command works.
            shape.Width = _initialMousePosition.X - shape.X;
            shape.Height = _initialMousePosition.Y - shape.Y;

            double xOffset = mousePosition.X - _initialMousePosition.X;
            double yOffset = mousePosition.Y - _initialMousePosition.Y;

            if (Math.Abs(shape.Width + xOffset) < MinShapeWidth) xOffset = MinShapeWidth - shape.Width;
            if (Math.Abs(shape.Height + yOffset) < MinShapeHeight) yOffset = MinShapeHeight - shape.Height;

            ExecCommand(new ResizeShapeCommand(shape, xOffset, yOffset));
        }
        #endregion

        #region addconnection
        public void AddConnection(ClassData shape)
            {
            if (shape != null && NewConnectionData.StartShapeId != shape.id)
            {
                NewConnectionData.EndShapeId = shape.id;
                    ExecCommand(new AddConnectionCommand(NewConnectionData.ConnectionId, NewConnectionData.StartShapeId, "", shape.id ?? 0, "", NewConnectionData.Type));
                if (!_multiAdd) ButtonDown = ButtonCommand.None;

            }
            Connections.Remove(NewConnectionData);
            NewConnectionData = null;
        }
      
        public void AddConnectionInit(ClassData shape, Point mousePosition)
        {
            ConnectionType type;
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

            NewConnectionData = new ConnectionData(shape.id ?? 0, "", null, "", type) {EndPoint = mousePosition};
            Connections.Add(NewConnectionData);
        }
        #endregion

        
        #region mouse down
        public void MouseDownClass(MouseButtonEventArgs e)
        {
            var classObj = TargetShape(e);
            var mousePosition = RelativeMousePosition(e);
            _initialMousePosition = mousePosition;

            if ((ButtonDown & ButtonCommand.Connection) > 0)
            {
                AddConnectionInit(classObj, mousePosition);
                _action = Action.AddingConnection | Action.NoSelecting;
            }
            else
            {
                // if cursor is close enough to right/bottom border, we want to resize
                // therefore resize and move is not set at the same time 
                if (ResizeShapeInit(classObj, e))
                {
                    _action = Action.Resizing | Action.NoSelecting;
                }
                else
                {
                    _action = Action.MovingClass | Action.Selecting;
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
                _action = Action.Resizing | Action.NoSelecting;
            }
            else
            {
                _action = Action.MovingComment | Action.Selecting;
            }
            e.MouseDevice.Target.CaptureMouse();
        }

        public void MouseDownConnection(MouseButtonEventArgs e)
        {
            var conn = TargetConnection(e);
            if (conn == null) return;

            SelectConnection(conn);
        }

        //public void MouseDown(MouseButtonEventArgs e)
        //{

        //    var shape = TargetShape(e);
        //    var connection = TargetConnection(e);
        //    if (shape != null || connection != null) _action = _action | Action.Selecting;

        //    // The mouse position relative to the target of the mouse event.
        //    var mousePosition = RelativeMousePosition(e);
        //    _initialMousePosition = mousePosition;

        //    if((ButtonDown & ButtonCommand.Connection) > 0) { 
        //        AddConnectionInit(shape, mousePosition);
        //        _action = _action | Action.AddingConnection;
        //    }
        //    else if ((ButtonDown & ButtonCommand.Shape) > 0)
        //    {
        //        // no class initialization needed
        //        _action = _action | Action.AddingClass;
        //    }
        //    else
        //    {
        //        // if cursor is close enough to right/bottom border, we want to resize
        //        // therefore resize and move is not set at the same time 
        //        if (ResizeShapeInit(shape, e))
        //        {
        //            _action = _action | Action.Resizing;
        //        }
        //        else
        //        {
        //            _action = _action | Action.Moving; // can only move class at the moment
        //        }
        //    }
        //    e.MouseDevice.Target.CaptureMouse();
        //}
        #endregion


        #region mouse move
        public void MouseMoveClass(MouseEventArgs e)
        {
            
            if (Mouse.Captured == null) return;
            
            var classObj = TargetShape(e);
            var mousePosition = RelativeMousePosition(e);

            // draw connection 
            if ((_action & Action.AddingConnection) > 0)
            {
                NewConnectionData.updatePoints(mousePosition);
                _action = _action | Action.NoSelecting;
            }
            // resize or move ClassView
            else if ((_action & Action.Resizing) > 0)
            {   // resizing
                ResizeShape(classObj, mousePosition);
                _action = _action | Action.NoSelecting;
            }
            else if ((_action & Action.MovingClass) > 0) // if moving class
            {
                // move ClassView if class in focus is selected, otherwise it is expected that a selection is about to be made
                if (!_selectedClasses.Contains(classObj))
                {
                    SelectShape(classObj);
                }
                MoveShape(mousePosition);
            }
        }

        public void MouseMoveComment(MouseEventArgs e)
        {
            if (Mouse.Captured == null) return;

            var comment = TargetComment(e);
            var mousePosition = RelativeMousePosition(e);

            // draw connection 
            if ((_action & Action.AddingConnection) > 0)
            {
                NewConnectionData.updatePoints(mousePosition);
            }
            // resize or move ClassView
            else if ((_action & Action.Resizing) > 0)
            {   // resizing
                ResizeShape(comment, mousePosition);
            }
            else if ((_action & Action.MovingComment) > 0)
            {
                // move ClassView if class in focus is selected, otherwise it is expected that a selection is about to be made
                if (!_selectedComments.Contains(comment))
                {
                    SelectComment(comment);
                }
                MoveShape(mousePosition);
            }
            else _action = _action | Action.Selecting;
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
        //	if ((_action & Action.AddingConnection) > 0)
        //	{
        //		NewConnectionData.updatePoints(mousePosition);
        //	}
        //          // resize or move ClassView
        //	else if ((_action & Action.Resizing) > 0)
        //	{   // resizing
        //		ResizeShape(shape, mousePosition);
        //	}
        //	else if ((_action & Action.MovingClass) > 0)
        //	{
        //	    // move ClassView if class in focus is selected, otherwise it is expected that a selection is about to be made
        //	    if (_selectedClasses.Contains(shape) || _selectedComments.Contains(comment))
        //	    {
        //	        MoveShape(mousePosition);
        //	        _action = _action | Action.NoSelecting;
        //	    }
        //	    else _action = _action | Action.Selecting;
        //	}
        //	else _action = _action | Action.Selecting;
        //}
        #endregion

        #region mouse up
        public void MouseUpClass(MouseButtonEventArgs e)
        {
            if (e == null) return;
            e.MouseDevice.Target?.ReleaseMouseCapture();
            var classObj = TargetShape(e);

            // The mouse position relative to the target of the mouse event.
            var mousePosition = RelativeMousePosition(e);
            // The mouse is released, as the move operation is done, so it can be used by other controls.
            bool movedShape = false;

            if ((ButtonDown & ButtonCommand.Connection) > 0 && (_action & Action.AddingConnection) > 0)
            {
                AddConnection(classObj);
            }
            //else if((ButtonDown & ButtonCommand.Shape) > 0 && (_action & Action.AddingClass) > 0) // adding shape is made in MouseClick
            else if ((_action & Action.Resizing) > 0)
            {
                ResizeShapeDone(classObj, mousePosition);
            }
            else if ((_action & Action.MovingClass) > 0 && _selectedClasses.Contains(classObj))
            {
                movedShape = MoveShapeDone(mousePosition);
            }
            
            if ((_action & Action.Selecting) > 0 && (_action & Action.NoSelecting) == 0)
            {
                 if(!movedShape) SelectShape(classObj);
            }

            if (DateTime.Now.Ticks - _doubleClickTimer < doubleClickTimeout)
            {
                if (classObj != null) EditClassContent(classObj);
            }
            _doubleClickTimer = DateTime.Now.Ticks;

            _action = Action.Nothing;
        }

        public void MouseUpComment(MouseButtonEventArgs e)
        {
            e.MouseDevice.Target.ReleaseMouseCapture();
            var comment = TargetComment(e);

            // The mouse position relative to the target of the mouse event.
            var mousePosition = RelativeMousePosition(e);
            // The mouse is released, as the move operation is done, so it can be used by other controls.
            e.MouseDevice.Target.ReleaseMouseCapture();

           
            if ((_action & Action.Resizing) > 0) ResizeShapeDone(comment, mousePosition);
            else if ((_action & Action.MovingClass) > 0 && _selectedComments.Contains(comment)) MoveShapeDone(mousePosition);
            if ((_action & Action.Selecting) > 0 && (_action & Action.NoSelecting) == 0)
            {
                SelectComment(comment);
            }

            if (DateTime.Now.Ticks - _doubleClickTimer < doubleClickTimeout)
            {
                if (comment != null) EditClassContent(comment);
            }
            _doubleClickTimer = DateTime.Now.Ticks;

            _action = Action.Nothing;
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

        //          if((ButtonDown & ButtonCommand.Connection) > 0 && (_action & Action.AddingConnection) > 0)
        //          //if (!ButtonDown.Equals(ButtonCommand.None) && NewConnectionData != null)
        //	{
        //              AddConnection(classData);
        //	}
        //	//else if((ButtonDown & ButtonCommand.Shape) > 0 && (_action & Action.AddingClass) > 0) // adding shape is made in MouseClick
        //          else if((_action & Action.Resizing) > 0) ResizeShapeDone((ClassView)classData==null? (ClassView) comment : classData, mousePosition);
        //          else if((_action & Action.MovingClass) > 0 && _selectedClasses.Contains(classData)) MoveShapeDone(mousePosition);
        //          if ((_action & Action.Selecting) > 0  && (_action & Action.NoSelecting) == 0)
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

        //          _action = Action.Nothing;
        //}
#endregion


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

        public void CollapseNode(MouseEventArgs e)
        {
            var cmd = TargetCommand(e);
            if (cmd == null) return;

            Console.WriteLine("collapsing node");
            cmd.CollapseNodes();
        }
		
		public void MouseClicked(MouseEventArgs e)
		{
            
		    if (!ButtonDown.Equals(ButtonCommand.None))
		    {
		        if (e == null) return;
		        var mousePosition = RelativeMousePosition(e);

		        Point anchorpoint = new Point(mousePosition.X - MinShapeWidth/2, mousePosition.Y - MinShapeHeight/2);

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
                        ExecCommand(new AddCommentCommand(mousePosition));
		                return;
		            default:
		                return;
		        }


		        ExecCommand(new AddClassCommand(null, stereoType, isAbstract, anchorpoint));
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
					clear = SaveFile();
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
			}
		}

		private static void StartTeamProject()
		{
			MessageBox.Show("This feature is not implemented yet");
		}

		private void LoadFile()
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
		
		private void SaveFileCmd()
		{
			SaveFile();
		    
        }

	    private void GenerateCode()
	    {
           
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "choose folder for generated files";

            fbd.ShowDialog();
            CodeGen.GenerateJavaCode(fbd.SelectedPath);
        }

		private bool SaveFile()
		{
            SaveFileDialog sfd = new SaveFileDialog();
			sfd.Filter = "XML files (*.xml)|*.xml";
			sfd.Title = "Save diagram as xml";
			sfd.ShowDialog();

			// If the file name is not an empty string open it for saving.
			if (sfd.FileName == "") return false;
			FileStream filestream = (FileStream)sfd.OpenFile();
			   
			CommandTree.AsyncSave(CommandController, filestream);

            if (canvas == null)
            {
                Console.WriteLine("canvas is null");
                return true;
            }
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)canvas.ActualWidth,
    (int)canvas.ActualHeight, 96d, 96d, System.Windows.Media.PixelFormats.Default);
            rtb.Render(canvas);

            var crop = new CroppedBitmap(rtb, new Int32Rect(0, 0, (int) canvas.ActualWidth, (int) canvas.ActualHeight));

            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(crop));

            using (var fs = System.IO.File.OpenWrite("C:/Users/Runi/Desktop/logo.png"))
            {
                pngEncoder.Save(fs);
            }

            try
            {
                //System.Windows.Controls.
                System.Windows.Controls.PrintDialog dialog = new System.Windows.Controls.PrintDialog();

                if (dialog.ShowDialog() != true)
                    return true;
                dialog.PrintVisual(canvas, "IFMS Print Screen");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Print Screen", MessageBoxButton.OK, MessageBoxImage.Error);
            }


            
                // create XPS file based on a WPF Visual, and store it in a memorystream
                //MemoryStream lMemoryStream = new MemoryStream();
                //Package package = Package.Open(lMemoryStream, FileMode.Create);
                //XpsDocument doc = new XpsDocument(package);
                //XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);
                //writer.Write(canvas);
                //doc.Close();
                //package.Close();

            
        
            return true;
		}

        static Visual CreateVisual()
        {
            const double Inch = 96;
            DrawingVisual visual = new DrawingVisual();
            DrawingContext dc = visual.RenderOpen();
            Pen bluePen = new Pen(Brushes.Blue, 1);
            dc.DrawRectangle(Brushes.Yellow, bluePen, new Rect(Inch / 2, Inch / 2, Inch * 1.5, Inch * 1.5));
            Brush pinkBrush = new SolidColorBrush(Color.FromArgb(128, 255, 0, 255));
            Pen blackPen = new Pen(Brushes.Black, 1);
            dc.DrawEllipse(pinkBrush, blackPen, new Point(Inch * 2.25, Inch * 2), Inch * 1.25, Inch);
            dc.Close();
            return visual;
        }

        private static void Help()
        {
            const string msg = "CTRL+S : save file\n"
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

	    //private ConnectionData TargetConnection(MouseEventArgs e)
	    //{
     //       // Here the visual element that the mouse is captured by is retrieved.
     //       var shapeVisualElement = (FrameworkElement)e.MouseDevice.Target;
	    //    // From the shapes visual element, the ClassData object which is the DataContext is retrieved.
     //       return shapeVisualElement?.DataContext as ConnectionData;
     //   }

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

        private Canvas canvas;
        // Gets the mouse position relative to the canvas.
        private Point RelativeMousePosition(MouseEventArgs e)
		{
			// Here the visual element that the mouse is captured by is retrieved.
			var shapeVisualElement = (FrameworkElement)e.MouseDevice.Target;
			// The canvas holding the shapes visual element, is found by searching up the tree of visual elements.
			if (shapeVisualElement is Canvas)
			{
                this.canvas = shapeVisualElement as Canvas;
                return Mouse.GetPosition(shapeVisualElement);

			}
            if (shapeVisualElement is Grid)
            {
                return Mouse.GetPosition(shapeVisualElement);
            }
            var canvas = FindParentOfType<Canvas>(shapeVisualElement);
            this.canvas = canvas;
            return Mouse.GetPosition(canvas);
            // The mouse position relative to the canvas is gotten here.

		}
		private static T FindParentOfType<T>(DependencyObject o)
		{
			
			dynamic parent = o == null ? null : VisualTreeHelper.GetParent(o);
			if (parent == null) return parent;
		    try
		    {
		        return parent.GetType().IsAssignableFrom(typeof (T)) ? parent : FindParentOfType<T>(parent);
		    }
		    catch (Exception e)
		    {
                Console.WriteLine(o.DependencyObjectType + ";" + parent);
		        return default(T);
		    }
		}

		public void ExecCommand(BaseCommand command)
		{
			CommandController.AddAndExecute(command);
			
		}

	
	}
}