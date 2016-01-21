using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Area51.SoftwareModeler.Models;

namespace Area51.SoftwareModeler.Models.Commands
{
    [XmlInclude(typeof(AddClassCommand))]
    [XmlInclude(typeof(DummyCommand))]
    [XmlInclude(typeof(MoveShapeCommand))]
    [XmlInclude(typeof(ResizeShapeCommand))]
    [XmlInclude(typeof(DeleteShapeCommand))]
    [XmlInclude(typeof(DeleteConnectionCommand))]
    [XmlInclude(typeof(UpdateClassInfoCommand))]
    [XmlInclude(typeof(AddConnectionCommand))]
    [XmlInclude(typeof(CopyClassCommand))]
    public abstract class BaseCommand : NotifyBase
    {
        //Static
        public static int nextid = 0;

        public int BranchLayer { get; set; }

        public int numCollapsed { get; set; } = 0;

        private bool isCollapsed = false;
        public bool IsCollapsed { get {return isCollapsed; } set { isCollapsed = value; NotifyPropertyChanged();
            if (value)
            {
                ShapeCollector.GetI().treeArrows.Remove(LineToParent);
            }
            else
            {
               ShapeCollector.GetI().treeArrows.Add(LineToParent);
                }
        } }
        
        private bool collapseable = false;
        public bool Collapseable { get { return collapseable; } set { collapseable = value; NotifyPropertyChanged(); } }

        private bool hasCollapseableChild = false;
        public bool HasCollapseableChild { get { return hasCollapseableChild; } set { hasCollapseableChild = value; NotifyPropertyChanged(); } }
        private int y;
        public int prevY;
        public int Y { get { return y; } set { y = value; NotifyPropertyChanged(); } }

        [XmlIgnore]
        public LineCommandTree LineToParent { get; set; }
        [XmlIgnore]
        //private Color color = Colors.Azure;
        private SolidColorBrush color = new SolidColorBrush(Colors.Azure);
        [XmlIgnore]
        public SolidColorBrush Color { get { return color; } set { color = value; NotifyPropertyChanged(); } }
        //Fields
        protected string parentstr = "hey";
        protected BaseCommand parent = null;
        protected List<BaseCommand> children = new List<BaseCommand>();
        public int Id { get; set; }

        //Getters and setters
        [XmlIgnore]
        public BaseCommand Parent { get { return parent; } set { parent = value; Y = Id*30 - numCollapsed*30;
            if (children.Count == 0 && value != null) Collapseable = true;
        } }
        public List<BaseCommand> Children { get { return children; } }

        //Abstract - Command pattern
        public abstract void execute();
        public abstract void unExecute();

        public int CalculateCollapseHeight()
        {
            if (children.Count == 0) return 1;
            if (!Collapseable) return 0;
            return children.ElementAt(0).CalculateCollapseHeight() + 1;
            
        }
        public void Collapse()
        {

            if (parent != null)
            {
                prevY = y;
                Y = parent.Y;
            }
            if (collapseable && children.Count <= 1)
            {
                IsCollapsed = true;
                Children.ForEach(x=> x.Collapse());
            }    
        }

        private void CollapseNodes()
        {
            
        }
        public void Expand()
        {
            if (parent != null)
            {
                Y = prevY;
            }
            
            IsCollapsed = false;
            children.ForEach(x=> x.Expand());
            
        }
        //Inherited methods
        public  int addChild(BaseCommand child, int currentBranchLayer)
        {
            
            if (children.Count == 0)
            {
                child.BranchLayer = this.BranchLayer;
            }
            else
            {
                child.BranchLayer = ++currentBranchLayer;
            }
            Console.WriteLine(currentBranchLayer + "CurrentBranchLayer");
            children.Add(child);
            if (children.Count > 1 && parent != null) Collapseable = false;
            else Collapseable = true;
            return currentBranchLayer;
        }
        public BaseCommand()
        {
            this.Id = BaseCommand.getNextId();
            
        }

        public static int getNextId()
        {
            return nextid++;
        }
        public String CommandType { get { return this.GetType().Name.Substring(0, 1); } }
    }
}
