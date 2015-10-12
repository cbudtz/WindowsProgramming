using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using System.Xml.Schema;

namespace Area51.SoftwareModeler.Models
{
   public class DummyCommand : BaseCommand
    {
        public event EventHandler CanExecuteChanged;

        public DummyCommand()
            :base(null)
        {

        }

        public override void addChild(BaseCommand child)
        {
            throw new NotImplementedException();
        }

        public override void execute()
        {
            throw new NotImplementedException();
        }

        public List<BaseCommand> getChildren()
        {
            throw new NotImplementedException();
        }

        public BaseCommand getParent()
        {
            throw new NotImplementedException();
        }
        public override void unExecute()
        {
            throw new NotImplementedException();
        }

        public override void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement(this.GetType().Name);
            //TODO: Fill in attributes
            reader.ReadEndElement();
            
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(this.GetType().Name);
            //TODO: Fill in attributes
            writer.WriteEndElement();
        }

        public override XmlSchema GetSchema()
        {
            return null;
        }
    }
}
