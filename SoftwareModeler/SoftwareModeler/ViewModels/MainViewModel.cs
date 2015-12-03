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
	//public enum ConnectionToAdd { NONE, ASSOCIATION, AGGREGATION, COMPOSITION }
	//public enum ClassToAdd { NONE, NORMAL, ABSTRACT, INTERFACE, COMMENT}
	public enum ButtonCommand { NONE, CLASS, ABSTRACT, INTERFACE, COMMENT, ASSOCIATION, AGGREGATION, COMPOSITION}
	
	public class MainViewModel : ViewModelBase
	{
		private EditClassPopupWindow editClassWindow = new EditClassPopupWindow();

		private ButtonCommand buttonDown = ButtonCommand.NONE;

		double initialWidth = 0;
		private double minShapeWidth = 150;
		private double minShapeHeight = 100;
		public bool isEditable { get; set; } = false;
		
		private Connection newConnection = null;

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
        private List<Shape> selectedShapes = new List<Shape>();
        private bool multiSelect = false;
        private bool isResizing = false;
        // Used for saving the classRep that a line is drawn from, while it is being drawn.
        private Shape addingLineFrom;
		// Saves the initial point that the mouse has during a move operation.
		private Point initialMousePosition;
        // Saves the initial point that the classRep has during a move operation.
        private List<Point> initialShapePosition = new List<Point>();
		private long doubleClickTimer;
		private long doubleClickTimeout = 500*10000; // nanosec. is 500msec

		//view access to observables
		public ObservableCollection<BaseCommand> commands { get { return ShapeCollector.getI().commands; } }
		public ObservableCollection<Shape> classes { get { return ShapeCollector.getI().obsShapes; } }
		public ObservableCollection<Connection> connections { get { return ShapeCollector.getI().obsConnections; } }
		public ObservableObject MaxBranchLayer { get { return ShapeCollector.getI().MaxBranchLayer; } }
		private Class classToEdit = null;


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

			multiSelect = false;
			if(ctrl > 0)
			{
				multiSelect = true;
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
				ClearSelectedShapes();
				buttonDown = ButtonCommand.NONE;
			}
			else if(del > 0 && selectedShapes != null)
			{
				selectedShapes.ForEach(x => execCommand(new DeleteShapeCommand(x)));
				ClearSelectedShapes();
			}
		}

		public void EditClassContent(Shape shape)
		{
            bool newWind = (editClassWindow == null ||
                                    editClassWindow.Visibility == Visibility.Collapsed ||
                                    editClassWindow.Visibility == Visibility.Hidden ||
                                    !editClassWindow.IsEnabled);
            if (editClassWindow != null) editClassWindow.Close();

            editClassWindow = new EditClassPopupWindow();
            classToEdit = (Class)shape;
            editClassWindow.ClassName.Text = classToEdit.name;
            editClassWindow.StereoType.Text = classToEdit.StereoType;
            editClassWindow.IsAbstract.IsChecked = classToEdit.IsAbstract;
            editClassWindow.Methods.ItemsSource = classToEdit.Methods;
            editClassWindow.Attributes.ItemsSource = classToEdit.Attributes;
            editClassWindow.Ok.Command = EditClassContentOkCommand;
            editClassWindow.Cancel.Command = EditClassContentCancelCommand;
            editClassWindow.Show();
        }

		public void EditClassOk()
		{
		  var methods =   editClassWindow.Methods.Items.OfType<Method>().ToList<Method>(); ;// as List<Method>;

			var attributes = editClassWindow.Attributes.Items.OfType<Models.Attribute>().ToList<Models.Attribute>();

			execCommand(new UpdateClassInfoCommand(classToEdit, editClassWindow.ClassName.Text, editClassWindow.StereoType.Text, editClassWindow.IsAbstract.IsChecked, methods, attributes));
			
			classToEdit = null;
			ClearSelectedShapes();
			editClassWindow.Hide();
		}

        public void EditClassCancel()
        {
            classToEdit = null;
            editClassWindow.Visibility = Visibility.Collapsed;
        }

        public void ClearSelectedShapes()
		{
			selectedShapes.ForEach(x => x.IsSelected = false);
			selectedShapes.Clear();
            initialShapePosition.Clear();
		}

		public void SelectShape(Shape s)
		{
			if (s == null) return;
			if (selectedShapes == null) selectedShapes = new List<Shape>();
			
			if (!multiSelect) ClearSelectedShapes();
            if (!selectedShapes.Contains(s))
            {
                s.IsSelected = true;
                selectedShapes.Add(s);
                initialShapePosition.Add(new Point(s.X, s.Y));
            }
		}

		public void MoveShape(Point mousePosition)
		{
            for(int i = 0; i < selectedShapes.Count; i++) {
                selectedShapes.ElementAt(i).X = initialShapePosition.ElementAt(i).X + (mousePosition.X - initialMousePosition.X);
                selectedShapes.ElementAt(i).Y = initialShapePosition.ElementAt(i).Y + (mousePosition.Y - initialMousePosition.Y);

                // lambda expr. update all connections. first connections where end classRep is the moving classRep then where start classRep is moving classRep
                connections.Where(x => x.End.id == selectedShapes.ElementAt(i).id).ToList().ForEach(x => x.End = selectedShapes.ElementAt(i));
                connections.Where(x => x.Start.id == selectedShapes.ElementAt(i).id).ToList().ForEach(x => x.Start = selectedShapes.ElementAt(i));
            }
		}

        public void MoveShapeDone(Point mousePosition)
        {
           
            double xOffset = mousePosition.X - initialMousePosition.X;
            double yOffset = mousePosition.Y - initialMousePosition.Y;

            if (Math.Abs(xOffset) < 10 && Math.Abs(yOffset) < 10) return;

            for (int i = 0; i < selectedShapes.Count; i++) {
                // The Shape is moved back to its original position, so the offset given to the move command works.
                selectedShapes.ElementAt(i).X = initialShapePosition.ElementAt(i).X;
                selectedShapes.ElementAt(i).Y = initialShapePosition.ElementAt(i).Y;              
                
                //initialShapePosition.Insert(i, new Point(selectedShapes.ElementAt(i).X + xOffset, selectedShapes.ElementAt(i).Y + yOffset));
                Console.WriteLine("moved done: " + new Point(selectedShapes.ElementAt(i).X + xOffset, selectedShapes.ElementAt(i).Y + yOffset));
              
                execCommand(new MoveShapeCommand(selectedShapes.ElementAt(i), xOffset, yOffset));
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

            initialMousePosition = mousePosition;

            initialWidth = border.ActualWidth;

            isResizing = true;

            
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
            if (!isResizing) return;          
            if (shape == null) return;
           
            // The Shape is moved back to its original position, so the offset given to the move command works.
            shape.Width = initialMousePosition.X - shape.X;
            shape.Height = initialMousePosition.Y - shape.Y;

            double xOffset = mousePosition.X - initialMousePosition.X;
            double yOffset = mousePosition.Y - initialMousePosition.Y;

            if (Math.Abs(shape.Width + xOffset) < minShapeWidth) xOffset = minShapeWidth - shape.Width;
            if (Math.Abs(shape.Height + yOffset) < minShapeHeight) yOffset = minShapeHeight - shape.Height;

            execCommand(new ResizeShapeCommand(shape, xOffset, yOffset));

            isResizing = false;
        }

        public void AddConnection(Shape shape)
        {
            if (shape == null || newConnection.Start.Equals(shape))
            {
                connections.Remove(newConnection);
            }
            else
            {
                newConnection.End = shape;
                execCommand(new AddConnectionCommand(newConnection.Start, "", newConnection.End, "", newConnection.type)); // TODO command not implemented yet

            }
            newConnection = null;
        }
      
        public void AddConnectionInit(Shape shape, Point mousePosition)
        {
            ConnectionType type = ConnectionType.Association;
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
                    return;
            }
            
            newConnection = new Connection(shape, "", null, "", type);
            newConnection.EndPoint = mousePosition;
            connections.Add(newConnection);
        }

        public void MouseMove(MouseEventArgs e)
		{
			if (Mouse.Captured == null) return;
			
			// The Shape is gotten from the mouse event.
			var shape = TargetShape(e);
				
			// The mouse position relative to the target of the mouse event.
			var mousePosition = RelativeMousePosition(e);
            
            // draw connection 
			if (!buttonDown.Equals(ButtonCommand.NONE) && newConnection != null)
			{
				newConnection.updatePoints(mousePosition);
			}
            // resize or move shape
			else if(buttonDown.Equals(ButtonCommand.NONE) && shape != null)
			{
				if (isResizing)
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
			var shape = TargetShape(e);
			
			// The mouse position relative to the target of the mouse event.
			var mousePosition = RelativeMousePosition(e);
            // The mouse is released, as the move operation is done, so it can be used by other controls.
            e.MouseDevice.Target.ReleaseMouseCapture();

            if (!buttonDown.Equals(ButtonCommand.NONE) && newConnection != null)
			{
                AddConnection(shape);
			}
			else if(buttonDown.Equals(ButtonCommand.NONE) && shape != null)
			{
                if (isResizing) ResizeShapeDone(shape, mousePosition);
                else MoveShapeDone(mousePosition);

                if (DateTime.Now.Ticks-doubleClickTimer < doubleClickTimeout)
				{
                    EditClassContent(shape);					
				}		
			}
			doubleClickTimer = DateTime.Now.Ticks;
		}

		public void MouseDown(MouseButtonEventArgs e)
		{
			var shape = TargetShape(e);
			if (shape == null) return;

            SelectShape(shape);
            // The mouse position relative to the target of the mouse event.
            var mousePosition = RelativeMousePosition(e);

            if (buttonDown == ButtonCommand.AGGREGATION ||
                buttonDown == ButtonCommand.ASSOCIATION ||
                buttonDown == ButtonCommand.COMPOSITION)
            {
                AddConnectionInit(shape, mousePosition);
            } else
            {
                ResizeShapeInit(shape, e);
            }

			initialMousePosition = mousePosition;

            e.MouseDevice.Target.CaptureMouse();
        }

		public void CherryPick(MouseEventArgs e)
		{
			var cmd = TargetCommand(e);
			commandController.setActiveCommand(cmd);
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
						stereoType = "interface";
						break;
					case ButtonCommand.CLASS:
					case ButtonCommand.COMMENT:
						break;
					default:
						return;
				}


				execCommand(new AddClassCommand(null, stereoType, isAbstract, anchorpoint, visibility));
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
			if (sfd.FileName == "") return saved;
			System.IO.FileStream filestream = (System.IO.FileStream)sfd.OpenFile();
			   
			CommandTree.asyncSave(commandController, filestream);
			saved = true;
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

		public void execCommand(BaseCommand command)
		{
			commandController.addAndExecute(command);
			
		}

	
	}
}