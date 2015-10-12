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
        protected string parentstr = "hey";
        protected BaseCommand parent = null;
        protected List<BaseCommand> children = new List<BaseCommand>();
        public abstract void execute();
        public abstract void unExecute();

        [XmlIgnore]
        public BaseCommand Parent { get { return parent; } set { parent = value; } }
        public List<BaseCommand> Children { get { return children; } }
        public  void addChild(BaseCommand child)
        {
            children.Add(child);
        }
        public BaseCommand()
        {

        }
        protected BaseCommand(BaseCommand _parent)
        {
            this.parent = _parent;
        }
    }
}
