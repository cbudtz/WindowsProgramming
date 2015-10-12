using Area51.SoftwareModeler.Model.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Area51.SoftwareModeler.Models
{
    [XmlInclude(typeof(DummyCommand))]
    [XmlInclude(typeof(MoveShapeCommand))]
    [XmlInclude(typeof(AddShapeCommand))]
    public abstract class BaseCommand
    {
        //Static
        public static int nextid = 0;
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
            children.Add(child);
        }
        public BaseCommand()
        {
            this.id = BaseCommand.getNextId();
        }
        protected BaseCommand(BaseCommand _parent)
        {
            this.id = BaseCommand.getNextId();
            this.parent = _parent;
        }

        public static int getNextId()
        {
            return nextid++;
        }
    }
}
