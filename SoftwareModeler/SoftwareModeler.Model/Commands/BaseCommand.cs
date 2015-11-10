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
    public abstract class BaseCommand
    {
        //Static
        public static int nextid = 0;
        public int BranchLayer { get; set; }
        public Color color = Colors.Transparent;
        //Fields
        protected string parentstr = "hey";
        protected BaseCommand parent = null;
        protected List<BaseCommand> children = new List<BaseCommand>();
        public int id;

        //Getters and setters
        [XmlIgnore]
        public BaseCommand Parent { get { return parent; } set { parent = value; } }
        public List<BaseCommand> Children { get { return children; } }

        //Abstract - Command pattern
        public abstract void execute();
        public abstract void unExecute();
        //Inherited methods
        public  void addChild(BaseCommand child)
        {
            child.BranchLayer += children.Count+ this.BranchLayer;
            children.Add(child);
        }
        public BaseCommand()
        {
            this.id = BaseCommand.getNextId();
        }

        public static int getNextId()
        {
            return nextid++;
        }
    }
}
