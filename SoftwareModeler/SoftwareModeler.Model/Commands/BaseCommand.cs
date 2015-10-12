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
    public abstract class BaseCommand : IXmlSerializable
    {

        protected BaseCommand parent;
        protected List<BaseCommand> children = new List<BaseCommand>();
        public abstract void execute();
        public abstract void unExecute();
        public BaseCommand Parent { get { return parent; } }
        public List<BaseCommand> Children { get { return children; } }
        public abstract void addChild(BaseCommand child);
        public abstract XmlSchema GetSchema();
        public abstract void ReadXml(XmlReader reader);
        public abstract void WriteXml(XmlWriter writer);

        protected BaseCommand(BaseCommand _parent)
        {
            this.parent = _parent;
        }
    }
}
