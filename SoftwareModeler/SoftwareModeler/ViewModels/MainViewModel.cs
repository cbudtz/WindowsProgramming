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
using Area51.SoftwareModeler.Views;
using Visibility = System.Windows.Visibility;

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
	public enum ButtonCommand { None, Class, Abstract, Interface, Comment, Association, Aggregation, Composition}
	
	public class MainViewModel : ViewModelBase
	{
	    private EditClassPopupWindow EditClassWindow { get; set; } = new EditClassPopupWindow();

	    public ButtonCommand ButtonDown { get; private set; } = ButtonCommand.None;

	    protected double InitialWidth = 0;
		private double minShapeWidth = 150;
		private double minShapeHeight = 100;
		public bool IsEditable { get; set; } = false;
		

	    private Connection NewConnection { get; set; } = null;

		public string Text { get; set; }

		#region command variables
		// Commands that the UI can be bound to.
		// Shapes
		public ICommand ClearSelectionCommand { get; }
		public ICommand EditClassContentCancelCommand { get; }
		public ICommand EditClassContentOkCommand { get; }

		public ICommand MouseDownShapeCommand { get; }
		public ICommand MouseMoveShapeCommand { get; }
		public ICommand MouseUpShapeCommand { get; }

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
        public ICommand HelpCommand { get; }

		public ICommand AddAssociationCommand { get; }
		public ICommand AddAggregationCommand { get; }
		public ICommand AddCompositionCommand { get; }

		public ICommand NewClassCommand { get; }
		public ICommand NewAbstractCommand { get; }
		public ICommand NewInterfaceCommand { get; }
		public ICommand NewCommentCommand { get; }


        #endregion
        private List<Class> selectedClasses = new List<Class>();
        private List<Class> classesCopied = new List<Class>();
        private Class _classToEdit = null;
        private bool _multiSelect = false;
        private bool _multiAdd = false;
        private bool _isResizing = false;

		// Saves the initial point that the mouse has during a move operation.
		private Point _initialMousePosition;
        // Saves the initial point that the ClassRep has during a move operation.
        private List<Point> _initialClassPosition = new List<Point>();
		private long _doubleClickTimer;
		private long doubleClickTimeout = 500*10000; // nanosec. is 500msec
        

        //maxbranchlayer added as an observablecollection for now, not a nice fix, but it works (for scroll area).
		public ObservableCollection<int> MaxBranchLayer { get { return ShapeCollector.GetI().MaxBranchLayer; } }
        //new collection for the lines in the command-tree.
        public ObservableCollection<Connection> treeArrows { get { return ShapeCollector.GetI().treeArrows; } }
        private Class classToEdit = null;
		public ObservableCollection<BaseCommand> Commands => ShapeCollector.GetI().Commands;
	    public ObservableCollection<Class> Classes => ShapeCollector.GetI().ObsShapes;
	    public ObservableCollection<Connection> Connections => ShapeCollector.GetI().ObsConnections;


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

			MouseDownShapeCommand = new RelayCommand<MouseButtonEventArgs>(MouseDown);
			MouseMoveShapeCommand = new RelayCommand<MouseEventArgs>(MouseMove);
			MouseUpShapeCommand = new RelayCommand<MouseButtonEventArgs>(MouseUp);

			CherryPickCommand = new RelayCommand<MouseEventArgs>(CherryPick);

			//TODO implement these
			//MouseDownConnectionCommand =
			//MouseMoveConnectionCommand = 
			//MouseUpConnectionCommand =
			 
			KeyEventCommand = new RelayCommand<KeyEventArgs>(KeyEvent);
			MouseClickCommand = new RelayCommand<MouseEventArgs>(MouseClicked);

			SaveCommand = new RelayCommand(saveFileCmd);
			LoadCommand = new RelayCommand(loadFile);
			NewCommand = new RelayCommand(StartNewProject);
			TeamCommand = new RelayCommand(StartTeamProject);
            HelpCommand = new RelayCommand(Help);

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
					CommandController.undo();
				}
				else if (y > 0)
				{
					CommandController.redo();
				}
				else if (s > 0)
				{
					saveFile();
				}
				else if (o > 0){
					loadFile();
				}else if(c > 0)
                {
                    classesCopied.Clear();
                    selectedClasses.ForEach(x => classesCopied.Add(x));
                }else if(v > 0)
                {
                    classesCopied.ForEach(x => execCommand(new CopyClassCommand(x)));
                }
			}
			else if (esc > 0)
			{
				ClearSelectedShapes();
				ButtonDown = ButtonCommand.None;
			}
			else if(del > 0 && selectedClasses != null)
			{
				selectedClasses.ForEach(x => execCommand(new DeleteShapeCommand(x)));
				ClearSelectedShapes();
			}
            else if (shiftR > 0 || shiftL > 0)
            {
                _multiAdd = true;
		}
		}

		public void EditClassContent(Shape shape)
		{
            bool newWind = (EditClassWindow == null ||
                                    EditClassWindow.Visibility == Visibility.Collapsed ||
                                    EditClassWindow.Visibility == Visibility.Hidden ||
                                    !EditClassWindow.IsEnabled);
            if (EditClassWindow != null) EditClassWindow.Close();

            EditClassWindow = new EditClassPopupWindow();
            _classToEdit = (Class)shape;
            EditClassWindow.ClassName.Text = _classToEdit.name;
            string stereoType = _classToEdit.StereoType;
            stereoType = stereoType == null || stereoType.Length < 4? "" : stereoType.Remove(0, 2);
            stereoType = stereoType == null || stereoType.Length < 2? "" : stereoType.Remove(stereoType.Length - 2, 2);
            EditClassWindow.StereoType.Text = stereoType;
            EditClassWindow.IsAbstract.IsChecked = _classToEdit.IsAbstract;
            EditClassWindow.Methods.ItemsSource = _classToEdit.Methods;
            EditClassWindow.Attributes.ItemsSource = _classToEdit.Attributes;
            EditClassWindow.Ok.Command = EditClassContentOkCommand;
            EditClassWindow.Cancel.Command = EditClassContentCancelCommand;
            EditClassWindow.Show();
        }

		public void EditClassOk()
		{
		  var methods =   EditClassWindow.Methods.Items.OfType<Method>().ToList<Method>(); ;// as List<Method>;

			var attributes = EditClassWindow.Attributes.Items.OfType<Models.Attribute>().ToList<Models.Attribute>();

			execCommand(new UpdateClassInfoCommand(_classToEdit, EditClassWindow.ClassName.Text, EditClassWindow.StereoType.Text, EditClassWindow.IsAbstract.IsChecked, methods, attributes));
			
			_classToEdit = null;
			ClearSelectedShapes();
			EditClassWindow.Hide();
		}

        public void EditClassCancel()
        {
            _classToEdit = null;
            EditClassWindow.Visibility = Visibility.Collapsed;
        }

        public void ClearSelectedShapes()
		{
			selectedClasses.ForEach(x => x.IsSelected = false);
			selectedClasses.Clear();
            _initialClassPosition.Clear();
		}

		public void SelectShape(Class s)
		{
			if (s == null) return;
			if (selectedClasses == null) selectedClasses = new List<Class>();
			
			if (!_multiSelect && !selectedClasses.Contains(s)) ClearSelectedShapes();

            if (!selectedClasses.Contains(s) && (selectedClasses.Count < 2 || _multiSelect)) 
            {
                s.IsSelected = true;
                selectedClasses.Add(s);
                _initialClassPosition.Add(new Point(s.X, s.Y));
            }
		}

		public void MoveShape(Point mousePosition)
		{
            for(int i = 0; i < selectedClasses.Count; i++) {
                selectedClasses.ElementAt(i).X = _initialClassPosition.ElementAt(i).X + (mousePosition.X - _initialMousePosition.X);
                selectedClasses.ElementAt(i).Y = _initialClassPosition.ElementAt(i).Y + (mousePosition.Y - _initialMousePosition.Y);

                // lambda expr. update all Connections. first Connections where end ClassRep is the moving ClassRep then where start ClassRep is moving ClassRep
                Connections.Where(x => x.EndShapeId == selectedClasses.ElementAt(i).id).ToList().ForEach(x => x.updatePoints());
                Connections.Where(x => x.StartShapeId == selectedClasses.ElementAt(i).id).ToList().ForEach(x => x.updatePoints());
            }
		}

        public void MoveShapeDone(Point mousePosition)
        {
           
            double xOffset = mousePosition.X - _initialMousePosition.X;
            double yOffset = mousePosition.Y - _initialMousePosition.Y;

            if (Math.Abs(xOffset) < 10 && Math.Abs(yOffset) < 10) return;

            for (int i = 0; i < selectedClasses.Count; i++) {
                // The Shape is moved back to its original position, so the offset given to the move command works.
                selectedClasses.ElementAt(i).X = _initialClassPosition.ElementAt(i).X;
                selectedClasses.ElementAt(i).Y = _initialClassPosition.ElementAt(i).Y;              
                
                //_initialClassPosition.Insert(i, new Point(selectedClasses.ElementAt(i).X + XOffset, selectedClasses.ElementAt(i).Y + YOffset));
                Console.WriteLine("moved done: " + new Point(selectedClasses.ElementAt(i).X + xOffset, selectedClasses.ElementAt(i).Y + yOffset));
              
                execCommand(new MoveShapeCommand(selectedClasses.ElementAt(i), xOffset, yOffset));
            }
            ClearSelectedShapes();
        }

        public void ResizeShapeInit(Shape shape, MouseButtonEventArgs e)
        {
            if (shape == null) return;
            double borderX = shape.X + shape.Width;
            double borderY = shape.Y + shape.Height;

            // The mouse position relative to the target of the mouse event.
            var mousePosition = RelativeMousePosition(e);

            if (Math.Abs(mousePosition.X - borderX) < 5 && Math.Abs(mousePosition.Y - borderY) < 5) return;

            var border = e.MouseDevice.Target as Border;
            if (border == null) return;

            _initialMousePosition = mousePosition;

            InitialWidth = border.ActualWidth;

            _isResizing = true;

            
        }

		public void ResizeShape(Shape shape, Point mousePosition)
		{
			shape.Width = mousePosition.X - shape.X;
			shape.Height = mousePosition.Y - shape.Y;
			if (Math.Abs(shape.Width) < minShapeWidth) shape.Width = minShapeWidth;
			if (Math.Abs(shape.Height) < minShapeHeight) shape.Height = minShapeHeight;
		}

        public void ResizeShapeDone(Shape shape, Point mousePosition)
        {
            if (!_isResizing) return;          
            if (shape == null) return;
           
            // The Shape is moved back to its original position, so the offset given to the move command works.
            shape.Width = _initialMousePosition.X - shape.X;
            shape.Height = _initialMousePosition.Y - shape.Y;

            double xOffset = mousePosition.X - _initialMousePosition.X;
            double yOffset = mousePosition.Y - _initialMousePosition.Y;

            if (Math.Abs(shape.Width + xOffset) < minShapeWidth) xOffset = minShapeWidth - shape.Width;
            if (Math.Abs(shape.Height + yOffset) < minShapeHeight) yOffset = minShapeHeight - shape.Height;

            execCommand(new ResizeShapeCommand(shape, xOffset, yOffset));

            _isResizing = false;
        }

        public void AddConnection(Class shape)
            {
            if (shape != null && NewConnection.StartShapeId != shape.id)
            {
                NewConnection.EndShapeId = shape.id;
                    execCommand(new AddConnectionCommand(NewConnection.StartShapeId ?? 0, "", shape.id ?? 0, "", NewConnection.Type));
                if (!_multiAdd) ButtonDown = ButtonCommand.None;

            }
            Connections.Remove(NewConnection);
            NewConnection = null;
        }
      
        public void AddConnectionInit(Class shape, Point mousePosition)
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
                default:
                    return;
            }
            
            NewConnection = new Connection(shape.id ?? 0, "", null, "", type);
            NewConnection.EndPoint = mousePosition;
            Connections.Add(NewConnection);
        }

        public void MouseMove(MouseEventArgs e)
		{
			if (Mouse.Captured == null) return;
			

			// The Shape is gotten from the mouse event.
			var shape = TargetShape(e);
				
			// The mouse position relative to the target of the mouse event.
			var mousePosition = RelativeMousePosition(e);
            
            // draw connection 
			if (!ButtonDown.Equals(ButtonCommand.None) && NewConnection != null)
			{
				NewConnection.updatePoints(mousePosition);
			}
            // resize or move shape
			else if(ButtonDown.Equals(ButtonCommand.None) && shape != null)
			{
				if (_isResizing)
				{   // resizing
					ResizeShape(shape, mousePosition);
				}
				else
				{   // move shape
					MoveShape(mousePosition);
				}
			}
			
		}

        public void MouseUp(MouseButtonEventArgs e)
		{
			// The Shape is gotten from the mouse event.
            e.MouseDevice.Target.ReleaseMouseCapture();
			var shape = TargetShape(e);
			


			// The mouse position relative to the target of the mouse event.
			var mousePosition = RelativeMousePosition(e);
            // The mouse is released, as the move operation is done, so it can be used by other controls.
            e.MouseDevice.Target.ReleaseMouseCapture();

            if (!ButtonDown.Equals(ButtonCommand.None) && NewConnection != null)
			{
                AddConnection(shape);
			}
			else if(ButtonDown.Equals(ButtonCommand.None) && shape != null)
			{
                if (_isResizing) ResizeShapeDone(shape, mousePosition);
                else MoveShapeDone(mousePosition);

                if (DateTime.Now.Ticks-_doubleClickTimer < doubleClickTimeout)
				{
                    EditClassContent(shape);					
				}		
			}
            //if(ButtonDown.Equals(ButtonCommand.None) && shape != null) SelectShape(shape);
            _doubleClickTimer = DateTime.Now.Ticks;


		}

		public void MouseDown(MouseButtonEventArgs e)
		{
			var shape = TargetShape(e);
			if (shape == null) return;

            SelectShape(shape);
            // The mouse position relative to the target of the mouse event.
            var mousePosition = RelativeMousePosition(e);

            if (ButtonDown == ButtonCommand.Aggregation ||
                ButtonDown == ButtonCommand.Association ||
                ButtonDown == ButtonCommand.Composition)
            {
                AddConnectionInit(shape, mousePosition);
            } else
            {
                ResizeShapeInit(shape, e);
            }

			_initialMousePosition = mousePosition;

            e.MouseDevice.Target.CaptureMouse();
        }

		public void CherryPick(MouseEventArgs e)
		{
			var cmd = TargetCommand(e);
			CommandController.setActiveCommand(cmd);
		}
		
		public void MouseClicked(MouseEventArgs e)
		{
			
			if (!ButtonDown.Equals(ButtonCommand.None))
			{
				if (e == null) return;
				var mousePosition = RelativeMousePosition(e);

				Point anchorpoint = new Point(mousePosition.X - minShapeWidth / 2, mousePosition.Y - minShapeHeight / 2);

				// default is normal class/comment --- comment should perhaps have some modifications
				string stereoType = "";
				bool isAbstract = false;

				switch(ButtonDown){
					case ButtonCommand.Abstract:
						isAbstract = true;
						break;
					case ButtonCommand.Interface:
						stereoType = "interface";
						break;
					case ButtonCommand.Class:
					case ButtonCommand.Comment:
						break;
					default:
						return;
				}


				execCommand(new AddClassCommand(null, stereoType, isAbstract, anchorpoint));
                // if shift is down, you can add several classes
                if (!_multiAdd) ButtonDown = ButtonCommand.None;

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
					CommandController = CommandTree.load(reader);
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
			if (sfd.FileName == "") return saved;
			System.IO.FileStream filestream = (System.IO.FileStream)sfd.OpenFile();
			   
			CommandTree.asyncSave(CommandController, filestream);
			saved = true;
			return saved;
		}

        private void Help()
        {

            string msg =      "CTRL+S : save file\n"
                            + "CTRL+O : load file\n"
                            + "CTRL+Z : undo\n"
                            + "CTRL+Y : redo\n"
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
			// From the shapes visual element, the Shape object which is the DataContext is retrieved.
			return shapeVisualElement.DataContext as BaseCommand;
		}
		private Class TargetShape(MouseEventArgs e)
		{
			// Here the visual element that the mouse is captured by is retrieved.
			var shapeVisualElement = (FrameworkElement)e.MouseDevice.Target;
			if (shapeVisualElement == null) return null;
			// From the shapes visual element, the Class object which is the DataContext is retrieved.
			return shapeVisualElement.DataContext as Class;
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

		public void execCommand(BaseCommand command)
		{
			CommandController.addAndExecute(command);
			
		}

	
	}
}