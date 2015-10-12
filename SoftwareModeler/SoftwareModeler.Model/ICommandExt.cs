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
    public abstract class ICommandExt : IXmlSerializable
    {
        public abstract void execute();
        public abstract void unExecute();
        public abstract bool canExecute();
        public abstract ICommandExt getParent();
        public abstract List<ICommandExt> getChildren();
        public abstract void addChild(ICommandExt child);
        public abstract XmlSchema GetSchema();
        public abstract void ReadXml(XmlReader reader);
        public abstract void WriteXml(XmlWriter writer);
    }
}
