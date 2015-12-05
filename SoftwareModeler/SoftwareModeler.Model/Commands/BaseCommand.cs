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
    [XmlInclude(typeof(UpdateClassInfoCommand))]
    [XmlInclude(typeof(AddConnectionCommand))]
    [XmlInclude(typeof(CopyClassCommand))]
    public abstract class BaseCommand : NotifyBase
    {
        //Static
        public static int nextid = 0;

        public int BranchLayer { get; set; }
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
        public BaseCommand Parent { get { return parent; } set { parent = value; } }
        public List<BaseCommand> Children { get { return children; } }

        //Abstract - Command pattern
        public abstract void execute();
        public abstract void unExecute();

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
    }
}
